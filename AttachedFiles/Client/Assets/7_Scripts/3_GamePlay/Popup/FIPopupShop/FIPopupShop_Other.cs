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
	void SetIngredient(int id, GameObject sample){
		var single = staticData.GetByID<GDShopIngredient>(id);
		sample.CLGetComponent<Image>("ImageBack/Image").sprite = sprite.GetItem(single.item.imageName);
		sample.CLSetFormattedText("Name",single.item.name);
		sample.CLSetFormattedText("Desc/Text",single.desc);
		if(easy.UserInfo.userLv < single.unlockLv){
			sample.CLGetComponent<Button>("Button").interactable = false;
			sample.CLGetComponent<Text>("Button/Text").text = string.Format("잠김\n레벨{0}",single.unlockLv+1);
		}else{
			sample.CLGetComponent<Text>("Button/Text").text = string.Format("구매\nitem_goldPoint{0}",single.reqGold);
			sample.CLOnThrottleClickAsObservable("Button").Subscribe(_=>{
				server.GetWithErrHandling("enc/sess/shop/buyingredient",JObject.FromObject(new{id=id}))
					.Subscribe(x=>{
						//Bought!..
					});
			});
		}
	}
	void SetCraftingTable(int id, GameObject go){
		var single = staticData.GetByID<GDShopCraftingTable>(id);
		go.CLSetFormattedText("Name",single.table.name);
		go.CLSetFormattedText("Desc/Text",single.table.desc);
		//Check if its unlocked first..
		bool isUnlocked = false;
		//Check if level is above..
		if( easy.UserInfo.userLv >= single.unlockLv){
			isUnlocked = true;
		}
		if(isUnlocked == false){
			go.CLGetComponent<Button>("Button").interactable = false;
			go.CLGetComponent<Text>("Button/Text").text = string.Format("잠김\n레벨{0}",
				single.unlockLv+1);
		}else{
			go.CLGetComponent<Text>("Button/Text").text = string.Format("구매\nitem_goldPoint{0}",single.reqGold);
			go.CLOnThrottleClickAsObservable("Button").Subscribe(_=>{
				server.GetWithErrHandling("enc/sess/shop/buycraftingtable",JObject.FromObject(new{id=id}))
					.Subscribe(x=>{
						//Bought!..
						toggle.Refresh();
					});
			});
		}
	}
	void SetInterior(int id, GameObject go){
		var single = staticData.GetByID<GDShopInterior>(id);
		go.CLSetFormattedText("Name",single.interior.name);
		go.CLSetFormattedText("Desc/Text",single.interior.desc);

		//Check already has interior..
		var runtimeInterior = runtimeData.GetList<DBInterior>()
			.Where(x=>x.interiorID==single.interior.id)
			.FirstOrDefault();
		if(runtimeInterior != null){
			go.CLGetComponent<Button>("Button").interactable = false;
			go.CLGetComponent<Text>("Button/Text").text = "구매완료";
			return;
		}
		if(easy.UserInfo.userLv < single.unlockLv){
			//Need unlock..
			go.CLGetComponent<Text>("Button/Text").text = string.Format("바로구매\nitem_diaPoint{0}\n또는 레벨{1}",
				single.unlockDia,
				single.unlockLv+1);
			go.CLOnThrottleClickAsObservable("Button").Subscribe(_=>{
				server.GetWithErrHandling("enc/sess/shop/buyinteriorwithdia",JObject.FromObject(new{id=id}))
					.Subscribe(x=>{
						//Unlocked!..
						toggle.Refresh();
					});
			});
		}else{
			//can buy..
			go.CLGetComponent<Text>("Button/Text").text = string.Format("구매\nitem_goldPoint{0}",single.reqGold);
			go.CLOnThrottleClickAsObservable("Button").Subscribe(_=>{
				server.GetWithErrHandling("enc/sess/shop/buyinterior",JObject.FromObject(new{id=id}))
					.Subscribe(x=>{
						//Unlocked!..
						toggle.Refresh();
					});
			});
		}
	}
}