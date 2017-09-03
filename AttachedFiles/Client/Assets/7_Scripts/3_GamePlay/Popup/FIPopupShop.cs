using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

public partial class FIPopupShop : CLSceneContext{
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
		view = viewManager.CreateView("Popup/Shop","Popup");
		BindInstance();
		BindLogic();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}


	enum TabType{
		Ingredient,
		CraftingTable,
		Interior,
		Dia,
		Gold,
		Package,
		Free
	}
	CLScrollView scrollView;
	List<Tuple<TabType,int>> dataList;
	CLUnityExtensionToggleGroup toggle;
	void BindInstance(){
		scrollView = new CLScrollView();
	}
	void BindLogic(){
		view.CLOnClickAsObservable("Window/BottomBar/Button_Exit").Subscribe(_=>{
			popupManager.DestroyPopup(this);
		});
		scrollView.Init(view.CLGetGameObject("Window/ScrollView/Viewport/Content"),
			(idx,sample)=>{
				var item = dataList[idx];
				switch(item.Item1){
				case TabType.Ingredient:
					SetIngredient(item.Item2,sample);
					break;
				case TabType.CraftingTable:
					SetCraftingTable(item.Item2,sample);
					break;
				case TabType.Interior:
					SetInterior(item.Item2,sample);
					break;
				case TabType.Gold:
					SetShopByType(item.Item1,item.Item2,sample);
					break;
				case TabType.Dia:
					SetShopByType(item.Item1,item.Item2,sample);
					break;
				case TabType.Package:
					SetShopByType(item.Item1,item.Item2,sample);
					break;
				case TabType.Free:
					SetShopByType(item.Item1,item.Item2,sample);
					break;
				}
			},()=>{
				return dataList.Count;
			});
		toggle = view.CLAddToggleGroup("Window/ToggleGroup",selectedToggle=>{
			OnRefresh( (TabType)System.Enum.Parse(typeof(TabType),selectedToggle) );
		},"Ingredient");
	}
	void OnRefresh(TabType tabType){
//		dataList = new List<Tuple<TabType, int>>();
		switch(tabType){
		case TabType.Ingredient:{
				dataList = staticData.GetList<GDShopIngredient>().Select(x=>{
					return Tuple.Create<TabType,int>(tabType,x.id);
				}).ToList();
			}
			break;
		case TabType.CraftingTable:{
				dataList = staticData.GetList<GDShopCraftingTable>().Select(x=>{
					return Tuple.Create<TabType,int>(tabType,x.id);
				}).ToList();
			}
			break;
		case TabType.Interior:{
				dataList = staticData.GetList<GDShopInterior>().Select(x=>{
					return Tuple.Create<TabType,int>(tabType,x.id);
				}).ToList();
			}
			break;
		case TabType.Dia:{
				dataList = staticData.GetList<GDShopItem>()
					.Where(x=>x.type==GDShopTabType.Dia)
					.Select(x=>{
					return Tuple.Create<TabType,int>(tabType,x.id);
				}).ToList();
			}
			break;
		case TabType.Gold:{
				dataList = staticData.GetList<GDShopItem>()
					.Where(x=>x.type==GDShopTabType.Gold)
					.Select(x=>{
						return Tuple.Create<TabType,int>(tabType,x.id);
					}).ToList();
			}
			break;
		case TabType.Package:{
				dataList = staticData.GetList<GDShopItem>()
					.Where(x=>x.type==GDShopTabType.Package)
					.Select(x=>{
						return Tuple.Create<TabType,int>(tabType,x.id);
					}).ToList();
			}
			break;
		case TabType.Free:{
				dataList = staticData.GetList<GDShopItem>()
					.Where(x=>x.type==GDShopTabType.Free)
					.Select(x=>{
						return Tuple.Create<TabType,int>(tabType,x.id);
					}).ToList();
			}
			break;
		}
		scrollView.OnRefresh();
	}
}