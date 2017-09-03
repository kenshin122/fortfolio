using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CLContextAttrib("Test")]
public class TestFunc : CLCoroutineContext{

	IObservable<int> observable;
	public override void OnGameStartLoaded (System.Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
	}
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		observable = Observable.Create<int>(observer=>{
			var disposer = new Disposer();
			StartCoroutine(Counter(observer,disposer));
			return disposer;
		});
	}
	System.IDisposable disposable;
	protected override void OnTestKeyPressedUp (string key)
	{
//		base.OnTestKeyPressedUp (key);

		switch(key){
		case "w":
			disposable = observable.Subscribe(x=>{
				Debug.Log(x.ToString());
			},()=>{
				Debug.Log("Ended!");
			});
			break;
		case "g":
			disposable.Dispose();
			break;
		}
	}
	public class Disposer:System.IDisposable{
		public bool IsEnded;
		public void Dispose(){
			IsEnded = true;
		}
	}
	IEnumerator Counter(IObserver<int> observer, Disposer disposer){
		for(int i = 0 ; i < 100 ; i++){
			if(disposer.IsEnded == true){
				observer.OnCompleted();
				yield break;
			}
			observer.OnNext(i);
			yield return new WaitForSeconds(1.0f);
		}
	}
}
