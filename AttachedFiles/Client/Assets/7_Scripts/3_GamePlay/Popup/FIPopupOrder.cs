using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

[CLContextAttrib("OrderPopup")]
public class FIPopupOrder : CLSceneContext{
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
	FIPopupManager popupManager;
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

	enum State{
		Invalid = -1,
		Nothing,
		Selected,
		Selected_ButWaiting
	}

	//Inner bindings..
	CLScrollView orderScrollView;
	CLScrollView requestItemScrollView;
	CLScrollView rewardItemScrollView;
	List<DBOrder> orderList;
	List<DBOrderItem> orderReqItemList;
	List<Tuple<GDItemData,int>> orderRewardItemList;
	CLStateUpdator<State> state;

	Button orderConfirmBtn;
	Button orderCancelBtn;
//	int lastSelectedIdx = -1;
	ReactiveProperty<int> selectedIdx = new ReactiveProperty<int>(-1);
	void BindInstances(){
		view = viewManager.CreateView("Popup/Order","Popup");

		orderScrollView = new CLScrollView();
		requestItemScrollView = new CLScrollView();
		rewardItemScrollView = new CLScrollView();

		orderConfirmBtn = view.CLGetComponent<Button>("Window/Bottom/Button_OK");
		orderCancelBtn = view.CLGetComponent<Button>("Window/Bottom/Button_Cancel");

		state = new CLStateUpdator<State>();
	}

	void BindLogic(){
		

		//Quit
		var quitStream = view.CLOnPointerClickAsObservable("Back").Select(x=>Unit.Default);
		quitStream.Merge(view.CLOnClickAsObservable("Window/Button_Exit")).Subscribe(x=>{
			popupManager.DestroyPopup(this);
		});

		orderScrollView.Init(view.CLGetGameObject("Window/OrderWindow/Content"),
			(idx,go)=>{
				go.name = idx.ToString();
				var process = go.AddComponent<OrderProcess>();
				container.Inject(process);
				process.Init(orderList[idx]);
				go.CLOnClickAsObservable().Subscribe(x=>{
					
					if(selectedIdx.Value == idx){
						
						selectedIdx.Value = -1;
					}
					else{
						selectedIdx.Value = idx;
					}
				});
			},()=>{ return orderList.Count; });
		requestItemScrollView.Init(view.CLGetGameObject("Window/InfoWindow/State/Selected/RequireContent"),
			(idx,go)=>{
				int curCnt = easy.GetItemCnt( orderReqItemList[idx].itemID );
				int needCnt = orderReqItemList[idx].itemCnt;
				string color = "green";
				if(curCnt < needCnt){
					color = "red";
				}
				go.CLSetFormattedText("Value",curCnt,needCnt,color,"white");
				go.CLGetComponent<Image>("Image").sprite = sprite.GetItem( orderReqItemList[idx].GetItem(staticData).imageName );
			},()=>{ return orderReqItemList.Count; });
		rewardItemScrollView.Init(view.CLGetGameObject("Window/InfoWindow/State/Selected/RewardContent"),
			(idx,go)=>{
				go.CLSetFormattedText("Value",orderRewardItemList[idx].Item2);
				go.CLGetComponent<Image>("Image").sprite = sprite.GetItem( orderRewardItemList[idx].Item1.imageName );
			},()=>{ return orderRewardItemList.Count; });
		
		state = new CLStateUpdator<State>();
		state.Init(()=>{
			if(selectedIdx.Value == -1){
				return State.Nothing;
			}
			if( orderList[selectedIdx.Value].waitStartedTime + easy.GlobalInfo.orderRegenTime < easy.ServerTime){
				return State.Selected;
			}
			return State.Selected_ButWaiting;
		},
			(before,after)=>{
				if(before != State.Invalid){
					view.CLGetGameObject("Window/InfoWindow/State/"+before.ToString()).SetActive(false);
				}
				view.CLGetGameObject("Window/InfoWindow/State/"+after.ToString()).SetActive(true);
//				
				switch(after){
				case State.Selected:
					SetDataReqReward(selectedIdx.Value);
					orderConfirmBtn.interactable = true;
					orderCancelBtn.interactable = true;
					break;
				case State.Selected_ButWaiting:

					break;
				default:
					orderConfirmBtn.interactable = false;
					orderCancelBtn.interactable = false;
					break;
				}
			},
			(current)=>{
				switch(current){
				case State.Selected:
				case State.Selected_ButWaiting:

					break;
				case State.Nothing:

					break;
				}
			});
		Observable.EveryGameObjectUpdate().Subscribe(x=>{
			state.Update();
		}).AddTo(go);
		int beforeSelected = -1;
		selectedIdx.Subscribe(x=>{
			if( beforeSelected != -1 )
				view.CLGetGameObject($"Window/OrderWindow/Content/{beforeSelected}/Selected").SetActive(false);
			if( x != -1 ){
				view.CLGetGameObject($"Window/OrderWindow/Content/{x}/Selected").SetActive(true);
				SetDataReqReward(x);
			}
			beforeSelected = x;
		});


		orderConfirmBtn.onClick.AsObservable().Subscribe(unit=>{
			if(selectedIdx.Value == -1){
				throw new System.NotImplementedException("Need popup");
			}

			if(orderList[selectedIdx.Value].waitStartedTime + easy.GlobalInfo.orderRegenTime > easy.ServerTime){
				throw new System.NotImplementedException("Need popup");
			}

			//Check can dispose item..
			var listOfReqItem = runtimeData.GetList<DBOrderItem>()
				.Where(x=>x.orderUID==orderList[selectedIdx.Value].uid)
				.Select(x=>Tuple.Create<int,int>(x.itemID,x.itemCnt))
				.ToArray();
			if( easy.CanDisposeItems(listOfReqItem) == false){
				throw new System.NotImplementedException("Need popup");
			}

			server.Get("enc/sess/order/accept",
				JObject.FromObject(new{uid=orderList[selectedIdx.Value].uid})
			).Subscribe(x=>{
				selectedIdx.Value = -1;
			});
		});
		orderCancelBtn.onClick.AsObservable().Subscribe(unit=>{
			if(selectedIdx.Value == -1){
				throw new System.NotImplementedException("Need popup");
			}

			if(orderList[selectedIdx.Value].waitStartedTime + easy.GlobalInfo.orderRegenTime > easy.ServerTime){
				throw new System.NotImplementedException("Need popup");
			}
			server.Get("enc/sess/order/dispose",
				JObject.FromObject(new{uid=orderList[selectedIdx.Value].uid})
			).Subscribe(x=>{
				selectedIdx.Value = -1;
			});
		});

		OnRefresh();
	}

	void OnRefresh(){
		orderList = (from item in runtimeData.GetList<DBOrder>()
			orderby item.waitStartedTime ascending
			select item).ToList();
		orderScrollView.OnRefresh();
	}
	void SetDataReqReward(int idx){
		orderReqItemList = runtimeData.GetList<DBOrderItem>().Where(x=>x.orderUID==orderList[idx].uid).ToList();


		int totalGold = 0;
		int totalExp = 0;
		foreach(var reqItemSingle in orderReqItemList){
			totalGold += reqItemSingle.CalcOrderPrice(staticData);
			totalExp += reqItemSingle.CalcOrderRewardExp(staticData);
		}

//		if(GDRFunc.IsMagicActive("customer") == true){
//			float ratio = (float)GDManager.GetByKey<GDManageMagicData>("customer").value / 100.0f;
//			totalGold = (int)((float)totalGold * ratio);
//		}


		orderRewardItemList = new List<Tuple<GDItemData, int>>(){
			Tuple.Create<GDItemData,int>( staticData.GetByID<GDItemData>(GDInstKey.ItemData_goldPoint),totalGold),
			Tuple.Create<GDItemData,int>( staticData.GetByID<GDItemData>(GDInstKey.ItemData_userExp),totalExp)
		};

		requestItemScrollView.OnRefresh();
		rewardItemScrollView.OnRefresh();
//		Debug.Log("Clicked="+idx);


		var reqArr = orderReqItemList.Select(x=>{
			return Tuple.Create<int,int>(x.itemID,x.itemCnt);
		}).ToArray();
		if(easy.CanDisposeItems(reqArr) == false){
			orderConfirmBtn.interactable = false;
		}else{
			orderConfirmBtn.interactable = true;
		}

	}


	class OrderProcess:MonoBehaviour{
		[Inject]

		FIEasy easy;

		enum State{
			Invalid,
			Normal,
			Waiting
		}
		DBOrder order;
		CLStateUpdator<State> state;
		public void Init(DBOrder _order){
			order = _order;
			state = new CLStateUpdator<State>();
			state.Init(
				OnCheckState,
				OnChangeState,
				OnUpdateState
			);
		}
		State OnCheckState(){
			//Check is waiting..
			if( order.waitStartedTime + easy.GlobalInfo.orderRegenTime < easy.ServerTime )
				return State.Normal;
			return State.Waiting;
		}
		void OnChangeState(State before, State after){
			switch(before){
			case State.Normal:
				gameObject.CLGetGameObject("Image").SetActive(false);
				break;
			case State.Waiting:
				gameObject.CLGetGameObject("Progress").SetActive(false);
				gameObject.CLGetGameObject("Clock").SetActive(false);
				break;
			}
			switch(after){
			case State.Normal:
				gameObject.CLGetGameObject("Image").SetActive(true);
				break;
			case State.Waiting:
				gameObject.CLGetGameObject("Progress").SetActive(true);
				gameObject.CLGetGameObject("Clock").SetActive(true);
				break;
			}
		}
		void OnUpdateState(State curState){
			switch(curState){
			case State.Waiting:{
					float ratio = easy.CalcRemainingRatio( order.waitStartedTime, easy.GlobalInfo.orderRegenTime);
					ratio = 1.0f - ratio;
					ratio = Mathf.Clamp(ratio,0.0f,1.0f);
					gameObject.CLGetComponent<Image>("Progress").fillAmount = ratio;
				}
				break;
			}
		}
		void Update(){
			state.Update();
		}
	}
}
