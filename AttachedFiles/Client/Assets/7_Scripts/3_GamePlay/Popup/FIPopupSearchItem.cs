using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

//[CLContextAttrib("PopupSearchItem")]
public class FIPopupSearchItem : CLSceneContext{
	#region Test
	[Inject]
	IDataProvider dataProvider;
	[Inject]
	FITestEasy testEasy;
	public override void OnGameStartLoaded (System.Action afterInit)
	{
		staticData.LoadFromXml(new GDInstCreator(),Resources.Load<TextAsset>("fiData").text);
		testEasy.PreloadStaticData();
		testEasy.InsertFakeLoginData();
		testEasy.InsertFakeItems();
		testEasy.InsertFakeItem(GDInstKey.ItemData_searchTicket,5);
		testEasy.DoFakeLogin(()=>{
			afterInit();
		});
	}
	protected override void OnTestKeyPressedUp (string key)
	{
		switch(key){
		case "w":
//			var first = dataProvider.Get("enc_test1");
//			var second = dataProvider.Get("enc_test2");
//			Observable.WhenAll(first,second).Subscribe(x=>{
//				Debug.Log("Got First="+x[0].Item2.ToString());
//				Debug.Log("Got Second="+x[1].Item2.ToString());
//			},()=>{
//				Debug.Log("Completed!");
//			});
			break;
		}
	}
	#endregion
	enum Filter{
		All,
		Farm,
		Create,
		Etc,
	}
	[Inject]
	FIPopupManager popupManager;
	[Inject]
	IRuntimeData runtimeData;
	[Inject]
	GDManager staticData;
	[Inject]
	FIEasy easy;
	[Inject]
	FISpriteData sprite;
	[Inject]
	FIDefaultRequest server;

	CLScrollView scrollView;
	List<GDItemData> itemList;
	CLUnityExtensionToggleGroup toggle;

	ReactiveProperty<GDItemData> selectedItem = new ReactiveProperty<GDItemData>();
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		BindInstance();
		BindLogic();
	}
	void BindInstance(){
		view = viewManager.CreateView("Popup/SearchItem","Popup");

	}
	void BindLogic(){

		//Quit
		var quitStream = view.CLOnPointerClickAsObservable("Back").Select(x=>Unit.Default);
		quitStream.Merge(view.CLOnClickAsObservable("Window/Button_Exit")).Subscribe(x=>{
			popupManager.DestroyPopup(this);
		});

		//ItemScrollView..
		scrollView = new CLScrollView();
		scrollView.Init(view.CLGetGameObject("Window/ScrollView/Viewport/Content"),
			(idx,go)=>{
				go.CLGetComponent<Image>("Item/Image").sprite = sprite.GetItem(itemList[idx].imageName);
				go.CLSetFormattedText("Item/Text",easy.GetItemCnt(itemList[idx].id) );
				selectedItem.Subscribe(x=>{
					if(x == itemList[idx]){
						go.CLGetGameObject("Check").SetActive(true);
					}else{
						go.CLGetGameObject("Check").SetActive(false);
					}
				}).AddTo(go);
				go.CLOnClickAsObservable().Subscribe(x=>{
					if(selectedItem.Value == itemList[idx]){
						selectedItem.Value = null;
					}else{
						selectedItem.Value = itemList[idx];
					}
				});
			},
			()=>{
				return itemList.Count;
			});

		//selected item..
		selectedItem.Subscribe(x=>{
			if(x == null){
				view.CLGetComponent<Button>("Window/BottomBar/Button_Confirm").interactable = false;
				view.CLGetGameObject("Window/BottomBar/Item/Inner").SetActive(false);
				view.CLGetGameObject("Window/BottomBar/Item/Empty").SetActive(true);
				view.CLGetComponent<Button>("Window/BottomBar/Item").interactable = false;
			}else{
				view.CLGetComponent<Button>("Window/BottomBar/Button_Confirm").interactable = true;
				view.CLGetGameObject("Window/BottomBar/Item/Inner").SetActive(true);
				view.CLGetGameObject("Window/BottomBar/Item/Empty").SetActive(false);
				view.CLGetComponent<Button>("Window/BottomBar/Item").interactable = true;
				view.CLGetComponent<Image>("Window/BottomBar/Item/Inner/Image").sprite = sprite.GetItem(x.imageName);
			}
		});

		//Confirm..
		view.CLOnClickAsObservable("Window/BottomBar/Button_Confirm").Subscribe(x=>{
			if( easy.GetItemCnt(GDInstKey.ItemData_searchTicket) <= 0 ){
//				throw new System.NotImplementedException("Need popup!");
				popupManager.PushPopup<FIPopupDialog>("Need Ticket!");
			}else{
				popupManager.DestroyPopup(this);
			}
		});

		//Update ticket count..
		view.CLSetFormattedText("Window/TopBar/Empty",easy.GetItemCnt(GDInstKey.ItemData_searchTicket));
		runtimeData.OnChangeObserver
			.Where(x=>x.Item2.GetType()==typeof(DBItem))
			.Select(x=>x.Item2 as DBItem)
			.Where(x=>x.itemID==GDInstKey.ItemData_searchTicket)
			.Subscribe(x=>{
//				x.count
				view.CLSetFormattedText("Window/TopBar/Empty",x.count);
			}).AddTo(go);


		//Filter toggle..
		toggle = view.CLAddToggleGroup("Window/ToggleGroup",selectedToggle=>{
			OnRefresh( (Filter)System.Enum.Parse(typeof(Filter),selectedToggle) );
		},"All");
	}
	void OnRefresh(Filter selectedFilter){
		switch(selectedFilter){
		case Filter.All:
			itemList = (from item in staticData.GetList<GDItemData>()
				where item.type.IsFlagSet(GDItemDataType.NotInv) == false
				select item).ToList();
			break;
		case Filter.Create:
			itemList = (from item in staticData.GetList<GDItemData>()
				where item.type.IsFlagSet(GDItemDataType.CustomerEat) == true
				select item).ToList();
			break;
		case Filter.Farm:
			itemList = (from item in staticData.GetList<GDItemData>()
				where item.type.IsFlagSet(GDItemDataType.Crop) == true
				select item).ToList();
			break;
		case Filter.Etc:
			itemList = (from item in staticData.GetList<GDItemData>()
				where item.type != GDItemDataType.NotInv
				select item).ToList();
			break;
		}
		scrollView.OnRefresh();
	}

	public override void Dispose ()
	{
		base.Dispose ();
	}
}
