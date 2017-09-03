using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using UnityEngine;
//Need this script to be attached to CLInAppBilling GameObject from Scene

public class CLIABCommunicator : MonoBehaviour {
	bool isOnProcess;
	public IIABAsyncResult Result{
		get{
			return result;
		}
	}
	CLIABAsyncResult result;
	#if UNITY_ANDROID
	AndroidJavaObject billing;
	#endif
	public void InitIAB(string packageName,System.Action<IIABAsyncResult> res){
		StartCoroutine(InitIABAsync(packageName,res));
	}
	public IEnumerator InitIABAsync(string packageName,System.Action<IIABAsyncResult> res = null){
		yield return StartCoroutine(CallSDKFunc("IABInit",packageName));
		if(res != null)
			res( result );
	}
	public void Echo(string msg,System.Action<IIABAsyncResult> res){
		StartCoroutine(EchoAsync(msg,res));
	}
	public IEnumerator EchoAsync(string msg,System.Action<IIABAsyncResult> res = null){
		yield return StartCoroutine(CallSDKFunc("Echo",msg));
		if(res != null)
			res( result );
	}
	public void PerformPurchase(string developerPayload,string productID, System.Action<IIABAsyncResult> res){
		StartCoroutine(PerformPurchaseAsync(developerPayload,productID,res));
	}
	public IEnumerator PerformPurchaseAsync(string developerPayload,string productID, System.Action<IIABAsyncResult> res = null){
		yield return StartCoroutine(CallSDKFunc("PerformPurchase",developerPayload,productID));
		if(res != null)
			res( result );
	}
	public void PerformSpending(string purchaseToken, System.Action<IIABAsyncResult> res){
		StartCoroutine(PerformSpendingAsync(purchaseToken,res));
	}
	public IEnumerator PerformSpendingAsync(string purchaseToken, System.Action<IIABAsyncResult> res = null){
		yield return StartCoroutine(CallSDKFunc("PerformSpending",purchaseToken));
		if(res != null)
			res( result );
	}
	public void PerformGetListOfPurchased(System.Action<IIABAsyncResult> res){
		StartCoroutine(PerformGetListOfPurchasedAsync(res));
	}
	public IEnumerator PerformGetListOfPurchasedAsync(System.Action<IIABAsyncResult> res = null){
		yield return StartCoroutine(CallSDKFunc("PerformGetListOfPurchased"));
		if(res != null)
			res( result );
	}







	IEnumerator CallSDKFunc(string func, params object[] args){
//		Debug.Log("Perform "+func);
		result = new CLIABAsyncResult();
		if(isOnProcess == true){
			result._err = CLIABErr.Processing;
			yield break;
		}
		if(Application.isEditor == true){
			result._err = CLIABErr.NotMobilePlatform;
			yield break;
		}

		isOnProcess = true;
		OnProcess(true,func,args);
		while(isOnProcess)
			yield return null;
		OnProcess(false,func);
	}
	void OnProcess(bool isStart,string func, params object[] args){
		Debug.Log("Processing "+func+" start="+isStart.ToString());
		switch(func){
		case "Echo":
			#if UNITY_ANDROID
			if(isStart == true){
				if(Application.platform == RuntimePlatform.Android ){
					billing.Call("Echo",(string)args[0]);
				}
			}else{
				isOnProcess = false;
				result._err = errCode;
				result._res = new JObject(new JProperty("msg",data));
			}
			#endif
			break;
		case "IABInit":
			#if UNITY_ANDROID
			if(isStart == true){
				if(Application.platform == RuntimePlatform.Android ){
					billing = new AndroidJavaObject("com.chromecat.billing.IABUnity");
					billing.Call("IABInit",(string)args[0]);
				}
			}else{
				isOnProcess = false;
				result._err = errCode;
				result._res = new JObject(new JProperty("msg",data));
			}
			#endif
			break;
		case "PerformPurchase":
			#if UNITY_ANDROID
			if(isStart == true){
				if(Application.platform == RuntimePlatform.Android ){
					billing.Call("PerformPurchase",(string)args[0],(string)args[1]);
				}
			}else{
				isOnProcess = false;
				result._err = errCode;
				result._res = new JObject(new JProperty("msg",data));
			}
			#endif
			break;
		case "PerformSpending":
			#if UNITY_ANDROID
			if(isStart == true){
				if(Application.platform == RuntimePlatform.Android ){
					billing.Call("PerformSpending",(string)args[0]);
				}
			}else{
				isOnProcess = false;
				result._err = errCode;
				result._res = new JObject(new JProperty("msg",data));
			}
			#endif
			break;
		case "PerformGetListOfPurchased":
			#if UNITY_ANDROID
			if(isStart == true){
				if(Application.platform == RuntimePlatform.Android ){
					billing.Call("PerformGetListOfPurchased");
				}
			}else{
				isOnProcess = false;
				if(string.IsNullOrEmpty(data) == false){
					var xmlData = XElement.Parse(data);
					var jsonArr = new JArray();
					var jsonProperty = new JProperty("products",jsonArr);
					var jsonObject = new JObject(jsonProperty);
					foreach(var item in xmlData.Elements()){
						jsonArr.Add( new JObject(
							new JProperty("productID",item.Element("productID").Value),
							new JProperty("purchaseState",item.Element("purchaseState").Value),
							new JProperty("developerPayload",item.Element("developerPayload").Value),
							new JProperty("purchaseToken",item.Element("purchaseToken").Value)
						));
					}
					result._err = errCode;
					result._res = jsonObject;
				}else{
					result._err = errCode;
					result._res = null;
				}
			}
			#endif
			break;
		}
	}
	CLIABErr errCode;
	string data;
	void OnIABResult(string str){
		Debug.Log("OnIABResult ="+str);
		isOnProcess = false;
		errCode = (CLIABErr)str[0];
		data = null;
		if(str.Length > 1){
			data = str.Substring(1);
		}
	}
	class CLIABAsyncResult:IIABAsyncResult{
		public CLIABErr _err;
		public CLIABErr Err{get{return _err;}}
		public JObject _res;
		public JObject Res{get{return _res;}}
	}
}
public enum CLIABErr{
	NotMobilePlatform = -2,
	Processing = -1,
	Fine = 10,
	Init = 13,
	Purchase = 15,
	Consume = 17,
	Get_Purchased_List = 19,
	Not_Consumed_Left = 21,
}
public enum CLIABPurchaseState{
	Purchased = 0,
	Canceled = 1,
	Refunded = 2,
}
public interface IIABAsyncResult{
	CLIABErr Err{get;}
	JObject Res{get;}
}