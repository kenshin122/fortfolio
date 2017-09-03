using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;


public class FIHttp{
	[System.Serializable]
	public class FIHttpSettings{
		public string serverUrl;
		public bool showDebug;
	}

	Dictionary<string,string> cookies = new Dictionary<string, string>();
	FIHttpSettings settings;
	CLCM cm;
	public FIHttp(FIBuildOptions _options,CLCM _cm){
		settings = _options.httpSettings;
		cm = _cm;
	}
	string TotalCookie{
		get{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			var list = cookies.ToList();
			for(int i = 0 ; i < list.Count ; i++){
				builder.Append(list[i].Key);
				if(string.IsNullOrEmpty(list[i].Value) == false){
					builder.Append("=");
					builder.Append(list[i].Value);
				}
				if(i<list.Count-1){
					builder.Append(";");
				}
			}
			return builder.ToString();
		}
	}
	FIHttpResult _result;
	public IHttpResult Result{
		get{
			return _result;
		}
	}
	public void SendRequest(string req, object obj,System.Action<FIErr,JObject> onRes){
		cm.StartCoroutine(InternalSendRequest(req,obj,onRes));
	}
	IEnumerator InternalSendRequest(string req, object obj,System.Action<FIErr,JObject> onRes){
		yield return cm.StartCoroutine(SendRequestAsync(req,obj));
		onRes( _result.Err, _result.Res );
	}
	public IEnumerator SendRequestAsync(string req, object obj){
		_result = null;
		System.Text.StringBuilder reqBuilder = new System.Text.StringBuilder();
		if(settings.showDebug == true){
			reqBuilder.AppendLine("Request=========================");
		}

		if(settings.showDebug == true){
			reqBuilder.AppendLine("HEADER=========================");
		}
		var dic = new Dictionary<string,string>();
		if(cookies.Count > 0){
			dic.Add("Cookie",TotalCookie);
			if(settings.showDebug == true){
				reqBuilder.AppendLine($"Cookie={TotalCookie}");
			}
		}
		dic.Add("Content-type", "application/json");

		//Data insert
		string reqJsonData = null;
		if(obj == null){
			reqJsonData = "{}";
		}else{
			reqJsonData = JsonConvert.SerializeObject( obj );

		}
		byte[] totalByte = System.Text.Encoding.UTF8.GetBytes( reqJsonData );
		if(settings.showDebug == true){
			reqBuilder.AppendLine(reqJsonData);
			Debug.Log(reqBuilder.ToString());
		}

		//Send
		WWW some = new WWW($"http://{settings.serverUrl}/{req}",totalByte,dic);
		while(some.isDone == false){
			yield return null;
		}

		if(some.error != null){
			_result = new FIHttpResult(){_err=FIErr.Client_HttpErr};
			yield break;
		}

		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		if(settings.showDebug)
			builder.AppendLine("Result=========================");
		if(!string.IsNullOrEmpty(some.error)){
			builder.AppendLine($"Not connected={some.error}");
			Debug.Log(builder.ToString());
			_result = new FIHttpResult(){_err=FIErr.InternetNotConnected};
			yield break;
		}


		if(settings.showDebug == true)
			builder.AppendLine("HEADER=========================");
		foreach(var item in some.responseHeaders){
			if(settings.showDebug == true)
				builder.AppendLine($"{item.Key}:{item.Value}");
			if(item.Key=="SET-COOKIE"){
				string[] splitted = item.Value.Split(';');
				foreach(var cookie in splitted){
					if(string.IsNullOrEmpty(cookie))
						continue;
					string[] keyAndValue = cookie.Split('=');
					if(keyAndValue.Length == 1){
						if(cookies.ContainsKey(keyAndValue[0]) == false){
							cookies.Add(keyAndValue[0],null);
						}else{
							cookies[keyAndValue[0]] = null;
						}
					}else{
						if(cookies.ContainsKey(keyAndValue[0]) == false){
							cookies.Add(keyAndValue[0],keyAndValue[1]);
						}else{
							cookies[keyAndValue[0]] = keyAndValue[1];
						}
					}

				}
			}
		}
		if(settings.showDebug == true){
			builder.AppendLine("DATA==========================");
			builder.AppendLine(some.text);
			Debug.Log(builder.ToString());
		}
		var resObj = JsonConvert.DeserializeObject<JObject>(some.text);
		_result = new FIHttpResult(){_err=(FIErr)resObj["err"].Value<long>(),_res=resObj};
	}
	class FIHttpResult:IHttpResult{
//		public bool _isDone;
//		public bool IsDone{get{return _isDone;}}
		public FIErr _err;
		public FIErr Err{get{return _err;}}
		public JObject _res;
		public JObject Res{get{return _res;}}
	}
}
public interface IHttpResult{
//	bool IsDone{get;}
	FIErr Err{get;}
	JObject Res{get;}
}
//public class FIHttpOptions{
//	public string url;
//	public bool showHttpDebug = true;
//}