using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using Newtonsoft.Json.Linq;
public partial class FIEasy{
	public void CanIPerformProcessWithCostItem(string title,string desc,Tuple<int,int> item,System.Action onSuccess){
		CanIPerformProcessWithCostItem(title,desc,new Tuple<int,int>[]{item},onSuccess);
	}
	public void CanIPerformProcessWithCostItem(string title,string desc,Tuple<int,int>[] items,System.Action onSuccess){
		//Description
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		builder.AppendLine(desc);
		foreach(var item in items){
			var staticItem = staticData.GetByID<GDItemData>(item.Item1);
			builder.AppendLine(string.Format("item_{0}:{1}",staticItem.imageName,item.Item2));
		}

		var popup = popupManager.PushPopup<FIPopupDialog>();
		popup.Title = title;
		popup.Desc = builder.ToString();
		popup.SetBtnCnt(2);
		popup.BtnOneText = "확인";
		popup.BtnTwoText = "취소";
		popup.BtnOneObservable.Subscribe(_=>{
			//Check for items...
			IsResourceAvailable(items,()=>{
				popup.DestroyPopup();
				onSuccess();
			});
		});
		popup.BtnTwoObservable.Subscribe(_=>{
			popup.DestroyPopup();
		});

	}
	public void IsResourceAvailable(int itemID,int itemCnt,System.Action onSuccess){
		IsResourceAvailable(new Tuple<int,int>[]{Tuple.Create<int,int>(itemID,itemCnt)},onSuccess);	
	}
	public void IsResourceAvailable(List<GDCostItemInfo> costList,System.Action onSuccess){
		IsResourceAvailable(costList.Select(x=>Tuple.Create<int,int>(x.item.id,x.cnt)).ToArray(),onSuccess);
	}
	public void IsResourceAvailable(Tuple<int,int>[] itemArr,System.Action onSuccess){
		if( CanDisposeItems(itemArr) == false){
			//There is no items.. need by it when i have dia..
			var lackPopup = popupManager.PushPopup<FIPopupDialog>();
			var lackArr = CalcLackResource(itemArr);
			lackPopup.SetLackResourcePopup(staticData,lackArr,(tempPopup,diaCnt)=>{
				//I choose to buy it with dia..
				//Check dia..
				DoIHaveDia(diaCnt,()=>{
					//I have dia.. then buy..
					server.GetWithErrHandling("enc/sess/resource/buyitemswithdia",JObject.FromObject(new{itemArr=lackArr}))
						.Subscribe(x=>{
							lackPopup.DestroyPopup();
						});
				});
			});
			return;
		}
		onSuccess();
	}
	public void DoIHaveGold(int goldCnt,System.Action onSuccess){
		if(CanDisposeItems(Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,goldCnt))==false){
			var lackPopup = popupManager.PushPopup<FIPopupDialog>();
			lackPopup.SetChoosePopup(
				"알림",
				"골드가 부족합니다.\n상점으로 이동하시겠습니까?",
				"가자",
				"싫어",
				_=>{
					lackPopup.DestroyPopup();
					popupManager.ChangePopup<FIPopupShop>();
				});
			return;
		}
		if(onSuccess != null)
			onSuccess();
	}
	public void DoIHaveDia(int diaCnt,System.Action onSuccess = null){
		if(CanDisposeItems(Tuple.Create<int,int>(GDInstKey.ItemData_diaPoint,diaCnt))==false){
			var lackPopup = popupManager.PushPopup<FIPopupDialog>();
			lackPopup.SetChoosePopup(
				"알림",
				"다이아가 부족합니다.\n상점으로 이동하시겠습니까?",
				"가자",
				"싫어",
				_=>{
					lackPopup.DestroyPopup();
					popupManager.ChangePopup<FIPopupShop>();
				});
			return;
		}
		if(onSuccess != null)
			onSuccess();
	}
	public Tuple<int,int>[] CalcLackResource(List<GDCostItemInfo> request){
		return CalcLackResource( request.Select(x=>Tuple.Create<int,int>(x.item.id,x.cnt)).ToArray() );
	}
	public Tuple<int,int>[] CalcLackResource(Tuple<int,int>[] request){
		var merged = MergeCostItems(request);
		var listOfLack = new List<Tuple<int,int>>();
		foreach(var item in merged){
			int curCnt = this.GetItemCnt(item.Item1);
			if(curCnt < item.Item2){
				listOfLack.Add( Tuple.Create<int,int>(item.Item1, item.Item2 - curCnt));
			}
		}
		return listOfLack.ToArray();
	}
	public Tuple<int,int>[] MergeCostItems(List<GDCostItemInfo> itemList){
		return MergeCostItems(itemList.Select(x=>Tuple.Create<int,int>(x.item.id,x.cnt)).ToArray());
	}
	public Tuple<int,int>[] MergeCostItems(Tuple<int,int>[] itemArr){
		var dic = new Dictionary<int,int>();
		foreach(var item in itemArr){
			if(dic.ContainsKey(item.Item1)==false){
				dic.Add(item.Item1,item.Item2);
			}else{
				dic[item.Item1]+=item.Item2;
			}
		}
		return dic.Select(x=>Tuple.Create<int,int>(x.Key,x.Value)).ToArray();
	}
}
