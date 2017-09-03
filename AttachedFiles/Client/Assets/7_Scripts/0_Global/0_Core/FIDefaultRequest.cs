using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine.UI;

[CLContextAttrib("DefaultRequest")]
public class FIDefaultRequest:CLCoroutineContext{
	[Inject]
	IDataProvider dataProvider;
	[Inject]
	FILoadingView loadingView;
	[Inject]
	FIPopupManager popupManager;
	public override void Dispose ()
	{
		base.Dispose ();
	}
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
	}
	//-If timeout.. then fail!
	//-If err... then show err!
	public void Request(string req,JObject data = null,CancellationToken? token=null,System.Action<FIErr,JObject> onRes = null){
		loadingView.Use();
		dataProvider.Request(req,data,token,(err,res)=>{
			loadingView.Release();
			if(onRes != null)
				onRes(err,res);
		});
	}
	public IFIResultEnumerator GetCoroutine(string req, JObject data = null){
		loadingView.Use();
		FICancelSource source = new FICancelSource();
		CancellationToken token = new CancellationToken(source);
		FIDefaultRequestEnumerator reqEnumerator = new FIDefaultRequestEnumerator(token);
		dataProvider.Request(req,data,null,(err,res)=>{
			loadingView.Release();
			reqEnumerator._err = err;
			reqEnumerator._res = res;
			reqEnumerator._isDone = true;
			source.Dispose();
		});
		return reqEnumerator;
	}
	public IObservable<Tuple<FIErr,JObject>>Get(string req, JObject data=null){
		loadingView.Use();
		return dataProvider.Get(req,data).Select(x=>{
			loadingView.Release();
			return x;
		});
	}
	public IObservable<Tuple<FIErr,JObject>>GetWithErrHandling(string req, JObject data=null){
		return Observable.Create<Tuple<FIErr,JObject>>(observer=>{
			var cancelSource = new FICancelSource();
//			var token = new CancellationToken(cancelSource);
			StartCoroutine(GetWithErrHandlingProcess(req,data,observer));
			return cancelSource;
		});
	}
	IEnumerator GetWithErrHandlingProcess(string req, JObject data, IObserver<Tuple<FIErr,JObject>> observer){
		
		while(true){
			loadingView.Use();
			var async = dataProvider.RequestAsync(req,data,null);
			while(async.IsDone == false)
				yield return null;
			loadingView.Release();
			switch(async.Err){
//			case FIErr.Client_HttpErr:
//				//Retry..
//				break;
//			case FIErr.ExpiredSession:
//				//Goto start screen.
//				break;
			case FIErr.Okay:
				observer.OnNext(Tuple.Create<FIErr,JObject>(async.Err,async.Res));
				observer.OnCompleted();
				yield break;
//				break;
			default:
				var popup = popupManager.PushPopup<FIPopupDialog>();
				popup.SetErrorPopup(async.Err.ToString());
				observer.OnCompleted();
				yield break;
//				break;
			}
		}
	}


//	void ShowDialog(string data){
//		testDialog.Visible = true;
//		testDialog.BlocksRaycast = true;
//		testDialog.CLGetComponent<Text>("Text").text = data;
//	}



//	protected override void OnTestKeyPressedUp (string key)
//	{
//		switch(key){
//		case "w":
////			Debug.Log("Keypressed!");
//			Request("enc/auth/logindid",new JObject(new JProperty("deviceID","Hello")));
//			break;
//		}
//	}
	class FIDefaultRequestEnumerator:IFIResultEnumerator{
		CancellationToken token;
		public FIDefaultRequestEnumerator(CancellationToken token){
			this.token = token;
		}
		public FIErr _err;
		public FIErr Err{
			get{return _err;}
		}
		public JObject _res;
		public JObject Res{
			get{return _res;}
		}
		public bool _isDone;
		public bool IsDone{get{return _isDone;}}

		public bool MoveNext()
		{
			if(this.token.IsCancellationRequested == true)
				return false;
			return true;
		}
		public void Reset(){}
		object IEnumerator.Current{get{return null;}}
	}
}
public interface IFIResultEnumerator:IEnumerator{
	FIErr Err{get;}
	JObject Res{get;}
	bool IsDone{get;}
}