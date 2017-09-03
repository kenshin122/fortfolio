using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

//[CLContextAttrib("StoragePopup")]
public class FIPopupCustomer : CLSceneContext{
	[Inject]
	IRuntimeData runtimeData;
	[Inject]
	GDManager staticData;
	[Inject]
	FIEasy easy;
	[Inject]
	FIDefaultRequest server;
	[Inject]
	FISpriteData sprite;
	[Inject]
	FIPopupManager popupManager;

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		customerData = (DBCustomer)args[0];
		BindInstance();
		BindLogic();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}

	DBCustomer customerData;
	CLScrollView requestScrollView;
	CLScrollView rewardScrollView;
	List<Tuple<int,int>> reqList;
	List<Tuple<int,int>> rewardList;
//	int totalGold;
//	int totalExp;
	void BindInstance(){
		view = viewManager.CreateView("Popup/Customer","Popup");
		requestScrollView = new CLScrollView();
		rewardScrollView = new CLScrollView();
		reqList = new List<Tuple<int, int>>();
		rewardList = new List<Tuple<int, int>>();
	}
	void BindLogic(){

		view.CLOnClickAsObservable("Window/Button_Wait").Subscribe(_=>{
//			popupManager.PopPopup();
			popupManager.DestroyPopup(this);
		});
		view.CLOnThrottleClickAsObservable("Window/Button_GoAway").Subscribe(_=>{
			server.GetWithErrHandling("enc/sess/customer/dispose",JObject.FromObject(new{uid=customerData.uid}))
				.Subscribe(x=>{
//					this.Dispose();
					popupManager.DestroyPopup(this);
				});
		});
		view.CLOnThrottleClickAsObservable("Window/Bottom/Button_Sell").Subscribe(_=>{
			if(easy.CanDisposeItems(reqList.ToArray()) == false){
				var notice = popupManager.PushPopup<FIPopupDialog>();
				notice.SetNoticePopup("재료가 부족합니다");
				return;
			}
			server.GetWithErrHandling("enc/sess/customer/accept",JObject.FromObject(new{uid=customerData.uid}))
				.Subscribe(x=>{
					popupManager.DestroyPopup(this);
				});
		});
		view.CLOnClickAsObservable("Window/Bottom/Button_Content").Subscribe(_=>{

		});

		var customerStaticData = staticData.GetByID<GDCustomerData>(customerData.customerID);
		view.CLSetFormattedText("Window/Name",customerStaticData.name);
		view.CLSetFormattedText("Window/MessageBox/Text",customerStaticData.speechArr[0]);
		view.CLGetComponent<Image>("Window/People").sprite = sprite.GetPeople(customerStaticData.image);


		reqList.Add( Tuple.Create<int,int>(customerData.itemID,customerData.itemCnt) );

		int totalGold = 0;
		int totalExp = 3;
		foreach(var item in reqList){
			var itemData = staticData.GetByID<GDItemData>(item.Item1);
//			int curItemCnt = easy.GetItemCnt( item.Item1 );
			totalGold += itemData.GetCustomerPrice(customerData.itemCnt);
		}
		rewardList.Add( Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint, totalGold) );
		rewardList.Add( Tuple.Create<int,int>(GDInstKey.ItemData_userExp, totalExp) );

		requestScrollView.Init(view.CLGetGameObject("Window/ItemInfo/RequestGrid"),
			(idx,go)=>{
				var itemData = staticData.GetByID<GDItemData>( reqList[idx].Item1 );
				int curCnt = easy.GetItemCnt( reqList[idx].Item1 );
				go.CLGetComponent<Image>("Item/Image").sprite = sprite.GetItem(itemData.imageName);
				go.CLSetFormattedText("Cnt",curCnt,reqList[idx].Item2);
			},()=>reqList.Count
		);
		rewardScrollView.Init(view.CLGetGameObject("Window/ItemInfo/RewardGrid"),
			(idx,go)=>{
				var itemData = staticData.GetByID<GDItemData>( rewardList[idx].Item1 );
				go.CLGetComponent<Image>("Item/Image").sprite = sprite.GetItem(itemData.imageName);
				go.CLSetFormattedText("Cnt",rewardList[idx].Item2);
			},()=>rewardList.Count
		);

		requestScrollView.OnRefresh();
		rewardScrollView.OnRefresh();

		if(easy.CanDisposeItems(reqList.ToArray()) == false){
			view.CLGetComponent<Button>("Window/Bottom/Button_Sell").interactable = false;
			//				throw new System.NotImplementedException("Need lack item popup!");
//			popupManager.PushPopup<FIPopupDialog>("Not enough item!");
			return;
		}
	}

}