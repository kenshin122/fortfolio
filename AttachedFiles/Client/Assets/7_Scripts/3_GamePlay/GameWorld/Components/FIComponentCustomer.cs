using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using DG.Tweening;
public class FIComponentCustomer : FIMonoBehaviour {
	FIEasy easy;
	IRuntimeData runtimeData;
	FISpriteData sprite;
	FIPopupManager popupManager;

	protected override void Awake(){
		base.Awake();
		easy = container.Resolve<FIEasy>();
		runtimeData = container.Resolve<IRuntimeData>();
		sprite = container.Resolve<FISpriteData>();
		popupManager = container.Resolve<FIPopupManager>();
	}

	CLScrollView scrollView;
	List<DBCustomer> customerList;
	void Start(){
		scrollView = new CLScrollView();
		scrollView.Init( gameObject,
			(idx,go)=>{
				go.CLOnPointerClickAsObservable().Subscribe(x=>{
					popupManager.PushPopup<FIPopupCustomer>(customerList[idx]);
				});

				UpdateInfo(go,idx);
				runtimeData.GetObserver(typeof(DBItem))
					.Merge( runtimeData.GetObserver(typeof(DBCustomer)))
					.Subscribe(x=>{
						UpdateInfo(go,idx);
					}).AddTo(go);
				ReactiveProperty<bool> isAvailable = new ReactiveProperty<bool>(false);
				isAvailable.Subscribe(x=>{
					if(x == true){
//						UpdateInfo(go,idx);
						go.CLGetComponent<RectTransform>("Mover").DOAnchorPosX(0,0.1f);
					}else{
						go.CLGetComponent<RectTransform>("Mover").DOAnchorPosX(400,0.1f);
					}
				});
				this.gameObject.CLUpdateAsObservable().Subscribe(x=>{
//					Debug.Log(string.Format("Cur={0} Cus={1}",easy.ServerTime,customerList[idx].waitStartedTime + easy.GlobalInfo.customerRegenTime));
					if( customerList[idx].waitStartedTime + easy.GlobalInfo.customerRegenTime  < easy.ServerTime )
						isAvailable.Value = true;
					else
						isAvailable.Value = false;
				});
			},()=>{
				return customerList.Count;
			}
		);
		customerList = runtimeData.GetList<DBCustomer>();
		scrollView.OnRefresh();
	}
	void UpdateInfo(GameObject go,int idx){
		var customerStaticData = easy.StaticData.GetByID<GDCustomerData>( customerList[idx].customerID );
		var itemData = easy.StaticData.GetByID<GDItemData>( customerList[idx].itemID );
		var curItemCnt = easy.GetItemCnt( customerList[idx].itemID );

		go.CLGetComponent<Image>("Mover/Item/Image").sprite = sprite.GetItem( itemData.imageName );
		go.CLGetComponent<Image>("Mover/Image").sprite = sprite.GetThumbPeople( customerStaticData.image );
		if(curItemCnt < customerList[idx].itemCnt){
			go.CLGetGameObject("Mover/Item/Check").SetActive(false);
		}else{
			go.CLGetGameObject("Mover/Item/Check").SetActive(true);
		}
	}
}
