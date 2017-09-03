using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

[CLContextAttrib("Achievement")]
public class FIPopupAchievement : CLSceneContext{
	#region Test
	[Inject]
	FITestEasy testEasy;
	public override void OnGameStartLoaded (System.Action afterInit){
		testEasy.EasyTestStart(()=>{
			afterInit();
//			var topInfo = cManager.CreateContext<FITopInfo>();
		});
	}
	protected override void OnTestKeyPressedUp (string key)
	{
		base.OnTestKeyPressedUp (key);
		switch(key){
		case "w":
			testEasy.IncreaseTime();
			break;
		}
	}
	#endregion
	//Things im referencing.
	[Inject]
	FIEasy easy;
	[Inject]
	FISpriteData sprite;
	[Inject]
	GDManager staticData;
	[Inject]
	IRuntimeData runtimeData;
	[Inject]
	FIDefaultRequest server;
	[Inject]
	FIPopupManager popupManager;

	public override void OnInitialize (params object[] args){
		base.OnInitialize(args);
		//Get order from server..
		BindInstances();
		BindLogic();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}

	CLScrollView scrollView;
	List<GDAchievementData> dataList;
	//Inner bindings..
	void BindInstances(){
		view = viewManager.CreateView("Popup/Achievement","Popup");
		scrollView = new CLScrollView();
	}

	void BindLogic(){
		//Quit
		view.CLOnPointerClickAsObservable("Back").Select(x=>Unit.Default).Subscribe(_=>{
			popupManager.DestroyPopup(this);
		});

		scrollView.Init(view.CLGetGameObject("Window/ScrollView/Viewport/Content"),
			(idx,go)=>{
				var singleData = dataList[idx];
				go.CLSetFormattedText("Name",singleData.name);

				if(singleData.reqAchiev == GDAchievementType.Invalid){
					go.CLGetGameObject("Button_Reward").SetActive(false);
					go.CLGetGameObject("Slider").SetActive(false);
					go.CLSetFormattedText("Cond","완료됨");
					return;
				}

				var baseDesc = staticData.GetList<GDAchievementTypeDesc>().Where(x=>x.type==singleData.reqAchiev).FirstOrDefault().desc;
				go.CLSetFormattedText("Cond",string.Format(baseDesc,singleData.reqAchievCnt));


				int curCnt = 0;
				var curAchivCntData = runtimeData.GetList<DBAchievementTypeCount>().Where(x=>x.type==singleData.reqAchiev).FirstOrDefault();
				if(curAchivCntData != null)
					curCnt = curAchivCntData.cnt;

				go.CLSetFormattedText("Slider/Text",curCnt,singleData.reqAchievCnt);
				go.CLGetComponent<Slider>("Slider").value = (float)curCnt/(float)singleData.reqAchievCnt;

				if(curCnt>=singleData.reqAchievCnt){
					go.CLGetComponent<Button>("Button_Reward").interactable = true;
				}else{
					go.CLGetComponent<Button>("Button_Reward").interactable = false;
				}
				var diaItem = staticData.GetByID<GDItemData>(GDInstKey.ItemData_diaPoint);
				go.CLGetComponent<Image>("Button_Reward/Item/Image").sprite = sprite.GetItem( diaItem.imageName );
				go.CLSetFormattedText("Button_Reward/Item/Text",singleData.rewardDiaCnt);
				go.CLOnThrottleClickAsObservable("Button_Reward").Subscribe(_=>{
					server.Get("enc/sess/achievement/collect",JObject.FromObject(new{id=singleData.id}))
						.Subscribe(x=>{
//							popupManager.PushPopup<FIPopupDialog>("업적 보상 받음");
							var notice = popupManager.PushPopup<FIPopupDialog>();
							notice.SetNoticePopup("업적 보상 받음");
							OnRefresh();
						});
				});

//				runtimeData.GetObserver<DBAchievementTypeCount>()
//					.Where(x=>x.Item2.type==singleData.reqAchiev)
//					.Select(x=>x.Item2)
//					.Subscribe(x=>{
//						
//					}).AddTo(go);
			},()=>{
				return dataList.Count;
			});

		OnRefresh();
	}

	void OnRefresh(){
		dataList = new List<GDAchievementData>();
		foreach(var item in staticData.GetList<GDAchievementData>()){
			//Is this already cleared?. Then ignore!
			var cleared = runtimeData.GetList<DBAchievementCleared>().Where(x=>x.achievementID==item.id).FirstOrDefault();
			if(cleared != null)
				continue;

			//Is this achievement not met preceding?
			if(item.unlockReq != null){
				var preceding = runtimeData.GetList<DBAchievementCleared>().Where(x=>x.achievementID==item.unlockReq.id).FirstOrDefault();
				if(preceding == null)
					continue;
			}

			//This is it!
			dataList.Add( item );
		}
		scrollView.OnRefresh();

		view.CLSetFormattedText("Window/ClearedText",
			runtimeData.GetList<DBAchievementCleared>().Count,
			staticData.GetList<GDAchievementData>().Count
		);
	}
}
