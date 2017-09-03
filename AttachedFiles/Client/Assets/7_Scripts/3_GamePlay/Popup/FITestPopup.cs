using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using UniRx;
public class FITestPopup : CLSceneContext {
	[Inject]
	FIPopupManager popupManager;

	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize (args);
		LoadDefaultView("Etc/TestResultDialog","Popup");
		view.CLGetComponent<Text>("Text").text = (string)args[0];
		view.CLOnPointerClickAsObservable("Button_Okay").Subscribe(_=>{
			popupManager.DestroyPopup(this);
		});
	}
	public override void Dispose ()
	{
		base.Dispose ();
	}
}
