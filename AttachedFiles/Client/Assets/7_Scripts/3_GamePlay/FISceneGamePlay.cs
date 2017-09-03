using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Newtonsoft.Json.Linq;
using DG.Tweening;
[CLContextAttrib("GamePlay")]
public partial class FISceneGamePlay : CLSceneContext{
	#region Test
	[Inject]
	IDataProvider dataProvider;
	[Inject]
	FITestEasy testEasy;
	public override void OnGameStartLoaded (System.Action afterInit)
	{

		container.Resolve<GDManager>().LoadFromXml(new GDInstCreator(),Resources.Load<TextAsset>("fiData").text);
		testEasy.PreloadStaticData();
		testEasy.InsertFakeLoginData();
		testEasy.InsertFakeItems();
//		testEasy.serverData.
		testEasy.InsertFakeItem(GDInstKey.ItemData_farmItem0,100);
		testEasy.InsertFakeItem(GDInstKey.ItemData_searchTicket,5);
		testEasy.InsertFakeItem(GDInstKey.ItemData_goldPoint,100000);
		testEasy.InsertFakeItem(GDInstKey.ItemData_diaPoint,1000);
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
	[Inject]
	readonly FIPopupManager popupManager;
	[Inject]
	readonly GDManager data;
	[Inject]
	readonly IRuntimeData runtimeData;
	[Inject]
	readonly FIDefaultRequest server;

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize(args);

		var invenInfoObserv = server.Get("enc/sess/get/itemlist");
		var customerObserv = server.Get("enc/sess/customer/getlist");
		var orderObserv = server.Get("enc/sess/order/getlist");
		var achievObserv = server.Get("enc/sess/achievement/getlist");
		var craftObserv = server.Get("enc/sess/craft/getcraftinfo");
		var categoryObserv = server.Get("enc/sess/craft/getcategoryinfo");
		var allReqObserv = Observable.WhenAll(
			invenInfoObserv,
			customerObserv,
			orderObserv,
			achievObserv,
			craftObserv,
			categoryObserv
		);
		allReqObserv.Subscribe(x=>{
			//Check all matches..
			bool isSuccess = true;
			foreach(var res in x){
				if(res.Item1 != FIErr.Okay){
					isSuccess = false;
					Debug.LogError("Err="+res.Item1.ToString());
					break;
				}
			}
			if(isSuccess == true){
				PreloadFinished();
			}
		});
	}
	public override void Dispose ()
	{
		base.Dispose ();
		DisposeInstance();

	}
	void PreloadFinished(){
		var now = System.DateTime.Now;
		var before = now - System.TimeSpan.FromDays(1);
		var span = before - now;
		Debug.Log(span);

//		Debug.Log("Value="+System.TimeSpan.FromDays(365).ToString());

		BindInstance();
		BindLogic();
	}

	void BindInstance(){
		BindUIInstance();
		BindGameWorldInstance();
		BindCameraInstance();
	}
	void DisposeInstance(){
		DisposeCameraInstance();
		DisposeGameWorldInstance();
		DisposeUIInstance();
	}
	void BindLogic(){
		BindUILogic();
		BindGameWorldLogic();
		BindCameraLogic();
	}
}
