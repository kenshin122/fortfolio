using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;
using UniRx;
public interface IDataProvider{
	IRuntimeData RuntimeData{get;}
	IDataProviderRequestAsync RequestAsync(string req, JObject obj, CancellationToken? token);
	void Request(string req, JObject obj, CancellationToken? token,  System.Action<FIErr,JObject> onRes);
	IObservable<Tuple<FIErr,JObject>>Get(string req, JObject obj = null);
}
public class FIDataProviderBase{
	public IRuntimeData RuntimeData{
		get{
			return targetContainer;
		}
	}
//	protected bool isProcessing;
//	public bool IsProcessing{
//		get{
//			return isProcessing;
//		}
//	}
//	protected System.DateTime processStartedTime;
//	public float ProcessElapsedTime{
//		get{
//			return (float)( (System.DateTime.Now - processStartedTime).TotalMilliseconds / 1000.0f );
//		}
//	}
	[Inject]
	protected FIRBaseDataTypeContainer dataTypeContainer;
	protected FIRData targetContainer = new FIRData();
	protected void ProcessResult(JObject resData){
		var updated = resData["updated"] as JObject;
		var deleted = resData["deleted"] as JObject;
		if(updated != null){
			var serializeSettings = new JsonSerializerSettings();
			serializeSettings.ContractResolver = new PrivateSetterContractResolver();

			var serializer = JsonSerializer.Create(serializeSettings);
			foreach(JProperty classData in updated.Properties()){
				var type = dataTypeContainer.TypeDic[classData.Name];
				var arr = classData.Value as JArray;
				foreach(JObject itemData in arr){
					FIRBaseData temp = null;
					int uid = itemData["uid"].Value<int>();
					if( targetContainer.ContainsID(type, uid ) == false){
						temp = targetContainer.Create(type);
						Debug.Log($"created class={type.Name} uid={uid}");
					}else{
						temp = targetContainer.GetByID(type,uid);
						Debug.Log($"updated class={type.Name} uid={uid}");
					}

					//Notify change!
					var afterData = (FIRBaseData)itemData.ToObject(type);
					targetContainer.notifier.OnNext(Tuple.Create<FIRBaseData,FIRBaseData>(temp,afterData));

					//					JsonConvert.PopulateObject(itemData.ToString(),temp,serializeSettings);
					//Update Item
					using(var reader = itemData.CreateReader()){
						serializer.Populate(reader,temp);
					}
					targetContainer.afterNotifier.OnNext(temp);
				}
			}
		}
		if(deleted != null){
			foreach(JProperty classData in deleted.Properties()){
				var type = dataTypeContainer.TypeDic[classData.Name];
				var arr = classData.Value as JArray;
//				Debug.Log("Trying="+type.Name);
				foreach(JObject itemData in arr){
					int uid = itemData["uid"].Value<int>();
					if( targetContainer.ContainsID(type, uid ) == true){
						targetContainer.Dispose(type,uid);
						Debug.Log($"Deleted class={type.Name} uid={uid}");
					}else{
						throw new System.Exception($"try to delete not exist class={type.Name} uid={uid}");
					}
				}
			}
		}
		targetContainer.totalAfterNotifier.OnNext(Unit.Default);
	}
}
public class FIAsyncData:IDataProviderRequestAsync{
	public bool _isDone;
	public bool IsDone{get{return _isDone;}}
	public FIErr _err;
	public FIErr Err{get{return _err;}}
	public JObject _res;
	public JObject Res{get{return _res;}}
}
