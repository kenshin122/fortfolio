using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

public partial class FIPopupCraft : CLSceneContext{
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

	DBCraftingTable runtimeTable;
	GDCraftingTable staticTable;
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		runtimeTable = args[0] as DBCraftingTable;
		staticTable = staticData.GetByID<GDCraftingTable>( runtimeTable.craftingTableID );

		view = viewManager.CreateView("Popup/Craft","Popup");
		BindInstance();
		BindLogic();
	}
	public override void Dispose ()
	{
		base.Dispose ();
		DisposeCategory();
	}



	void BindInstance(){
//		scrollView = new CLScrollView();
		var backObserv = view.CLOnPointerClickAsObservable("Back").Select(x=>Unit.Default);
		view.CLOnClickAsObservable("Window/CraftingArea/Button_Exit").Merge(backObserv)
			.Subscribe(_=>{
				popupManager.DestroyPopup(this);
			});
		view.CLOnClickAsObservable("Window/CraftingArea/Button_Sell").Subscribe(_=>{
			if(runtimeData.GetList<DBCraftingTable>().Count <= 1){
				popupManager.PushPopup<FIPopupDialog>().SetErrorPopup("하나남은 작업댄 팔수없습니다");
				return;
			}
			var currMakingList = runtimeData.GetList<DBCraftingItem>().Where(x=>x.tableUID==runtimeTable.uid).ToList();
			if(currMakingList.Count > 0){
				popupManager.PushPopup<FIPopupDialog>().SetErrorPopup("만들고 있는 아이템이 있습니다");
				return;
			}

			//Do Selling..
			var sellPopup = popupManager.PushPopup<FIPopupDialog>();
			sellPopup.SetChoosePopup(
				"알림",
				string.Format("item_goldPoint{0}에 작업대를 파시겠습니까?",staticTable.sellPrice),
				"확인",
				"취소",
				currPopup=>{
					server.GetWithErrHandling("enc/sess/craft/selltable",JObject.FromObject(new{uid=runtimeTable.uid}))
						.Subscribe(x=>{
							sellPopup.DestroyPopup();
						});	
				});
		});
		BindCategoryInstance();
		BindCraftingAreaInstance();
		BindCompleteSlotInstance();
		BindRecipeInstance();
	}
	void BindLogic(){
		BindCategoryLogic();
		BindCraftingAreaLogic();
		BindCompleteSlotLogic();
		BindRecipeLogic();
	}
}