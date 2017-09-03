using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using UniRx;
public class CLCoroutineContext:CLContextBase{
	protected CLCoroutineMono mono;
	protected GameObject go{
		get{
			return mono.gameObject;
		}
	}
	public override void OnInitialize(params object[] args){
		var gameObject = new GameObject(this.GetType().Name);
		mono = gameObject.AddComponent<CLCoroutineMono>();
		if(isTesting == true){
			mono.StartCoroutine(TestCall());
		}
		mono.StartCoroutine(OnUpdateCoroutine());
	}
	public override void Dispose(){
		GameObject.Destroy(mono.gameObject);
	}
	protected Coroutine StartCoroutine(IEnumerator _enumerator){
		return mono.StartCoroutine(_enumerator);
	}
	IEnumerator OnUpdateCoroutine(){
		while(true){
			OnUpdate();
			yield return null;
		}
	}
	protected virtual void OnUpdate(){

	}
	IEnumerator TestCall(){
		while(true){
			if( string.IsNullOrEmpty(Input.inputString) == false)
				OnTestKeyPressedUp(Input.inputString);
			yield return null;
		}
	}
	protected virtual void OnTestKeyPressedUp(string key){

	}
}
public class CLSceneContext : CLCoroutineContext {
	[Inject]
	protected CLViewManager viewManager;
	protected CLView view;
	public CLView View{
		get{ return view; }
	}
	public override void Dispose(){
		base.Dispose();
//		GameObject.Destroy(mono.gameObject);
		if(view != null)
			view.Dispose();
	}
	protected void LoadDefaultView(string location = null, string layer = null){
		string loc = location;
//		string curlayer = layer;
		if(loc == null){
			var attrib = (CLContextAttrib)Attribute.GetCustomAttribute(this.GetType(),typeof(CLContextAttrib));
			loc = attrib.Name;
			view  = viewManager.CreateView($"Views/{attrib.Name}",layer);
		}else{
			view = viewManager.CreateView(loc,layer);
		}
		view.BlocksRaycast = true;
		view.Visible = true;
	}
	protected void ChangeScene(string name,params object[] args){
		cManager.CreateContext(name,args);
		this.Dispose();
	}
}
public class CLCoroutineMono:MonoBehaviour{}
