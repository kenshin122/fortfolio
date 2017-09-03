using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Zenject;
using Newtonsoft.Json.Linq;

public partial class FIPopupDialog : CLSceneContext {
	[Inject]
	FIPopupManager popupManager;
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		LoadDefaultView("Etc/DefaultDialog","TopPopup");
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}
	public void DestroyPopup(){
		popupManager.DestroyPopup(this);
	}
	public string Title{
		get{
			return view.CLGetComponent<Text>("Window/Title").text;
		}set{
			view.CLSetFormattedText("Window/Title",value);
		}
	}
	public string Desc{
		get{
			return view.CLGetComponent<Text>("Window/Inner/Text").text;
		}set{
			view.CLSetFormattedText("Window/Inner/Text",value);
		}
	}
	public IObservable<Unit> BtnOneObservable{
		get{
			return view.CLOnClickAsObservable("Window/Btns/0");
		}
	}
	public IObservable<Unit> BtnTwoObservable{
		get{
			return view.CLOnClickAsObservable("Window/Btns/1");
		}
	}
	public string BtnOneText{
		get{
			return view.CLGetComponent<Text>("Window/Btns/0/Text").text;
		}set{
			view.CLSetFormattedText("Window/Btns/0/Text",value);
		}
	}
	public string BtnTwoText{
		get{
			return view.CLGetComponent<Text>("Window/Btns/1/Text").text;
		}set{
			view.CLSetFormattedText("Window/Btns/1/Text",value);
		}
	}
	public void SetBtnCnt(int cnt){
		switch(cnt){
		case 0:
			view.CLGetGameObject("Window/Btns/0").SetActive(false);
			view.CLGetGameObject("Window/Btns/1").SetActive(false);
			break;
		case 1:
			view.CLGetGameObject("Window/Btns/0").SetActive(true);
			view.CLGetGameObject("Window/Btns/1").SetActive(false);
			break;
		case 2:
			view.CLGetGameObject("Window/Btns/0").SetActive(true);
			view.CLGetGameObject("Window/Btns/1").SetActive(true);
			break;
		default:
			throw new System.Exception("Only can handle btn cnt 0,1,2");
//			break;
		}
	}
}
