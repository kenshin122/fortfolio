using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using DG.Tweening;
[CLContextAttrib("PopupManager")]
public class FIPopupManager : CLCoroutineContext {
	#region Test
	public override void OnGameStartLoaded (Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
		Debug.Log("called!");
	}
	int counter = 0;
	protected override void OnTestKeyPressedUp (string key)
	{
		Debug.Log("KeyPressed=!"+key);
		base.OnTestKeyPressedUp (key);
		switch(key){
		case "w":
			PushPopup<FITestPopup>("Push="+counter.ToString());
			break;
		case "s":
			PopPopup();
			break;
		case "a":
			ChangePopup<FITestPopup>("Changed="+counter.ToString());
			break;
		case "d":
			AlterPopup<FITestPopup>("Altered="+counter.ToString());
			break;
		}
		counter++;
	}
	#endregion

	[Inject]
	readonly CLViewManager viewManager;
	[Inject]
	readonly FITransitionView transition;

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}
	void DoStartAnim(CLView view,System.Action onEndAction=null){
		view.BlocksRaycast = true;
		var temp = view.CGroup.DOFade(1,0.1f);
		if(onEndAction != null)
			temp.OnComplete(()=>{
				onEndAction();
			});
		view.Trans.localScale = Vector3.one * 0.9f;
		view.Trans.DOScale(Vector3.one,0.2f);
	}
	void DoEndAnim(CLView view,System.Action onEndAction=null){
		view.BlocksRaycast = false;
		var temp = view.CGroup.DOFade(0,0.1f);
		if(onEndAction != null)
			temp.OnComplete(()=>{
				onEndAction();
			});
		view.Trans.localScale = Vector3.one;
		view.Trans.DOScale(Vector3.one * 0.9f,0.2f);
	}
//	Stack<CLSceneContext> stack = new Stack<CLSceneContext>();
	List<CLSceneContext> stackList = new List<CLSceneContext>();
	CLSceneContext Peek(){
		return stackList[stackList.Count-1];
	}
	CLSceneContext Pop(){
		if(stackList.Count<=0)
			throw new Exception("Not enough count stackList");
		var temp = stackList[stackList.Count - 1];
		stackList.RemoveAt(stackList.Count - 1);
		return temp;
	}
	void Push(CLSceneContext temp){
		stackList.Add(temp);
	}
	public T PushPopup<T>(params object[] args)where T:CLSceneContext,new(){
		if(stackList.Count > 0){
//			var stack.Peek();
			var top = Peek();
			DoEndAnim(top.View);
//			top.View.CGroup.DOFade(0,0.2f);
//			top.View.BlocksRaycast=false;
		}else{
			transition.DefaultBelowStart();
		}
		var newPopup = cManager.CreateContext<T>(args);
		DoStartAnim(newPopup.View);
//		newPopup.View.CGroup.DOFade(1,0.2f);
//		newPopup.View.BlocksRaycast=true;
		Push(newPopup);
		return newPopup;
	}
	public void PopPopup(){
		if(stackList.Count <= 0)
			throw new Exception("There is no thing to pop");
		var top = Pop();
		DoEndAnim(top.View,()=>{
			top.Dispose();
		});
//		top.View.BlocksRaycast=false;
//		top.View.CGroup.DOFade(0,0.2f).OnComplete(()=>{
//			top.Dispose();
//		});
		if(stackList.Count <= 0){
			transition.DefaultBelowEnd();
		}else{
			var popped = Peek();
			DoStartAnim(popped.View);
//			popped.View.BlocksRaycast = true;
//			popped.View.CGroup.DOFade(1,0.2f);
		}
	}
	public T ChangePopup<T>(params object[] args)where T:CLSceneContext,new(){
		if(stackList.Count <= 0){
			transition.DefaultBelowStart();
		}else{
			var top = Pop();
			DoEndAnim(top.View,()=>{
				top.Dispose();
			});
//			top.View.CGroup.DOFade(0,0.2f).OnComplete(()=>{
//				top.Dispose();
//			});
//			top.View.BlocksRaycast = false;

			while(stackList.Count > 0){
				var temp = Pop();
				temp.Dispose();
			}
		}
		var newPopup = cManager.CreateContext<T>(args);
		DoStartAnim(newPopup.View);
//		newPopup.View.CGroup.DOFade(1,0.2f);
//		newPopup.View.BlocksRaycast=true;
		Push(newPopup);
		return newPopup;
	}
	public T AlterPopup<T>(params object[] args)where T:CLSceneContext,new(){
		if(stackList.Count > 0){
			var top = Pop();
			DoEndAnim(top.View,()=>{
				top.Dispose();
			});
//			top.View.CGroup.DOFade(0,0.2f).OnComplete(()=>{
//				top.Dispose();
//			});
//			top.View.BlocksRaycast = false;
		}else{
			transition.DefaultBelowStart();
		}
		var newPopup = cManager.CreateContext<T>(args);
		DoStartAnim(newPopup.View);
//		newPopup.View.CGroup.DOFade(1,0.2f);
//		newPopup.View.BlocksRaycast=true;
		Push(newPopup);
		return newPopup;
	}
	public void DestroyPopup(CLSceneContext single){
		//Check exists..
		int idx = -1;
		for(int i = 0 ; i < stackList.Count ; i++){
			if(stackList[i] == single){
				idx = i;
				break;
			}
		}
		if(idx == -1){
			throw new Exception("Cannot dispose which not exists!");
		}
		if(stackList.Count-1 == idx){
			//Need this to be disposed normally.
			PopPopup();
		}else{
			//Might be middle of popup..
			stackList[idx].Dispose();
			stackList.RemoveAt(idx);
		}
	}
}
