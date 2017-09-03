using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Zenject;
using UniRx;

public class FIIAPHandler{
	readonly FIBuildOptions options;
	readonly CLIABCommunicator iabController;
	readonly IDataProvider dataProvider;
	bool isAvailable;
	public FIIAPHandler(CLIABCommunicator _iabController, IDataProvider _dataProvider, FIBuildOptions _options){
		iabController = _iabController;
		dataProvider = _dataProvider;
		options = _options;
		iabController.InitIAB(options.packageName,res=>{
			if( res.Err == CLIABErr.Fine ){
				isAvailable = true;
			}
		});
	}
//	public IEnumerator BuyItemAsync(string itemKey){
//		if(isAvailable == false){
//			yield break;
//		}
//		var res = dataProvider.RequestAsync("enc/sess/googleiap/getorderid",null);
//		while(res.IsDone == false)
//			yield return null;
//	}

}
