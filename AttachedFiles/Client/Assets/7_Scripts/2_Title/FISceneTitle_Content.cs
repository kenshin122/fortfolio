using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
public partial class FISceneTitle : CLSceneContext{
	IEnumerator OnContentLoadProcess(){
		if(buildOption.loadContentFromNetwork == true){
			throw new System.Exception("Not Emplemented loadContentFromNetwork!");
		}
		var loadingPanel = view.CLGetGameObject("LoadingPanel");
		var slider = view.CLGetComponent<Slider>("LoadingPanel/Slider_Loading");
		var label = view.CLGetComponent<Text>("LoadingPanel/Label_Info");
		loadingPanel.SetActive(true);
		var asyncState = data.LoadFromXmlAsync(new GDInstCreator(),Resources.Load<TextAsset>("fiData").text);
		while(asyncState.IsDone == false){
			slider.value = asyncState.Perc;
			label.text = asyncState.State;
			yield return null;
		}
		loadingPanel.SetActive(false);
		StartCoroutine(OnLoginProcess());
	}
}
