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
	CLScrollView recipeInfoScrollView;
	List<Tuple<int,int>> recipeInfoDataList;
//	Subject<GDCraftRecipeData> recipeInfoObserv = new Subject<GDCraftRecipeData>();
	void BindRecipeInstance(){
		recipeInfoScrollView = new CLScrollView();
	}
	void BindRecipeLogic(){
		view.CLOnClickAsObservable("Window/RecipeInfo/State/Show/Button_Create").Subscribe(_=>{
			easy.IsResourceAvailable(selectedRecipeObserv.Value.reqItemArr,()=>{
				server.GetWithErrHandling("enc/sess/craft/insertrecipe",JObject.FromObject(
					new{
						tableUID=runtimeTable.uid,
						recipeID=selectedRecipeObserv.Value.id
					}))
					.Subscribe(x=>{
						//DoNothing..
					});
			});
		});

		recipeInfoScrollView.Init(view.CLGetGameObject("Window/RecipeInfo/State/Show/IngredientInfo/Grid"),
			(idx,sample)=>{
				var staticSingle = staticData.GetByID<GDItemData>(recipeInfoDataList[idx].Item1);
				var curCnt = easy.GetItemCnt( staticSingle.id );
				sample.CLSetFormattedText("Text"
					,recipeInfoDataList[idx].Item2
					,curCnt);
				runtimeData.GetAfterUpdateObserver<DBItem>()
					.Where(x=>x.itemID==staticSingle.id)
					.Subscribe(_=>{
						curCnt = easy.GetItemCnt( staticSingle.id );
						sample.CLSetFormattedText("Text"
							,curCnt
							,recipeInfoDataList[idx].Item2);
					}).AddTo(sample);
				sample.CLGetComponent<Image>("Image").sprite = sprite.GetItem(staticSingle.imageName);
			},()=>{
				return recipeInfoDataList.Count;
			});
		
		selectedRecipeObserv.Subscribe(data=>{
			if(data==null){
				view.CLGetGameObject("Window/RecipeInfo/State/Waiting").SetActive(true);
				view.CLGetGameObject("Window/RecipeInfo/State/Show").SetActive(false);
//				recipeInfoDataList = data.reqItemArr.Select(x=>Tuple.Create<int,int>( x.item.id, x.cnt )).ToList();
//				recipeInfoScrollView.OnRefresh();
				return;
			}
			view.CLGetGameObject("Window/RecipeInfo/State/Waiting").SetActive(false);
			view.CLGetGameObject("Window/RecipeInfo/State/Show").SetActive(true);

			view.CLGetComponent<Image>("Window/RecipeInfo/State/Show/Item").sprite = sprite.GetItem(data.rewardArr[0].item.imageName);
			view.CLSetFormattedText("Window/RecipeInfo/State/Show/ItemName",data.rewardArr[0].item.name);
			view.CLSetFormattedText("Window/RecipeInfo/State/Show/IngredientInfo/Time",data.reqTime);

			//Update...
			recipeInfoDataList = data.reqItemArr.Select(x=>Tuple.Create<int,int>( x.item.id, x.cnt )).ToList();
			recipeInfoScrollView.OnRefresh();
		}).AddTo(this.go);
//		recipeInfoObserv.OnNext(null);
	}
}