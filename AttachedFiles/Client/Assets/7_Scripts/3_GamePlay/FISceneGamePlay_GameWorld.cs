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
	GameObject gameWorld;

	void BindGameWorldInstance(){
		var res = Resources.Load<GameObject>("GameWorld/GameWorld");
		gameWorld = GameObject.Instantiate<GameObject>(res);
	}
	void DisposeGameWorldInstance(){
		GameObject.Destroy(gameWorld);
	}
	void BindGameWorldLogic(){
		gameWorld.CLOnClickAsObservable("Lobby/Components/Storage/Button").Subscribe(x=>{
//			cManager.CreateContext<FIPopupStorage>();
			popupManager.PushPopup<FIPopupStorage>();
		});
		gameWorld.CLOnClickAsObservable("Lobby/Components/Achievement/Button").Subscribe(x=>{
//			cManager.CreateContext<FIPopupAchievement>();
			popupManager.PushPopup<FIPopupAchievement>();
		});
		gameWorld.CLOnClickAsObservable("Lobby/Components/Quest/Button").Subscribe(x=>{
//			cManager.CreateContext<FIPopupAchievement>();
		});
		gameWorld.CLOnClickAsObservable("Lobby/Components/MagicBook/Button").Subscribe(x=>{
//			cManager.CreateContext<FIPopupAchievement>();
		});
		gameWorld.CLOnClickAsObservable("Lobby/Components/Order/Button").Subscribe(x=>{
//			cManager.CreateContext<FIPopupOrder>();
			popupManager.PushPopup<FIPopupOrder>();
		});
	}


}