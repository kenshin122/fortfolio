using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using Newtonsoft.Json.Linq;

public class FITestEasy{
	GDManager staticData;
	DiContainer container;
	FIDefaultRequest server;
	public FIRData serverData;
	[Inject]
	public void Construct(DiContainer _container,FIDefaultRequest _server,GDManager _staticData){
		container = _container;
		server = _server;
		staticData = _staticData;
		serverData = (container.Resolve<IDataProvider>() as FIFakeServerDataProvider).srcContainer;
	}
	public void EasyTestStart(System.Action onRes){
		PreloadStaticData();
		InsertFakeLoginData();
		InsertFakeItems();
		DoFakeLogin(onRes);
	}
	public void PreloadStaticData(){
		staticData.LoadFromXml(new GDInstCreator(),Resources.Load<TextAsset>("fiData").text);
	}
	public void InsertFakeLoginData(){
		//Fake Data Insertion.
		var serverData = (container.Resolve<IDataProvider>() as FIFakeServerDataProvider).srcContainer;
		//Insert fake user..
		var authData = serverData.Create<DBAuthData>();
		authData.grantType = GrantType.DeviceID;
		authData.grantValue = "hello";

		var userData = serverData.Create<DBUserInfo>();
		userData.authUID = authData.uid;
		userData.nick = "coranse";
	}
	public void InsertFakeItems(){
		//Insert fake itemData..
		for(int i = 0 ; i < 100 ; i++){
			int tryingKey = Random.Range(58,74);
			var itemData = serverData.GetList<DBItem>().Where(x=>x.itemID==tryingKey).FirstOrDefault();
			if(itemData == null){
				itemData = serverData.Create<DBItem>();
				itemData.itemID = tryingKey;
				itemData.count = 0;
			}
			itemData.count += Random.Range(1,3);
		}
	}
	public void InsertFakeItem(int itemID,int cnt = 1){
		var itemData = serverData.GetList<DBItem>().Where(x=>x.itemID==itemID).FirstOrDefault();
		if(itemData == null){
			itemData = serverData.Create<DBItem>();
			itemData.itemID = itemID;
			itemData.count = 0;
		}
		itemData.count += cnt;
	}
	public void DoFakeLogin(System.Action onRes){
		server.Get("enc/auth/logindid",JObject.FromObject(new{deviceID="hello"}))
			.SelectMany(x=>server.Get("enc/sess/auth/userLogin",new JObject()))
			.SelectMany(x=>server.Get("enc/sess/get/itemlist",new JObject()))
			.Subscribe(x=>{
				onRes();
			});
	}
	public void IncreaseTime(){
		server.Get("enc/sess/inctime",JObject.FromObject(
			new{time=System.TimeSpan.FromHours(1)}
		)).Subscribe(x=>{
			
		});
	}
}
