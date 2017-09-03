using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Zenject;
using UniRx;
using DG.Tweening;

[CLContextAttrib("TopInfo")]
public class FITopInfo:CLSceneContext{
	[Inject]
	IRuntimeData runtimeData;
	[Inject]
	GDManager staticData;
	[Inject]
	FIEasy easy;
	[Inject]
	FIPopupManager popupManager;

	public override void OnGameStartLoaded (System.Action afterInit)
	{
		staticData.LoadFromXml(new GDInstCreator(),Resources.Load<TextAsset>("fiData").text);
		afterInit();
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}
	public override void OnInitialize (params object[] args){
		base.OnInitialize(args);

		view = viewManager.CreateView("Etc/TopInfo","Top");
		view.Visible = true;
		view.BlocksRaycast = true;

		view.CLGetComponent<Text>("LvInfo/Text").text = (easy.UserInfo.userLv+1).ToString();
		view.CLGetComponent<Slider>("LvInfo/Slider").value = easy.User_CurrentExpRatio;
		view.CLGetComponent<Text>("StarInfo/Text").text = (easy.UserInfo.userLv+1).ToString();
		view.CLGetComponent<Text>("CoinInfo/Value").text = easy.GetItemCnt(GDInstKey.ItemData_goldPoint).ToString();
		view.CLGetComponent<Text>("DiaInfo/Value").text = easy.GetItemCnt(GDInstKey.ItemData_diaPoint).ToString();

		//User update..
		runtimeData.GetObserver<DBUserInfo>().Subscribe(x=>{
			view.CLGetComponent<Text>("LvInfo/Text").text = (x.Item2.userLv+1).ToString();
			view.CLGetComponent<Slider>("LvInfo/Slider").value = easy.GetUserCurrentExpRatio(x.Item2);
			Debug.Log("BeforeLv="+x.Item1.userLv+" AfterLv="+x.Item2.userLv);
			if(x.Item1.userLv < x.Item2.userLv){
				var notice = popupManager.PushPopup<FIPopupDialog>();
				notice.SetNoticePopup($"Lv{x.Item1.userLv} to Lv{x.Item2.userLv}");
			}
		}).AddTo(go);
		//Gold update..
		runtimeData.GetObserver<DBItem>()
			.Where(x=>x.Item2.itemID == GDInstKey.ItemData_goldPoint)
			.Subscribe(x=>{
				Debug.Log("Gold Increased=!"+(x.Item2.count-x.Item1.count));
				view.CLGetComponent<Text>("CoinInfo/Value").text = x.Item2.count.ToString();
			}).AddTo(go);
		//Dia update..
		runtimeData.GetObserver<DBItem>()
			.Where(x=>x.Item2.itemID == GDInstKey.ItemData_diaPoint)
			.Subscribe(x=>{
				Debug.Log("Dia Increased=!"+(x.Item2.count-x.Item1.count));
				view.CLGetComponent<Text>("CoinInfo/Value").text = x.Item2.count.ToString();
			}).AddTo(go);

	}
	protected override void OnTestKeyPressedUp (string key)
	{
//		switch(key){
//		case "w":
//			testEasy.InsertItem(GDInstKey.ItemData_goldPoint,100);
//			break;
//		case "s":
//			testEasy.InsertItem(GDInstKey.ItemData_diaPoint,100);
//			break;
//		}
	}

	protected override void OnUpdate ()
	{
//		view.CLGetComponent<Text>("LvInfo/Text").text = (easy.UserInfo.userLv+1).ToString();
//		view.CLGetComponent<Slider>("LvInfo/Slider").value = easy.User_CurrentExpRatio;
//		view.CLGetComponent<Text>("StarInfo/Text").text = (easy.UserInfo.userLv+1).ToString();
//		view.CLGetComponent<Text>("CoinInfo/Value").text = easy.GetItemCnt(GDInstKey.ItemData_goldPoint).ToString();
//		view.CLGetComponent<Text>("DiaInfo/Value").text = easy.GetItemCnt(GDInstKey.ItemData_diaPoint).ToString();
	}
}
