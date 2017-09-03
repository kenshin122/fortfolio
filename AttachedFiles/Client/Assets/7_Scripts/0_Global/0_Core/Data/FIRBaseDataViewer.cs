using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
public class FIRBaseDataViewer : MonoBehaviour {
	DiContainer container;
//	FIRData data;

	[Inject]
	public void Construct(DiContainer _container){
		container = _container;
	}
	public bool loadData;
	public List<SingleData> totalData = new List<SingleData>();
	public List<SingleData> totalServerData = new List<SingleData>();
	void Update(){
		if(loadData == false){
			return;
		}

		FIRData srvData = (container.Resolve<IDataProvider>() as FIFakeServerDataProvider).srcContainer;
		GetData(totalServerData,srvData);
		FIRData cliData = (container.Resolve<IDataProvider>() as FIFakeServerDataProvider).RuntimeData as FIRData;
		GetData(totalData,cliData);

		loadData = false;
	}
	void GetData(List<SingleData> src, FIRData data){
		
		src.Clear();
		foreach(var classData in data.objectDic){
			foreach(var item in classData.Value){
				var singleItem = new SingleData();
				singleItem.type = string.Format("{0}:{1}",classData.Key.Name,item.Key);
				var jObj = JObject.FromObject(item.Value);
				foreach(var prop in jObj.Properties()){
					singleItem.inside.Add( string.Format("{0}:{1}",prop.Name,prop.Value) );
				}
				src.Add(singleItem);
			}
		}
	}
	[System.Serializable]
	public class SingleData{
		public string type = "";
		public List<string> inside = new List<string>();
	}
}
