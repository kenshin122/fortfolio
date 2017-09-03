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
	enum CoolState{
		Invalid=-1,
		CanBuy,
		CoolTime,
		CannotBuy,
	}
	void SetShopByType(TabType type, int id, GameObject go){
		var single = staticData.GetByID<GDShopItem>(id);
		go.CLSetFormattedText("Name",single.name);
		go.CLSetFormattedText("Desc/Text",single.desc);

		switch(type){
		case TabType.Gold:
		case TabType.Dia:{
				go.CLGetComponent<Text>("Button/Text").text = string.Format("구매\n{0}",single.price);
				go.CLOnClickAsObservable("Button").Subscribe(_=>{

				});
			}
			break;
		case TabType.Free:
		case TabType.Package:{
				//Check cooltime!
				DBShopItemCoolTime serverCoolTime = null;
				CLStateUpdator<CoolState> state = new CLStateUpdator<CoolState>();
				go.CLOnClickAsObservable("Button").Subscribe(_=>{
					//Buy code..
					switch(type){
					case TabType.Package:
						Debug.Log("Buy Package");
						break;
					case TabType.Free:
						Debug.Log("Buy Free");
						break;
					}
				});
				state.Init(()=>{
					serverCoolTime = runtimeData.GetList<DBShopItemCoolTime>()
						.Where(x=>x.shopItemID==id)
						.FirstOrDefault();
					if(serverCoolTime != null){
						if( serverCoolTime.endTime > easy.ServerTime ){
							//Its cooltime!..
							var remain = serverCoolTime.endTime - easy.ServerTime;
							if(remain > System.TimeSpan.FromDays(365)){
								return CoolState.CannotBuy;
							}
							return CoolState.CoolTime;
						}
					}
					return CoolState.CanBuy;
				},(before,after)=>{
					switch(after){
					case CoolState.CanBuy:
						go.CLGetComponent<Button>("Button").interactable = true;
						if(type == TabType.Package)
							go.CLGetComponent<Text>("Button/Text").text = string.Format("구매\n{0}",single.price);
						else
							go.CLGetComponent<Text>("Button/Text").text = single.price;
						break;
					case CoolState.CoolTime:
						go.CLGetComponent<Button>("Button").interactable = false;

						break;
					case CoolState.CannotBuy:
						go.CLGetComponent<Button>("Button").interactable = false;
						go.CLGetComponent<Text>("Button/Text").text = "구매함";
						break;
					}
				},(cur)=>{
					switch(cur){
					case CoolState.CoolTime:
						var timeSpan = serverCoolTime.endTime - easy.ServerTime;
						go.CLGetComponent<Text>("Button/Text").text = string.Format("쿨타임\n{0}",timeSpan);
						break;
					}
				});
				go.CLUpdateAsObservable().Subscribe(x=>{
					state.Update();
				});
			}
			break;
		}

	}
}