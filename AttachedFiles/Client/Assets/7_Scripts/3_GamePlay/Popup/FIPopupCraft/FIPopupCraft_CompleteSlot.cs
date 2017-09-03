using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

public partial class FIPopupCraft : CLSceneContext{
	CLScrollView completeScrollView;
	void BindCompleteSlotInstance(){
		completeScrollView = new CLScrollView();
	}
	void BindCompleteSlotLogic(){
		completeScrollView.Init(view.CLGetGameObject("Window/CompleteSlot/Grid"),
			(idx,sample)=>{
				var comp = sample.AddComponent<CompleteInfo>();
				container.Inject(comp);
				comp.Init(idx,runtimeTable);
			},()=>{
				Debug.Log("Cnt="+easy.GlobalInfo.totalCraftedCompleteCnt.ToString());
				return easy.GlobalInfo.totalCraftedCompleteCnt;
			});
		completeScrollView.OnRefresh();
	}
	class CompleteInfo:MonoBehaviour,IPointerEnterHandler,IBeginDragHandler,IEndDragHandler,IDragHandler{
		[Inject]
		readonly IRuntimeData runtimeData;
		[Inject]
		readonly GDManager staticData;
		[Inject]
		readonly FIEasy easy;
		[Inject]
		readonly FISpriteData sprite;
		[Inject]
		readonly FIDefaultRequest server;


		enum State{
			Invalid = -1,
			Empty,
			Made,
		}
		static Dictionary<int,CompleteInfo> collectDic;
		DBCraftingTable runtimeTable;
		int idx;
		CLStateUpdator<State> state;
		List<DBCraftingItem> completeList;
		// DBCraftingItem currentItem;
		ReactiveProperty<DBCraftingItem> currentItem = new ReactiveProperty<DBCraftingItem>();
		ReactiveProperty<bool> selected = new ReactiveProperty<bool>(false);
		public void Init(int _idx,DBCraftingTable _runtimeTable){
			idx = _idx;
			runtimeTable = _runtimeTable;
			state = new CLStateUpdator<State>();
			state.Init(OnCheckState,OnSetState,OnUpdateState);
			selected.Subscribe(isOn=>{
				if(isOn == true){
					gameObject.CLGetComponent<RectTransform>("Image").DOScale(Vector3.one*2.0f,0.2f);
				}else{
					gameObject.CLGetComponent<RectTransform>("Image").DOScale(Vector3.one,0.2f);
				}
			});
			currentItem.Subscribe(item=>{
				if(item == null){
					gameObject.CLGetComponent<Image>("Image").sprite = null;
				}else{
					var staticItem = staticData.GetByID<GDCraftRecipeData>( item.recipeID ).rewardArr[0].item;
					gameObject.CLGetComponent<Image>("Image").sprite = sprite.GetItem(staticItem.imageName);
				}
			});
		}
		public void ResetSelected(){
			selected.Value = false;
		}
		public void OnPointerEnter(PointerEventData eventData){
			if(collectDic == null)
				return;
			if(currentItem.Value == null)
				return;
			if(collectDic.ContainsKey(currentItem.Value.uid) == true)
				return;
//			Debug.Log("Collecting="+
			collectDic.Add(currentItem.Value.uid,this);
			selected.Value = true;
//			Debug.Log("Enter!="+idx.ToString());
		}
		bool isDragging = false;
		public void OnBeginDrag(PointerEventData eventData){
			if(currentItem.Value == null)
				return;
			collectDic = new Dictionary<int, CompleteInfo>();
			collectDic.Add(currentItem.Value.uid,this);
			selected.Value = true;

//			Debug.Log("DragBegin="+idx.ToString());
		}
		public void OnEndDrag(PointerEventData eventData){
			if(collectDic == null)
				return;
			var toData = collectDic.Select(x=>x.Key).ToArray();
			var dataObj = JObject.FromObject(new{uidArr=toData});
			server.GetWithErrHandling("enc/sess/craft/collect",dataObj)
				.Subscribe(_=>{
					foreach(var item in collectDic){
						item.Value.ResetSelected();
					}
					collectDic = null;
				});

//			foreach(var item in collectDic){
//				item.Value.ResetSelected();
//			}
//			collectDic = null;
		}
		public void OnDrag(PointerEventData eventData){
//			collectDic = null;
//			Debug.Log("Dragging!="+idx.ToString());
		}

		State OnCheckState(){
			var allDishes = runtimeData.GetList<DBCraftingItem>()
				.Where(x=>x.tableUID==runtimeTable.uid)
				.OrderBy(x=>x.insertedTime)
				.ToList();
			completeList = FIShareCraftFunc.GetMadeDishList(
				easy.GlobalInfo.totalCraftedCompleteCnt,
				easy.ServerTime - runtimeTable.recipeStartedTime,
				allDishes);
			if(idx < completeList.Count){
				currentItem.Value = completeList[idx];
				return State.Made;
			}
			currentItem.Value = null;
			return State.Empty;
		}
		void OnSetState(State before, State after){
			// switch(after){
			// case State.Empty:
			// 	gameObject.CLGetComponent<Image>("Image").sprite = null;
			// 	break;
			// case State.Made:
			// 	var staticItem = staticData.GetByID<GDCraftRecipeData>( currentItem.Value.recipeID ).rewardArr[0].item;
			// 	gameObject.CLGetComponent<Image>("Image").sprite = sprite.GetItem(staticItem.imageName);
			// 	break;
			// }
		}
		void OnUpdateState(State currState){}
		void Update(){
			state.Update();
		}
	}

//	List<DBCraftingItem> GetListOfCompleted(){
//		List<DBCraftingItem> orderByEndTime = new List<DBCraftingItem>();
//		var curList = runtimeData.GetList<DBCraftingItem>()
//			.Where(x=>x.tableUID==runtimeTable.uid)
//			.OrderBy(x=>{
//				x.
//			}).ToList();
//	}
}