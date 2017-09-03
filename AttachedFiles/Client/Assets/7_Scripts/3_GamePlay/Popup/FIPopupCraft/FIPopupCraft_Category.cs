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
	CLScrollView categoryScrollView;
	CLScrollView recipeScrollView;
	List<GDCraftRecipeData> recipeList;
	ReactiveProperty<GDCraftRecipeData> selectedRecipeObserv = new ReactiveProperty<GDCraftRecipeData>();
	ReactiveProperty<GDCraftCategoryInfo> selectedCategoryObserv = new ReactiveProperty<GDCraftCategoryInfo>();
	void DisposeCategory(){
		selectedRecipeObserv.Dispose();
		selectedCategoryObserv.Dispose();
	}
	void BindCategoryInstance(){
		categoryScrollView = new CLScrollView();
		recipeScrollView = new CLScrollView();
	}
	void BindCategoryLogic(){
		List<GDCraftCategoryInfo> categoryList = null;
		System.Action onCategoryRefresh =  ()=>{
			var tempList = staticData.GetList<GDCraftCategoryInfo>();
			categoryList = new List<GDCraftCategoryInfo>();
			foreach(var item in tempList){
				//Check unlockable..
				var currentUnlock = runtimeData.GetList<DBCraftingRecipeCategoryUnlock>()
					.Where(x=>x.categoryID==item.id)
					.FirstOrDefault();
				if(currentUnlock != null){
					categoryList.Add( item );
				}else{
					categoryList.Add( item );
					break;
				}
			}
			categoryScrollView.OnRefresh();
		};

		view.CLOnClickAsObservable("Window/Category/CategoryState/Button_Unlock").Subscribe(_=>{
			var selectPopup = popupManager.PushPopup<FIPopupDialog>();
			selectPopup.SetChoosePopup(
				"알림",
				string.Format("item_goldPoint{0}에 언락하시겠습니까?",selectedCategoryObserv.Value.unlockReqGold),
				"승인",
				"거부",
				a=>{
					if(easy.GetItemCnt(GDInstKey.ItemData_goldPoint)<selectedCategoryObserv.Value.unlockReqGold){
						selectPopup.DestroyPopup();
						popupManager.PushPopup<FIPopupDialog>()
							.SetNoticePopup("골드가 부족합니다");
						return;
					}

					server.GetWithErrHandling("enc/sess/craft/unlockcategory",JObject.FromObject(new{id=selectedCategoryObserv.Value.id}))
						.Subscribe(x=>{
							selectPopup.DestroyPopup();
							popupManager.PushPopup<FIPopupDialog>()
								.SetNoticePopup("확장되었습니다");
							onCategoryRefresh();
							selectedCategoryObserv.SetValueAndForceNotify( selectedCategoryObserv.Value );
						});
				});
		});


		categoryScrollView.InitWithZenject(container,view.CLGetGameObject("Window/Category/ToggleScrollView/Viewport/Content"),
			(idx,sample)=>{
				var info = categoryList[idx];
				sample.CLGetComponent<Image>("ItemImage").sprite = sprite.GetItem(info.imageName);
				sample.CLOnClickAsObservable().Subscribe(_=>{
					selectedCategoryObserv.Value = info;
				});
			},()=>{
				return categoryList.Count;
			});
		onCategoryRefresh();

		recipeScrollView.InitWithZenject(container,view.CLGetGameObject("Window/Category/RecipeScrollView/Viewport/Content"),
			(idx,sample)=>{
				var comp = sample.AddComponent<RecipeChecker>();
				container.Inject(comp);
				comp.Init( recipeList, idx,selectedRecipeObserv );
			},()=>{
				return recipeList.Count;
			});





//		runtimeData.GetAfterUpdateObserver()
//			.Where(x=>{
//				if( x.GetType() == typeof(DBCraftingRecipeUnlock) ||
//					x.GetType() == typeof(DBUserInfo)){
//					return true;
//				}
//				return false;
//			}).Subscribe(_=>{
//				toggleGroup.Refresh();
//			}).AddTo(this.go);


		selectedCategoryObserv.Subscribe(changing=>{
			if(changing == null)
				return;
			
			int idx = -1;
			for(int i = 0 ; i < categoryList.Count ; i++){
				if(categoryList[i]==changing){
					idx = i;
					break;
				}
			}

			var createdList = categoryScrollView.CreatedList;
			for(int i = 0 ; i < createdList.Count ; i++){
				if(i == idx){
					createdList[i].CLGetGameObject("Selected").SetActive(true);
				}else{
					createdList[i].CLGetGameObject("Selected").SetActive(false);
				}
			}

			var curUnlockState = runtimeData.GetList<DBCraftingRecipeCategoryUnlock>()
				.Where(x=>x.categoryID==changing.id)
				.FirstOrDefault();
			if(curUnlockState != null){
				recipeList = staticData.GetList<GDCraftRecipeData>()
					.Where(x=>x.category==changing.type)
					.ToList();
				recipeScrollView.OnRefresh();
				view.CLGetGameObject("Window/Category/CategoryState").SetActive(false);
			}else{
				view.CLGetGameObject("Window/Category/CategoryState").SetActive(true);
				if(easy.UserInfo.userLv<changing.unlockLv){
					view.CLGetComponent<Button>("Window/Category/CategoryState/Button_Unlock").interactable = false;
					view.CLSetFormattedText("Window/Category/CategoryState/Button_Unlock/Text",
						string.Format("잠김\nitem_goldPoint{0}",changing.unlockReqGold));
				}else{
					view.CLGetComponent<Button>("Window/Category/CategoryState/Button_Unlock").interactable = true;
					view.CLSetFormattedText("Window/Category/CategoryState/Button_Unlock/Text",
						string.Format("잠금해제\nitem_goldPoint{0}",changing.unlockReqGold));
				}
			}
		});
		selectedCategoryObserv.Value = categoryList[0];
	}
	void OnRefreshToggle(GDCraftCategory category){
		recipeList = staticData.GetList<GDCraftRecipeData>().Where(x=>x.category==category).ToList();
		recipeScrollView.OnRefresh();
	}

	class RecipeChecker:MonoBehaviour{
		[Inject]
		IRuntimeData runtimeData;
		[Inject]
		GDManager staticData;
		[Inject]
		FIEasy easy;
		[Inject]
		FISpriteData sprite;
		[Inject]
		FIPopupManager popupManager;
		[Inject]
		FIDefaultRequest server;


		enum State{
			Invalid = -1,
			Normal,
			Locked,
			Unlockable,
			Hide,
		}
		State currState;
		int idx;
		List<GDCraftRecipeData> recipeList;
		GDCraftRecipeData currRecipe;
		GDItemData resultItem;
		ReactiveProperty<GDCraftRecipeData> selectedRecipeObserv;
		public void Init(List<GDCraftRecipeData> _recipeList,int _idx,ReactiveProperty<GDCraftRecipeData> _selectedRecipeObserv){
			recipeList = _recipeList;
			idx = _idx;
			selectedRecipeObserv = _selectedRecipeObserv;
			currRecipe = recipeList[idx];
			resultItem = currRecipe.rewardArr[0].item;

			gameObject.CLOnClickAsObservable().Subscribe(_=>{
				switch(currState){
				case State.Normal:
					if(selectedRecipeObserv.Value == currRecipe){
						selectedRecipeObserv.Value = null;
					}else{
						selectedRecipeObserv.Value = currRecipe;
					}
					break;
				case State.Locked:

					break;
				case State.Unlockable:
					easy.CanIPerformProcessWithCostItem(
						"레시피 언락",
						"다음 재료가 듭니다",
						Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,currRecipe.unlockReqGold),
						()=>{
							server.GetWithErrHandling("enc/sess/craft/unlockrecipe",JObject.FromObject(new{id=currRecipe.id}))
								.Subscribe(x=>{
									//Success!
									popupManager.PushPopup<FIPopupDialog>().SetNoticePopup("언락하였습니다!");
								});
					});
					break;
				}
			});

			selectedRecipeObserv.Subscribe(item=>{
				if(item == currRecipe){
					gameObject.CLGetGameObject("Selected").SetActive(true);
				}else{
					gameObject.CLGetGameObject("Selected").SetActive(false);
				}
			}).AddTo(gameObject);

			runtimeData.GetAfterTotalUpdateObserver().Subscribe(_=>{
				OnUpdate();
			}).AddTo(gameObject);
			OnUpdate();
		}
		void OnUpdate(){
			int lastUnlockableIdx = -1;
			for(int i = 0 ; i < recipeList.Count ; i++){
				var unlockedData = runtimeData.GetList<DBCraftingRecipeUnlock>().Where(x=>x.recipeID==recipeList[i].id).FirstOrDefault();
				if(unlockedData == null){
					lastUnlockableIdx = i;
					break;
				}
			}
			//Its unlocked all..
			if(lastUnlockableIdx == -1){
				lastUnlockableIdx = 1000;
			}

			var curCnt = easy.GetItemCnt(resultItem.id);
			gameObject.CLGetComponent<Image>("ItemImage").sprite = sprite.GetItem( resultItem.imageName );
			if(idx < lastUnlockableIdx){
				currState = State.Normal;
				gameObject.CLSetFormattedText("Text",string.Format("재고:{0}",curCnt));
				gameObject.CLGetGameObject("Disabled").SetActive(false);
			}else if(idx == lastUnlockableIdx){
				if(easy.UserInfo.userLv < currRecipe.unlockLv){
					currState = State.Locked;
					gameObject.CLSetFormattedText("Text",string.Format("잠김\n레벨{0}",currRecipe.unlockLv+1));
					gameObject.CLGetGameObject("Disabled").SetActive(true);
				}else{
					currState = State.Unlockable;
					gameObject.CLSetFormattedText("Text",string.Format("해제가능\n레벨{0}",currRecipe.unlockLv+1));
					gameObject.CLGetGameObject("Disabled").SetActive(true);
				}
			}else{
				//This is hidden..
				currState = State.Hide;
				gameObject.CLGetComponent<Image>("ItemImage").sprite = null;
				gameObject.CLGetGameObject("Disabled").SetActive(true);
				gameObject.CLSetFormattedText("Text","");
			}
		}
	}
}