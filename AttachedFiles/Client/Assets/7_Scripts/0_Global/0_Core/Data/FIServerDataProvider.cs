using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Reflection;
using Zenject;
using UniRx;
public class FIServerDataProvider:FIDataProviderBase,IDataProvider {
	[Inject]
	FIBuildOptions settings;

	[Inject]
	FIHttp http;
	public IObservable<Tuple<FIErr,JObject>> Get(string req,JObject obj = null){
		return Observable.Create<Tuple<FIErr,JObject>>(x=>{
			var cancelSource = new FICancelSource();
			var token = new CancellationToken(cancelSource);
			Request(req,obj, token, (err,res)=>{
				x.OnNext(Tuple.Create<FIErr,JObject>(err,res));
				x.OnCompleted();
			});
			return cancelSource;
		});
	}
	public IDataProviderRequestAsync RequestAsync(string req, JObject obj, CancellationToken? token){
		FIAsyncData asyncData = new FIAsyncData();
//		Debug.Log("Requesting="+req);
		http.SendRequest(req,obj,(err,res)=>{
			asyncData._isDone = true;
			asyncData._err = err;
			asyncData._res = res;
			if(err == FIErr.Okay){
				ProcessResult(res);
			}
		});
		return asyncData;
	}
	public void Request(string req, JObject obj, CancellationToken? token, System.Action<FIErr,JObject> onRes){
		Debug.Log("Requesting="+req);
		http.SendRequest(req,obj,(err,res)=>{
			if(err == FIErr.Okay){
				ProcessResult(res);
			}
			onRes(err,res);
		});
	}

}
