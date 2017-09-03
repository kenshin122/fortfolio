using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
public class FIComponentCraftingTable : FIMonoBehaviour {
	FIEasy easy;
	GDManager staticData;
	IRuntimeData runtimeData;
	FISpriteData sprite;
	FIPopupManager popupManager;

	protected override void Awake(){
		base.Awake();
		easy = container.Resolve<FIEasy>();
		runtimeData = container.Resolve<IRuntimeData>();
		sprite = container.Resolve<FISpriteData>();
		popupManager = container.Resolve<FIPopupManager>();
		staticData = container.Resolve<GDManager>();
	}
	void Start(){
		BindInstance();
		BindLogic();
	}
	void OnDestroy(){

	}

	CLScrollView scrollView;
	List<DBCraftingTable> tableList;
	void BindInstance(){
		scrollView = new CLScrollView();
	}

	void BindLogic(){
		scrollView.Init(gameObject,
			(idx,go)=>{
				var staticTable = staticData.GetByID<GDCraftingTable>( tableList[idx].craftingTableID );
				go.CLOnClickAsObservable().Subscribe(_=>{
					popupManager.ChangePopup<FIPopupCraft>(tableList[idx]);
				});
				go.CLSetFormattedText("Text",staticTable.name);
			},()=>{
				return tableList.Count;
			});
		runtimeData.GetAfterTotalUpdateObserver().Subscribe(x=>{
			Debug.Log("TableUpdated!");
			OnRefresh();
		}).AddTo(this.gameObject);
		OnRefresh();
	}
	void OnRefresh(){
		tableList = runtimeData.GetList<DBCraftingTable>().OrderBy(x=>x.createdTime).ToList();
		scrollView.OnRefresh();
	}

}
