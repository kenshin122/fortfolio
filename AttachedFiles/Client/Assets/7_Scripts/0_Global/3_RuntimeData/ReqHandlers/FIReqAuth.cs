using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static partial class FIClientReqHandler{
	public static JObject sess;
	public static JObject enc_auth_logindid(FIFakeContext HttpContext){
		CheckParameterExists(HttpContext,"deviceID");
		var deviceID = HttpContext.body["deviceID"].Value<string>();

		DBAuthData user = null;
		user = (from item in HttpContext.dbContext.GetList<DBAuthData>()
			where item.grantValue == deviceID && item.grantType == GrantType.DeviceID
			select item).FirstOrDefault();

		//Check user is registered..
		if(user == null){
			//Newly created! need nickname to login. confirm!
			user = new DBAuthData(GrantType.DeviceID,deviceID);
			HttpContext.dbContext.InsertWithID(user);
		}

		//Create Session..
		sess = new JObject( new JProperty("authuid",user.uid) );
		return GetNoErrJObject();
	}
	public static JObject Internal_IsUserLoggedIn(FIFakeContext HttpContext){
		var userInfoJObject = (HttpContext.Items["sess"] as JObject)["userinfo"] as JObject;
		if(userInfoJObject == null){
			throw new FIException(FIErr.Auth_UserNotLoggedIn,"user not logged in");
		}
		return userInfoJObject;
	}
	public static JObject enc_sess_auth_createuserinfo(FIFakeContext HttpContext){
		CheckParameterExists(HttpContext,"nick");
		var authuid = (HttpContext.Items["sess"] as JObject)["authuid"].Value<int>();
		//check already has user data..
		var newUserData = (from item in HttpContext.dbContext.GetList<DBUserInfo>()
			where item.authUID == authuid
			select item).FirstOrDefault();
		
		if(newUserData != null){
			throw new FIException(FIErr.Auth_AlreadyUserCreated,"Already created user");
		}

		//create data in database..
		var createdUser = HttpContext.dbContext.Create<DBUserInfo>();
		createdUser.authUID = authuid;
		createdUser.nick = (string)HttpContext.body["nick"];
		return GetNoErrJObject();
	}
	public static JObject enc_sess_auth_userLogin(FIFakeContext HttpContext){
		//Check already logged in..
		var userInfoJObject = (HttpContext.Items["sess"] as JObject)["userinfo"] as JObject;
		if(userInfoJObject != null){
			throw new FIException(FIErr.Auth_AlreadyUserLoggedIn);
		}

		var authuid = (HttpContext.Items["sess"] as JObject)["authuid"].Value<int>();

		//find if there is user data
		var newUserData = (from item in HttpContext.dbContext.GetList<DBUserInfo>()
			where item.authUID == authuid
			select item).FirstOrDefault();

		if(newUserData == null){
			throw new FIException(FIErr.Auth_NoUserInDatabase,"There is no user data in db");
		}

		var userJObject = JObject.FromObject(newUserData);
		(HttpContext.Items["sess"] as JObject).Add(new JProperty("userinfo",userJObject));
		InsertUpdated(HttpContext,newUserData);
		return GetDefaultJObject(HttpContext);
//		return GetNoErrJObject( GetArrProperty("updated",new object[]{newUserData}) );
	}
}