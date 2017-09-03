using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static partial class FIClientReqHandler{
	static void InsertUpdated(FIFakeContext context, params FIRBaseData[] args){
		if(context.Items.ContainsKey("updated") == false)
			context.Items.Add("updated",new Dictionary<System.Type,Dictionary<int,FIRBaseData>>());

		var dic = context.Items["updated"] as Dictionary<System.Type,Dictionary<int,FIRBaseData>>;
		foreach(var item in args){
			var curType = item.GetType();
			Dictionary<int,FIRBaseData> curDic = null;
			if(dic.ContainsKey(curType) == false){
				curDic = new Dictionary<int, FIRBaseData>();
				dic.Add(curType,curDic);
			}else{
				curDic = dic[curType];
			}
			if(curDic.ContainsKey(item.uid) == true){
				curDic.Remove(item.uid);
			}
			curDic.Add(item.uid,item);
		}
	}
	static void InsertDeleted(FIFakeContext context, params FIRBaseData[] args){
		if(context.Items.ContainsKey("deleted") == false)
			context.Items.Add("deleted",new Dictionary<System.Type,Dictionary<int,FIRBaseData>>());
		
		var dic = context.Items["deleted"] as Dictionary<System.Type,Dictionary<int,FIRBaseData>>;
		foreach(var item in args){
			var curType = item.GetType();
			Dictionary<int,FIRBaseData> curDic = null;
			if(dic.ContainsKey(curType) == false){
				curDic = new Dictionary<int, FIRBaseData>();
				dic.Add(curType,curDic);
			}else{
				curDic = dic[curType];
			}
			if(curDic.ContainsKey(item.uid) == true){
				curDic.Remove(item.uid);
			}
			curDic.Add(item.uid,item);
		}
	}
	static System.DateTime startedTime = System.DateTime.Now;
	static System.DateTime loginTime = System.DateTime.Now;
	static System.DateTime CurrentTime{
		get{
			return loginTime + (System.DateTime.Now - startedTime);
		}
	}
	static JObject GetDefaultJObject(FIFakeContext context,params JProperty[] args){
		var jObj = new JObject();
		jObj.Add( new JProperty("err",FIErr.Okay) );
		
		InsertUpdated(context,
			new DBGlobalData(){
				ServerTime=CurrentTime
			}
		);
		if(args.Length > 0)
			jObj.Add( args );

		if(context.Items.ContainsKey("updated") == true){
			var listOfItems = new List<object>();
			var topDic = context.Items["updated"] as Dictionary<System.Type,Dictionary<int,FIRBaseData>>;
			foreach(var outer in topDic){
				foreach(var inner in outer.Value){
					listOfItems.Add(inner.Value);
				}
			}
			jObj.Add( GetArrProperty("updated", listOfItems.ToArray() ) );
		}
		if(context.Items.ContainsKey("deleted") == true){
			var listOfItems = new List<object>();
			var topDic = context.Items["deleted"] as Dictionary<System.Type,Dictionary<int,FIRBaseData>>;
			foreach(var outer in topDic){
				foreach(var inner in outer.Value){
					listOfItems.Add(inner.Value);
				}
			}
			jObj.Add( GetArrProperty("deleted", listOfItems.ToArray() ) );
		}

		return jObj;
	}

	static JObject GetNoErrJObject(params JProperty[] args){
		return new JObject(new JProperty("err",FIErr.Okay),args);
	}
	static JProperty GetArrProperty(string name, params object[] args){
		var prop = new JProperty(name);

		var content = new JObject();
		prop.Value = content;
		Dictionary<System.Type,List<object>> dic = new Dictionary<System.Type, List<object>>();
		foreach(var item in args){
			if(dic.ContainsKey(item.GetType()) == false){
				dic.Add( item.GetType() , new List<object>() );
			}
			dic[item.GetType()].Add(item);
		}
		foreach(var item in dic){
			var tempArr = new JArray();
			var tempProp = new JProperty( item.Key.Name, tempArr );
			foreach(var tempObj in item.Value){
				tempArr.Add( JObject.FromObject(tempObj) );
			}
			content.Add(tempProp);
		}
		
		return prop;
	}

	static void CheckParameterExists(FIFakeContext context,params string[] args){
		var requiredList = args.ToList();
		foreach(var item in args){
			var obj = context.body[item];
			if(obj != null){
				requiredList.Remove(item);
			}
		}
		if(requiredList.Count > 0){
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for(int i = 0 ; i < requiredList.Count ; i++){
				builder.Append(requiredList[i]);
				if(i < requiredList.Count - 1){
					builder.Append(",");
				}
			}
			throw new FIException(FIErr.ParameterMissing,builder.ToString());
		}
	}
}