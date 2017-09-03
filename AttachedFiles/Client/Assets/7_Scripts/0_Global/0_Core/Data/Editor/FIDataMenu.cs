using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class FIDataMenu{

	[MenuItem("FantasyInn/DeleteFakeServerData")]
	static void DeleteFakeServerData(){
		var fileName = GameObject.Find("BuildOptions").GetComponent<FIBuildOptions>().fakeServerFileName;
		var totalPath = Application.persistentDataPath+"/"+fileName ;
		if(System.IO.File.Exists( totalPath ) ){
			System.IO.File.Delete( totalPath );
		}
	}
	[MenuItem("FantasyInn/DeleteClientData")]
	static void DeleteClientData(){
		var fileName = GameObject.Find("BuildOptions").GetComponent<FIBuildOptions>().clientDataFileName;
		var totalPath = Application.persistentDataPath+"/"+fileName ;
		if(System.IO.File.Exists( totalPath ) ){
			System.IO.File.Delete( totalPath );
		}
	}
}
