using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CLContextAttrib("Logo")]
public partial class FISceneLogo:CLSceneContext{
	public override void OnGameStartLoaded (System.Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
	}
	public override void OnInitialize (params object[] args){
		base.OnInitialize(args);
		this.LoadDefaultView();
		container.Resolve<FIRBaseDataTypeContainer>();
//		StartCoroutine(OnSceneChange());
	}
	IEnumerator OnSceneChange(){
		yield return new WaitForSeconds(3.0f);
		ChangeScene("Title");
	}
}
