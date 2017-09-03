using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
public static partial class FIClientReqHandler{
	public static JObject enc_sess_resource_buyitemswithdia(FIFakeContext context){
		CheckParameterExists(context,"itemArr");
		var itemArr = context.body["itemArr"].ToTupleArr<int,int>();
		var merged = Storage_MergeItems(itemArr);

		//Check if items are all buyable with dia..
		int totalDia = 0;
		foreach(var item in merged){
			var data = context.staticData.GetByID<GDItemData>(item.Item1);
			if(data.diaPrice == 0){
				throw new FIException(FIErr.Resource_CannotBuyItemWithDia);
			}
			totalDia += data.diaPrice*item.Item2;
		}

		//Check can insert items to inventory..
		if( Storage_CheckCanInsertItems(context,merged) == false){
			throw new FIException(FIErr.Resource_CannotInsertStorageIsFull);
		}

		//DisposeDia..
		Storage_DisposeItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_diaPoint,totalDia));
		Storage_InsertItem(context,merged);

		return GetDefaultJObject(context);
	}
}
