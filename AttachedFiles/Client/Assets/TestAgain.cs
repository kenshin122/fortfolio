using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CLContextAttrib("Test2")]
public class TestAgain : CLCoroutineContext{
	
	public override void OnGameStartLoaded (System.Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
	}
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
	}
	protected override void OnTestKeyPressedUp (string key)
	{
		switch(key){
		case "w":
			StartCoroutine(SomeThing());
			break;
		}
	}
	IEnumerator SomeThing(){
		var temp = new Some();
		Debug.Log("Stated!"+Time.realtimeSinceStartup.ToString());
		yield return temp;
		Debug.Log("Ended!"+Time.realtimeSinceStartup.ToString());
	}
	public class Some:IEnumerator{
		int counter  = 3;
//		public Some(){
//			
//		}
		public void Reset(){}
		public bool MoveNext()
		{
			Debug.Log("MoveNext!"+counter.ToString()+" "+Time.realtimeSinceStartup.ToString());
			if(counter > 0){
				counter--;
				return true;
			}
			return false;
		}



		object IEnumerator.Current
		{
			get
			{
				Debug.Log("Current!! "+Time.realtimeSinceStartup.ToString());
				return null;
			}
		}
//		IEnumerator IEnumerable.GetEnumerator(){
//			return new SomeEnum();
//		}

//		class SomeEnum:IEnumerator{
//			CancellationToken token;
//			public SomeEnum(CancellationToken token){
//				this.token = token;
//			}
//
//		}
	}
}
