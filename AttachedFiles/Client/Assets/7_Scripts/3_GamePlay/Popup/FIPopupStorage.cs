using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
using Newtonsoft.Json.Linq;

[CLContextAttrib("StoragePopup")]
public class FIPopupStorage : CLSceneContext{
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
		testEasy.DoFakeLogin(()=>{
			afterInit();
		});
	}
	protected override void OnTestKeyPressedUp (string key)
	{
		switch(key){
		case "w":
			var first = dataProvider.Get("enc_test1");
			var second = dataProvider.Get("enc_test2");
			Observable.WhenAll(first,second).Subscribe(x=>{
				Debug.Log("Got First="+x[0].Item2.ToString());
				Debug.Log("Got Second="+x[1].Item2.ToString());
			},()=>{
				Debug.Log("Completed!");
			});
			break;
		}
	}
	#endregion
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

//	CLView view;
	CLScrollView itemScroll;
	CLUnityExtensionToggleGroup toggle;

	List<DBItem> itemList;

	Button upgradeBtn;
	Button sellModeBtn;
	Button sellCancelBtn;
	Button sellBtn;
	GameObject sellModeGameObject;
	GameObject notSellModeGameObject;

	ReactiveProperty<bool> isSell = new ReactiveProperty<bool>();
	Dictionary<int,int> itemToSell = new Dictionary<int, int>();

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		var itemData = runtimeData.GetList<DBItem>();
		if(itemData.Count <= 0){
//			server.Request();
			server.Request("enc/sess/get/itemlist",null,null,(err,res)=>{
				PreloadFinished();
			});
		}else{
			PreloadFinished();
		}
	}
	public override void Dispose ()
	{
		base.Dispose ();
//		view.Dispose();
	}

	void PreloadFinished(){
		
		view = viewManager.CreateView("Popup/Storage","Popup");
		view.CLOnPointerClickAsObservable("Back").Subscribe(x=>{
			popupManager.DestroyPopup(this);
		});

		//Set Content items..
		itemScroll = new CLScrollView();
		itemScroll.Init(view.CLGetGameObject("Window/ScrollView/Viewport/Content"),(idx,go)=>{
			var curItem = itemList[idx];
			var baseItem = staticData.GetByID<GDItemData>( curItem.itemID );
			go.CLGetComponent<Image>("Image").sprite = sprite.GetItem(baseItem.imageName);
			go.CLSetFormattedText("NormalValue",
				baseItem.name,
				curItem.count
			);
			go.CLSetFormattedText("SellValue",
				baseItem.GetSellPrice()
			);

			var btn = go.CLGetComponent<Button>();
			btn.onClick.AsObservable().Subscribe(x=>{
				if(itemToSell.ContainsKey( baseItem.id ) == false)
					itemToSell.Add(baseItem.id,0);
				if(itemToSell[baseItem.id] < curItem.count){
					itemToSell[baseItem.id]++;
					go.CLSetFormattedText("Sell/Value",
						itemToSell.ContainsKey(baseItem.id) == false ? 0 : itemToSell[baseItem.id],
						curItem.count
					);
				}
			});
			isSell.Subscribe(x=>{
				if(x == true){
					go.CLGetGameObject("NormalValue").SetActive(false);
					go.CLGetGameObject("SellValue").SetActive(true);
					go.CLGetGameObject("NotSell").SetActive(false);
					go.CLGetGameObject("Sell").SetActive(true);

					go.CLSetFormattedText("Sell/Value",
						itemToSell.ContainsKey(baseItem.id) == false ? 0 : itemToSell[baseItem.id],
						curItem.count
					);
					btn.interactable = true;
				}else{
					go.CLGetGameObject("NormalValue").SetActive(true);
					go.CLGetGameObject("SellValue").SetActive(false);
					go.CLGetGameObject("NotSell").SetActive(true);
					go.CLGetGameObject("Sell").SetActive(false);

					go.CLSetFormattedText("NotSell/Value",
						curItem.count
					);

					btn.interactable = false;
				}
			}).AddTo(go);
		},()=>{
			return itemList.Count;
		});
		//		progressText = CLLabelFormatter.CreateFormatter(view.CLGetComponent<Text>("Window/TopBar/ProgressBar/Text"));



		upgradeBtn = view.CLGetComponent<Button>("Window/TopBar/Button_Upgrade");
		sellModeBtn = view.CLGetComponent<Button>("Window/BottomBar/NotSellMode/Button_Sell");
		sellCancelBtn = view.CLGetComponent<Button>("Window/BottomBar/SellMode/Button_Cancel");
		sellBtn = view.CLGetComponent<Button>("Window/BottomBar/SellMode/Button_Sell");

		sellModeGameObject = view.CLGetGameObject("Window/BottomBar/SellMode");
		notSellModeGameObject = view.CLGetGameObject("Window/BottomBar/NotSellMode");

		int totalMoney = 0;
		isSell.Subscribe(x=>{
			if(x == false){
				sellModeGameObject.SetActive(false);
				notSellModeGameObject.SetActive(true);
			}else{
				sellModeGameObject.SetActive(true);
				notSellModeGameObject.SetActive(false);
				itemToSell.Clear();
			}
		});

		Observable.EveryGameObjectUpdate().Subscribe(x=>{
			totalMoney = 0;
			foreach(var item in itemToSell){
				var baseItem = staticData.GetByID<GDItemData>(item.Key);
				totalMoney += baseItem.GetSellPrice( item.Value );
			}
			view.CLGetComponent<Text>("Window/BottomBar/SellMode/Value").text = totalMoney.ToString();
			if(totalMoney <= 0){
				sellBtn.interactable = false;
			}else{
				sellBtn.interactable = true;
			}
		}).AddTo(go);

		sellModeBtn.OnClickAsObservable().Subscribe(x=>{
			isSell.Value = true;
		});
		sellCancelBtn.OnClickAsObservable().Subscribe(x=>{
			isSell.Value = false;
		});
		sellBtn.OnClickAsObservable().Subscribe(x=>{

			var arr = new JArray();
			var prop = new JProperty("listOfSellItems",arr);
			var reqObj = new JObject(prop);
			foreach(var item in itemToSell){
				arr.Add( new JObject(
					new JProperty("uid",item.Key),
					new JProperty("cnt",item.Value)
				)
				);
			}
			server.Request("enc/sess/storage/sellitem",reqObj,null,(err,onRes)=>{
				Debug.Log("Success!");
				isSell.Value = false;
				toggle.Refresh();
			});
		});



		toggle = view.CLAddToggleGroup("Window/ToggleGroup",selectedToggle=>{
			OnRefresh( (Filter)System.Enum.Parse(typeof(Filter),selectedToggle) );
		},"All");
	}
	enum Filter{
		All,
		Farm,
		Create,
		Etc,
	}
	void OnRefresh(Filter selectedFilter){
		switch(selectedFilter){
		case Filter.All:
			itemList = (from item in runtimeData.GetList<DBItem>()
				where item.count > 0 &&
				staticData.GetByID<GDItemData>( item.itemID ).type.IsFlagSet(GDItemDataType.NotInv) == false
				select item).ToList();
			break;
		case Filter.Create:
			itemList = (from item in runtimeData.GetList<DBItem>()
				where item.count > 0 &&
				staticData.GetByID<GDItemData>( item.itemID ).type.IsFlagSet(GDItemDataType.CustomerEat) == true
				select item).ToList();
			break;
		case Filter.Farm:
			itemList = (from item in runtimeData.GetList<DBItem>()
				where item.count > 0 &&
				staticData.GetByID<GDItemData>( item.itemID ).type.IsFlagSet(GDItemDataType.Crop) == true
				select item).ToList();
			break;
		case Filter.Etc:
			itemList = (from item in runtimeData.GetList<DBItem>()
				where item.count > 0 &&
				staticData.GetByID<GDItemData>( item.itemID ).type != GDItemDataType.NotInv
				select item).ToList();
			break;
		}
		view.CLSetFormattedText("Window/TopBar/ProgressBar/Text",
			easy.CurrentStorageItemCnt,
			easy.MaxStorageItemCnt
		);

		view.CLGetComponent<Slider>("Window/TopBar/ProgressBar/Slider").value = (float)easy.CurrentStorageItemCnt/(float)easy.MaxStorageItemCnt;
		itemScroll.OnRefresh();

		//UpgradeCheck..
		var listOfUpgrade = staticData.GetList<GDStorageUpgradeData>();
		if(easy.UserInfo.storageUpgradeLv >= listOfUpgrade.Count - 1){
			//FullUpgrade!...
			view.CLGetComponent<Button>("Window/TopBar/Button_Upgrade").interactable = false;
			return;
		}
		view.CLGetComponent<Button>("Window/TopBar/Button_Upgrade").interactable = true;
	}



}
