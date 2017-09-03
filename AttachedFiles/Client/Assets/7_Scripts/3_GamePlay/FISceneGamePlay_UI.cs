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
	[Inject]
	FITransitionView transition;

	FITopInfo topInfo;
	void BindUIInstance(){
		view = viewManager.CreateView("Views/GamePlay","View");
		topInfo = cManager.CreateContext<FITopInfo>();
	}
	void DisposeUIInstance(){
		topInfo.Dispose();
	}
	void BindUILogic(){
		view.Visible = true;
		view.BlocksRaycast = true;
		transition.DefaultStart(false);
		transition.DefaultEnd(true);

		view.CLOnClickAsObservable("Bottom/Button_Shop").Subscribe(_=>{
			popupManager.ChangePopup<FIPopupShop>();
		});
		view.CLOnClickAsObservable("Bottom/Button_CookBook").Subscribe(_=>{

		});
		view.CLOnClickAsObservable("Bottom/Button_TownFarm").Subscribe(_=>{

		});
		view.CLOnClickAsObservable("Bottom/Button_Trade").Subscribe(_=>{

		});
		view.CLOnClickAsObservable("Bottom/Button_InternationalTrade").Subscribe(_=>{

		});
	}
}