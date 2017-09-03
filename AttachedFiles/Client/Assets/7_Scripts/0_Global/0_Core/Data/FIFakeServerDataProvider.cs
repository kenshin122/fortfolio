using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Zenject;
using UnityEngine;
using UniRx;
public class FIFakeServerDataProvider:FIDataProviderBase,IDataProvider{
	[Inject]
	FIEasy easy;
	[Inject]
	CLCM cm;
	[Inject]
	FIBuildOptions settings;
	[Inject]
	GDManager staticData;
//	[Inject]
//	FIRBaseDataTypeContainer dataTypeContainer;

	public FIRData srcContainer = new FIRData();
	Dictionary<string,MethodInfo> methodDic = new Dictionary<string, MethodInfo>();

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
	public void Request(string req, JObject obj, CancellationToken? token, System.Action<FIErr,JObject> onRes){
		cm.StartCoroutine(InternalRequest(req,obj, token ,onRes));
	}
	IEnumerator InternalRequest(string req, JObject obj, CancellationToken? token, System.Action<FIErr,JObject> onRes){
		var asyncRes = RequestAsync(req,obj,token);
		while(asyncRes.IsDone == false)
			yield return null;
		onRes(asyncRes.Err,asyncRes.Res);
	}
	public IDataProviderRequestAsync RequestAsync(string req, JObject obj, CancellationToken? token){
		FIAsyncData asyncData = new FIAsyncData();
		asyncData._isDone = false;

		if(token != null){
			var checkGO = new GameObject("Checker");
			checkGO.CLUpdateAsObservable().Subscribe(x=>{
				if(token.Value.IsCancellationRequested == false)
					return;
				asyncData._err = FIErr.Client_Canceled;
				asyncData._isDone = true;
				asyncData._res = new JObject( new JProperty("err",FIErr.Client_Canceled) );
				GameObject.Destroy(checkGO);
			});
		}

		cm.DelayedRun(settings.fakeDelay,()=>{
			if(token != null){
				if(token.Value.IsCancellationRequested == true){
					return;
				}
			}
			if(settings.showHttpReqResult){
				if(obj != null){
					Debug.Log("Requesting "+req+" data="+obj.ToString());
				}else{
					Debug.Log("Requesting "+req+" data=null");
				}
				
			}
				
			var res = InnerRequest(req,obj);
			//			Debug.Log(res.ToString());
			asyncData._err = res["err"].Value<FIErr>();
			asyncData._res = res;
			asyncData._isDone = true;
			if(settings.showHttpReqResult)
				Debug.Log("Result "+req+" err="+asyncData._err.ToString()+" res="+res.ToString());
			//			isProcessing = false;
		});
		return asyncData;
	}
	JObject InnerRequest(string req, JObject obj){
		string replaced = req.Replace("/","_");
		if(methodDic.ContainsKey(replaced) == false){
			var method = typeof(FIClientReqHandler).GetMethod(replaced);
			if(method == null){
				Debug.LogError("[ClientDataUpdator Request]There is no function called=" + req);
				return new JObject(new JProperty("err",FIErr.Client_HttpErr));
			}
			methodDic.Add(replaced,method);
		}
		var gotMethod = methodDic[replaced];
		var fakeContext = new FIFakeContext(){
			dbContext=srcContainer,
			body=obj??new JObject(),
			Items=new Dictionary<string,object>(),
			easy = this.easy,
			staticData=this.staticData
		};
		if(replaced.Contains("_sess_") == true){
			if(FIClientReqHandler.sess == null){
				return new JObject(new JProperty("err",FIErr.ExpiredSession));
			}else{
				fakeContext.Items["sess"] = FIClientReqHandler.sess;
			}
		}
		JObject res = null;
		try{
			res = (JObject)gotMethod.Invoke(null, new object[]{fakeContext});
		}catch(TargetInvocationException e){
			if(e.InnerException.GetType() == typeof(FIException)){
				var fiException = e.InnerException as FIException;
				res = new JObject( new JProperty("err",fiException.err) );
			}else{
				Debug.LogError("FIFakeServerDataProvider InnerException UnhandledException!");
				Debug.LogError(e.InnerException.Message);
				Debug.LogError(e.InnerException.StackTrace);
				res = new JObject( new JProperty("err",FIErr.InternalErr) );
			}
		}catch(System.Exception e){
			Debug.LogError("FIFakeServerDataProvider UnhandledException!");
			Debug.LogError(e.Message);
			Debug.LogError(e.StackTrace);
			res = new JObject( new JProperty("err",FIErr.InternalErr) );
		}
		if(res["err"].Value<FIErr>() == FIErr.Okay){
			ProcessResult(res);
		}
		if(settings.fakeSaveServerData)
			Save();
		return res;
	}





	string FilePath{
		get{
			return Application.persistentDataPath+"/"+settings.fakeServerFileName;
		}
	}
	//Testing Purpose!
	public void LoadServerData(){
		srcContainer = new FIRData();
		if(System.IO.File.Exists( FilePath ) == true){
			string jsonStr = System.IO.File.ReadAllText(FilePath);
			var jsonData = JsonConvert.DeserializeObject<JObject>(jsonStr);
			srcContainer.DeserializeFromJson(dataTypeContainer,jsonData);
			Debug.Log("[FIFakeServerDataProvider] Loaded");
		}
	}
	public void Save(){
		var jsonObject = srcContainer.SerializeToJson();
		System.IO.File.WriteAllText(FilePath,JsonConvert.SerializeObject(jsonObject,Formatting.Indented) );
		Debug.Log("[FIFakeServerDataProvider] Saved at "+FilePath);
	}
	public void FakeRequest(System.Func<FIRData,JObject> fakeAction){
		var res = fakeAction(srcContainer);
		ProcessResult(res);
	}
}
public class FICancelSource:ICancelable{
	bool isDisposed;
	public bool IsDisposed { get{return isDisposed;} }
	public void Dispose(){
		isDisposed = true;
	}
}

public interface IDataProviderRequestAsync{
	bool IsDone{get;}
	FIErr Err{get;}
	JObject Res{get;}
}
public class FIFakeContext{
	public GDManager staticData;
	public FIRData dbContext;
	public JObject body;
	public FIEasy easy;
	public Dictionary<string,object> Items;
}