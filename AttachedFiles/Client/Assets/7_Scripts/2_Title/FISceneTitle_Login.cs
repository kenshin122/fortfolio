using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using Newtonsoft.Json.Linq;

public partial class FISceneTitle : CLSceneContext{
	IEnumerator OnConfirmLogin(){
		view.CLGetGameObject("PressToContinue").SetActive(true);
		view.CLOnPointerClickAsObservable().Subscribe(x=>{
			ChangeScene("GamePlay");
		});
		yield return null;
	}
	IEnumerator OnLoginProcess(){
		if(buildOption.useLogin == false){
			FIFakeServerDataProvider temp = container.Resolve<IDataProvider>() as FIFakeServerDataProvider;
			temp.LoadServerData();

			var reqData = new JObject(
				new JProperty("deviceID","hello")
			);

			//HttpErr..
			//Problem
				//Internet is not connected!
					//
				//Wrong request!

			//Server Internal Error..
			//Problem
				//This is server that cannot process right. Its critical...
					//Redirect to start screen!

			//Timeout!
			//Problem
				//Server is busy.
					//Redirect to start screen
				//Internet connection is slow.
					//Redirect to start screen.

			//Then go to start!

			var req = server.GetCoroutine("enc/auth/logindid",reqData);
			yield return req;
			if( req.Err != FIErr.Okay ){
				yield break;
			}

			req = server.GetCoroutine("enc/sess/auth/userLogin",reqData);
			yield return req;
			if( req.Err == FIErr.Auth_NoUserInDatabase){
				req = server.GetCoroutine("enc/sess/auth/createuserinfo",JObject.FromObject(new{nick="conan"}));
				yield return req;
				req = server.GetCoroutine("enc/sess/auth/userLogin",reqData);
				yield return req;
			}else if(req.Err != FIErr.Okay){
				yield break;
			}

			//LoadInventory..
//			req = server.GetCoroutine("enc/sess/get/itemlist");
//			yield return req;

			StartCoroutine(OnConfirmLogin());
			yield break;
		}else{
			var loginPanel = view.CLGetGameObject("LoginPanel");
			loginPanel.SetActive(true);
			loginPanel.CLOnPointerClickAsObservable("Button_Guest").Subscribe(x=>{
				StartCoroutine(OnGuestLoginProcess());
			});
			loginPanel.CLOnPointerClickAsObservable("Button_Facebook").Subscribe(x=>{
				StartCoroutine(OnFacebookLoginProcess());
			});
		}
		yield return null;
	}
	IEnumerator OnGuestLoginProcess(){
		
		var loginPanel = view.CLGetGameObject("LoginPanel");
		loginPanel.SetActive(false);
		var reqData = new JObject(
			new JProperty("deviceID","hello")
		);

		var res = server.GetCoroutine("enc/auth/logindid",reqData);
		while(res.IsDone == false)
			yield return null;
		if(res.Err != FIErr.Okay){

		}

		StartCoroutine(OnConfirmLogin());
		yield return null;
	}
	IEnumerator OnFacebookLoginProcess(){
//		throw new Exception("Not implemented exceptions");
		yield return null;
	}

}
