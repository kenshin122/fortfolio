using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using Zenject;
using Newtonsoft.Json.Linq;

public class FITransitionView : CLSceneContext {
	[Inject]
	FIBuildOptions settings;
	CLView belowView;
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		LoadDefaultView("Etc/Transition","TopTop");
		view.Visible = false;
		view.BlocksRaycast = false;
		belowView = viewManager.CreateView("Etc/Transition","BelowPopup");
		belowView.Visible = false;
		belowView.BlocksRaycast = false;
//		view.CGroup.alpha = 0.0f;
	}
	public override void Dispose ()
	{
		base.Dispose ();
		belowView.Dispose();
	}
	public void DefaultBelowStart(bool isAnimate = true){
		belowView.BlocksRaycast = true;
		if(isAnimate == true)
			belowView.CGroup.DOFade(0.8f,0.2f);
		else
			belowView.CGroup.alpha = 0.8f;
	}
	public void DefaultBelowEnd(bool isAnimate = true){
		if(isAnimate == true)
			belowView.CGroup.DOFade(0.0f,0.2f).OnComplete(()=>{
				belowView.BlocksRaycast = false;
			});
		else{
			belowView.CGroup.alpha = 0.8f;
			belowView.BlocksRaycast = false;
		}
	}
	public void Default(System.Action onMiddleOfTransitDelegate){
		StartCoroutine(TransitProcess(onMiddleOfTransitDelegate));
	}
	public void DefaultStart(bool isAnimate = true){
		view.BlocksRaycast = true;
		if(isAnimate == true)
			view.CGroup.DOFade(1,0.2f);
		else
			view.CGroup.alpha = 1.0f;
	}
	public void DefaultEnd(bool isAnimate = true){
		view.BlocksRaycast = false;
		view.CGroup.DOFade(0,0.2f);
		if(isAnimate == true)
			view.CGroup.DOFade(0,0.2f);
		else
			view.CGroup.alpha = 0.0f;
	}
	IEnumerator TransitProcess(System.Action onMiddleOfTransitDelegate){
		DefaultStart();
		yield return new WaitForSeconds(0.2f);
		if(onMiddleOfTransitDelegate != null)
			onMiddleOfTransitDelegate();
		DefaultEnd();
	}
}
