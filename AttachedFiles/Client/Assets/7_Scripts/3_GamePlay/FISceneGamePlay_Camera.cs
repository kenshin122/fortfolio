using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public partial class FISceneGamePlay : CLSceneContext{
	enum CameraRegion{
		Left,
		Middle,
		Right,
	}
	Transform camera;
	CanvasGroup cameraControl;
	ReactiveProperty<CameraRegion> currentCameraRegion = new ReactiveProperty<CameraRegion>(CameraRegion.Left);
	void BindCameraInstance(){
		camera = gameWorld.CLGetComponent<Transform>("WorldCamera");
		cameraControl = gameWorld.CLGetComponent<CanvasGroup>("Lobby");
	}
	void DisposeCameraInstance(){

	}
	void BindCameraLogic(){
		//Direction control
		view.CLOnClickAsObservable("Bottom/Button_Left").Subscribe(_=>{
			//			camera.
			cameraControl.blocksRaycasts = false;
			camera.DOLocalMoveX(0.0f,1.0f).SetEase(Ease.OutQuad).OnComplete(()=>{
				cameraControl.blocksRaycasts = true;
			});
		});
		view.CLOnClickAsObservable("Bottom/Button_Right").Subscribe(_=>{
			cameraControl.blocksRaycasts = false;
			camera.DOLocalMoveX(10.8f,1.0f).SetEase(Ease.OutQuad).OnComplete(()=>{
				cameraControl.blocksRaycasts = true;
			});
		});
		view.CLUpdateAsObservable().Subscribe(_=>{
			//			camera.position
			if(camera.position.x < 1.5f){
				currentCameraRegion.Value = CameraRegion.Left;
			}else if(camera.position.x > 9.0f){
				currentCameraRegion.Value = CameraRegion.Right;
			}else{
				currentCameraRegion.Value = CameraRegion.Middle;
			}
		});
		currentCameraRegion.Subscribe(x=>{
			switch(x){
			case CameraRegion.Left:
				view.CLGetGameObject("Bottom/Button_Left").SetActive(false);
				view.CLGetGameObject("Bottom/Button_Right").SetActive(true);
				break;
			case CameraRegion.Middle:
				view.CLGetGameObject("Bottom/Button_Left").SetActive(true);
				view.CLGetGameObject("Bottom/Button_Right").SetActive(true);
				break;
			case CameraRegion.Right:
				view.CLGetGameObject("Bottom/Button_Left").SetActive(true);
				view.CLGetGameObject("Bottom/Button_Right").SetActive(false);
				break;
			}
		});
	}
}
