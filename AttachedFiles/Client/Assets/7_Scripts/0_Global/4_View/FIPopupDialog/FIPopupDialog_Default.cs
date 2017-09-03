using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;
using Newtonsoft.Json.Linq;

public partial class FIPopupDialog : CLSceneContext {
	public void SetNoticePopup(string desc){
		Title = "알림";
		Desc = desc;
		SetBtnCnt(1);
		BtnOneText = "확인";
		BtnOneObservable.Subscribe(_=>{
			DestroyPopup();
		});
	}

	public void SetErrorPopup(string desc){
		Title = "에러";
		Desc = desc;
		SetBtnCnt(1);
		BtnOneText = "확인";
		BtnOneObservable.Subscribe(_=>{
			DestroyPopup();
		});
	}
//	public void SetProcessNeedsResourceToContinue(string title,string desc,System.Action<FIPopupDialog> onYes){
//		Title = title;
//		System.Text.StringBuilder builder = new System.Text.StringBuilder();
//		builder.Append(desc+"\n");
////		Desc = desc;
//		SetBtnCnt(2);
//		BtnOneText = "확인";
//		BtnTwoText = "취소";
//
//
//
//		if(onYes != null){
//			BtnOneObservable.Subscribe(_=>{
//				onYes(this);
//			});
//		}else{
//			BtnOneObservable.Subscribe(_=>{
//				DestroyPopup();
//			});
//		}
//		BtnTwoObservable.Subscribe(_=>{
//			DestroyPopup();
//		});
//	}
	public void SetChoosePopup(
		string title,
		string desc,
		string okayText,
		string noText,
		System.Action<FIPopupDialog> onYes = null,
		System.Action<FIPopupDialog> onNo = null
	){
		Title = title;
		Desc = desc;
		SetBtnCnt(2);
		BtnOneText = okayText;
		BtnTwoText = noText;

		if(onYes != null){
			BtnOneObservable.Subscribe(_=>{
				onYes(this);
			});
		}else{
			BtnOneObservable.Subscribe(_=>{
				DestroyPopup();
			});
		}
		if(onNo != null){
			BtnTwoObservable.Subscribe(_=>{
				onNo(this);
			});
		}else{
			BtnTwoObservable.Subscribe(_=>{
				DestroyPopup();
			});
		}
	}
	public void SetLackResourcePopup(GDManager staticData, Tuple<int,int>[] lackList, System.Action<FIPopupDialog,int> onConfirm){
		Title = "재료부족";
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		builder.AppendLine("재료를 구매할까요?\n");
		int totalDia = 0;
		foreach(var item in lackList){
			var itemData = staticData.GetByID<GDItemData>(item.Item1);
			totalDia += itemData.diaPrice * item.Item2;
			builder.AppendLine(string.Format("item_{0}{1}",itemData.imageName,item.Item2));
		}
		Desc = builder.ToString();
		SetBtnCnt(2);
		BtnOneText = "item_diaPoint"+totalDia.ToString();
		BtnTwoText = "취소";
		BtnOneObservable.Subscribe(_=>{
			onConfirm(this,totalDia);
		});
		BtnTwoObservable.Subscribe(_=>{
			DestroyPopup();
		});
	}
//	public void SetLackOfDia(GDManager
}
