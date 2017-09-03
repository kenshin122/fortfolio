using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FIClientDataProvider{
	FIBuildOptions buildOption;
	FIRBaseDataTypeContainer dataTypeContainer;

	public FIRData Data{
		get{
			return baseData;
		}
	}
	string FilePath{
		get{
			return Application.persistentDataPath+"/"+buildOption.clientDataFileName;
		}
	}

	FIRData baseData = new FIRData();

	public FIClientDataProvider(FIBuildOptions _buildOptions, FIRBaseDataTypeContainer _typeContainer){
		buildOption = _buildOptions;
		dataTypeContainer = _typeContainer;
		baseData = new FIRData();
		if(System.IO.File.Exists( FilePath ) == true){
			string jsonStr = System.IO.File.ReadAllText(FilePath);
			var jsonData = JsonConvert.DeserializeObject<JObject>(jsonStr);
			baseData.DeserializeFromJson(dataTypeContainer,jsonData);
			Debug.Log("[ClientDataProvider] Loaded");
		}else{
			Debug.Log("[ClientDataProvider] New");
		}
	}
	public void Save(){
		var jsonObject = baseData.SerializeToJson();
		System.IO.File.WriteAllText(FilePath,JsonConvert.SerializeObject(jsonObject) );
		Debug.Log("[ClientDataProvider] Saved");
	}
}
