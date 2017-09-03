using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

public partial class FIPopupGlobalTrade : CLSceneContext{
	[Inject]
	FIDefaultRequest server;
	[Inject]
	IRuntimeData runtimeData;
	[Inject]
	GDManager staticData;
	[Inject]
	FIPopupManager popupManager;
	[Inject]
	FISpriteData sprite;
	[Inject]
	FIEasy easy;

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);

		view = viewManager.CreateView("Popup/GlobalTrade","Popup");
		BindInstance();
		BindLogic();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}



	void BindInstance(){
		
	}
	void BindLogic(){
		
	}
}