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
	CLScrollView remainingAreaScrollView;
	void BindCraftingAreaInstance(){
		remainingAreaScrollView = new CLScrollView();
	}
	void BindCraftingAreaLogic(){
		remainingAreaScrollView.Init(view.CLGetGameObject("Window/CraftingArea/StackArea/Grid"),
			(idx,sample)=>{
				var comp = sample.AddComponent<RemainingArea>();
				container.InjectGameObject(comp.gameObject);
				comp.Init(runtimeTable,idx);
			},()=>{
				return staticTable.maxSlot;
			});
		remainingAreaScrollView.OnRefresh();
		Observable.EveryUpdate().Subscribe(_=>{
			var allDishes = runtimeData.GetList<DBCraftingItem>()
				.Where(x=>x.tableUID==runtimeTable.uid)
				.OrderBy(x=>x.insertedTime)
				.ToList();
			var remaining = FIShareCraftFunc.GetRemainingDishList(easy.GlobalInfo.totalCraftedCompleteCnt,easy.ServerTime-runtimeTable.recipeStartedTime,allDishes);
//			Debug.Log("Remaining="+remaining.Count);
		}).AddTo(this.go);

		var makingComp = view.CLGetGameObject("Window/CraftingArea/StackArea/CreatingArea").AddComponent<CurrentMakingArea>();
		container.Inject(makingComp);
		makingComp.Init(runtimeTable);
	}
	class CurrentMakingArea:MonoBehaviour{
		[Inject]
		FIEasy easy;
		[Inject]
		GDManager staticData;
		[Inject]
		IRuntimeData runtimeData;
		[Inject]
		FISpriteData sprite;

		enum State{
			Invalid = -1,
			Empty,
			Making,
			Unhold
		}


		DBCraftingTable runtimeTable;
		GDCraftingTable staticTable;
		CLStateUpdator<State> state;
		DBCraftingItem currentMaking;
		System.TimeSpan passedTime;
//		ReactiveProperty<DBCraftingItem> currentMaking = new ReactiveProperty<DBCraftingItem>();
		public void Init(DBCraftingTable _table){
			runtimeTable = _table;
			staticTable = staticData.GetByID<GDCraftingTable>(runtimeTable.craftingTableID);
			state = new CLStateUpdator<State>();
			state.Init(OnCheckState,OnSetState,OnUpdateState);

		}
		State OnCheckState(){
			passedTime = System.TimeSpan.Zero;
			bool isUnhold;
			var allDishes = runtimeData.GetList<DBCraftingItem>()
				.Where(x=>x.tableUID==runtimeTable.uid)
				.OrderBy(x=>x.insertedTime)
				.ToList();
			currentMaking = FIShareCraftFunc.GetCurrentCookingRecipe(
				easy.GlobalInfo.totalCraftedCompleteCnt,
				easy.ServerTime - runtimeTable.recipeStartedTime,
				allDishes,
				out passedTime,
				out isUnhold);
			if(currentMaking == null)
				return State.Empty;
			if(isUnhold == true){
				return State.Unhold;
			}
			return State.Making;
		}
		void OnSetState(State before, State after){
			if(before != State.Invalid){
				gameObject.CLGetGameObject(string.Format("State/{0}",before)).SetActive(false);
			}
			gameObject.CLGetGameObject(string.Format("State/{0}",after)).SetActive(true);
//			switch(after){
//			case State.Making:{
//					var item = staticData.GetByID<GDCraftRecipeData>(currentMaking.recipeID);
//					gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",after)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
//				}
//				break;
//			case State.Unhold:{
//					var item = staticData.GetByID<GDCraftRecipeData>(currentMaking.recipeID);
//					gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",after)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
//				}
//				break;
//			}
		}
		void OnUpdateState(State currState){
			switch(currState){
			case State.Unhold:{
					var item = staticData.GetByID<GDCraftRecipeData>(currentMaking.recipeID);
					gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",currState)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
				}
				break;
			case State.Making:{
					var item = staticData.GetByID<GDCraftRecipeData>(currentMaking.recipeID);
					gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",currState)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
					gameObject.FISetSlider("State/Making/Slider",passedTime,currentMaking.reqTime);
				}
				break;
			}
		}
		void Update(){
			state.Update();
		}
	}
	class RemainingArea:MonoBehaviour{
		[Inject]
		FIEasy easy;
		[Inject]
		GDManager staticData;
		[Inject]
		FISpriteData sprite;
		[Inject]
		IRuntimeData runtimeData;

		enum State{
			Invalid = -1,
			Empty,
			Waiting,
			Expand,
			CannotExpand,
		}


		DBCraftingTable runtimeTable; 
		int idx;
		GDCraftingTable staticTable;
		CLStateUpdator<State> state;
		List<DBCraftingItem> remainingList;
		DBCraftingItem currentItem;
		public void Init(DBCraftingTable _table,int _idx){
			runtimeTable = _table;
			idx = _idx;
			staticTable = staticData.GetByID<GDCraftingTable>(runtimeTable.craftingTableID);
			state = new CLStateUpdator<State>();
			state.Init(OnCheckState,OnSetState,OnUpdateState);

			gameObject.CLOnClickAsObservable("State/Expand").Subscribe(_=>{
				//Popup..
			});
		}
		State OnCheckState(){
			var upgradeCnt = staticTable.maxSlot-staticTable.minSlot;
			var curSlotCnt = runtimeTable.upgradeLv+staticTable.minSlot - 1; // -1 is for CurrentMakingArea
			var totalAvailableUpgradeCnt = upgradeCnt - runtimeTable.upgradeLv;
			if(idx == curSlotCnt){
				if(totalAvailableUpgradeCnt > 0){
					return State.Expand;
				}
				throw new System.Exception("Cannot Come here!");
			}else if(idx > curSlotCnt){
				return State.CannotExpand;
			}


			var allDishes = runtimeData.GetList<DBCraftingItem>()
				.Where(x=>x.tableUID==runtimeTable.uid)
				.OrderBy(x=>x.insertedTime)
				.ToList();
			remainingList = FIShareCraftFunc.GetRemainingDishList(
				easy.GlobalInfo.totalCraftedCompleteCnt,
				easy.ServerTime - runtimeTable.recipeStartedTime,
				allDishes
			);
//			Debug.Log("Idx="+idx.ToString()+" Remain="+remainingList.Count);
			if(idx < remainingList.Count){
				//This is making..
				currentItem = remainingList[idx];
				return State.Waiting;
			}
			currentItem = null;

			return State.Empty;
		}
		void OnSetState(State before, State after){
			if(before != State.Invalid){
				gameObject.CLGetGameObject(string.Format("State/{0}",before)).SetActive(false);
			}
			gameObject.CLGetGameObject(string.Format("State/{0}",after)).SetActive(true);
//			switch(after){
//			case State.Waiting:
//				var item = staticData.GetByID<GDCraftRecipeData>(remainingList[idx].recipeID);
//				gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",after)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
//				break;
//			case State.Expand:
//				gameObject.CLSetFormattedText("State/Expand/Text",staticTable.slotExpandReqGold);
//				break;
//			}
		}
		void OnUpdateState(State currState){
			switch(currState){
			case State.Waiting:
				var item = staticData.GetByID<GDCraftRecipeData>(currentItem.recipeID);
				gameObject.CLGetComponent<Image>(string.Format("State/{0}/Image",currState)).sprite = sprite.GetItem(item.rewardArr[0].item.imageName);
				break;
			case State.Expand:
				gameObject.CLSetFormattedText("State/Expand/Text",staticTable.slotExpandReqGold);
				break;
			}
		}
		void Update(){
			state.Update();
		}
	}
}