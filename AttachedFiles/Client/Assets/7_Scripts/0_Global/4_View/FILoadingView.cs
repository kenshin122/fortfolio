using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using Zenject;
using Newtonsoft.Json.Linq;

[CLContextAttrib("LoadingView")]
public class FILoadingView : CLSceneContext {
	[Inject]
	FIBuildOptions settings;
	protected override void OnTestKeyPressedUp (string key)
	{
		base.OnTestKeyPressedUp (key);
		switch(key){
		case "w":
			Use();
			break;
		case "s":
			Release();
			break;
		}
	}
	public override void OnGameStartLoaded (System.Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
	}
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		LoadDefaultView("Etc/Loading","TopTop");
		view.Visible = false;
		view.BlocksRaycast = false;
		view.CGroup.alpha = 0.0f;
		view.Trans.SetAsFirstSibling();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}
	bool visible;
	bool Visible{
		get{
			return visible;
		}set{
			if(value == true){
				view.CLGetComponent<CanvasGroup>().DOFade(1,0.2f);
			}else{
				view.CLGetComponent<CanvasGroup>().DOFade(0,0.2f);
			}
			visible = value;
		}
	}
	int totalCnt;
	System.DateTime startedTime;
	public void Use(){
		if( totalCnt == 0 ){
			View.BlocksRaycast = true;
			startedTime = System.DateTime.Now;
		}
		totalCnt++;
	}
	public void Release(){
		if(totalCnt <= 0){
			throw new System.Exception("[FILoadingView Release] Somewhere releasing it again!");
		}
		
		if( totalCnt == 1){
			if(Visible == true)
				Visible = false;
			View.BlocksRaycast = false;
		}
		totalCnt--;
	}

	protected override void OnUpdate ()
	{
		if(totalCnt <= 0)
			return;

		if( (System.DateTime.Now - startedTime).TotalSeconds > settings.loadingThreshold 
			&& Visible == false){
			Visible = true;
		}
	}
}
