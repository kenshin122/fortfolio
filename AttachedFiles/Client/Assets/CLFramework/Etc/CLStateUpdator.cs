using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLStateUpdator<T>{
	T currentState;
	public T CurrentState{
		get{
			return currentState;
		}
	}
	System.Func<T> onCheckDelegate;
	System.Action<T,T> onStateInit;
	System.Action<T> onStateUpdate;
	public void Init(System.Func<T> _onCheckDelegate,System.Action<T,T> _onStateInit, System.Action<T> _onStateUpdate){
		currentState = (T)(object)-1;
		onCheckDelegate = _onCheckDelegate;
		onStateInit = _onStateInit;
		onStateUpdate = _onStateUpdate;
		Update();
	}
	public void Update(){
		T nowState = onCheckDelegate();
		if(!nowState.Equals(currentState)){
			onStateInit(currentState,nowState);
			currentState = nowState;
		}
		onStateUpdate(currentState);
	}
	public void Refresh(){
		onStateInit(currentState,currentState);
	}

}