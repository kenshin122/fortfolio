using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;

public static partial class FIClientReqHandler{
	public static JObject enc_sess_storage_sellitem(FIFakeContext context){
		CheckParameterExists(context,"listOfSellItems");
		var list = context.body["listOfSellItems"] as JArray;
		var listOfItems = new List<Tuple<int,int>>();
		foreach(var item in list){
			listOfItems.Add( Tuple.Create<int,int>(item["uid"].Value<int>(),item["cnt"].Value<int>()) );
		}
		//Check every item exists..
		var disposingItemArr = listOfItems.ToArray();
		if(Storage_CheckCanDisposeItems(context,disposingItemArr) == false)
			throw new FIException(FIErr.Storage_CannotDisposeMoreThanHas);


		int totalPrice = 0;
		//Calc price
		foreach(var item in disposingItemArr){
			var staticItem = context.staticData.GetByID<GDItemData>(item.Item1);
			int singlePrice = staticItem.GetSellPrice(item.Item2);
			totalPrice += singlePrice;
		}

		Storage_DisposeItem(context,disposingItemArr);
		Storage_InsertItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,totalPrice));

		return GetDefaultJObject(context);
	}
	static Tuple<int,int>[] Storage_MergeItems(List<GDCostItemInfo> itemList){
		return Storage_MergeItems(itemList.Select(x=>Tuple.Create<int,int>(x.item.id,x.cnt)).ToArray());
	}
	static Tuple<int,int>[] Storage_MergeItems(params Tuple<int,int>[] args){
		var dic = new Dictionary<int,int>();
		foreach(var item in args){
			if(dic.ContainsKey(item.Item1) == false){
				dic.Add(item.Item1,item.Item2);
			}else{
				dic[item.Item1]+= item.Item2;
			}
		}
		return dic.Select(x=>Tuple.Create<int,int>(x.Key,x.Value)).ToArray();
	}
	static bool Storage_CheckCanInsertItems(FIFakeContext context, params Tuple<int,int>[] args){
		//Only count which can insert to inventory.
		int totalCnt = 0;
		var totalCountableItems = args.Where(x=>context.staticData.GetByID<GDItemData>(x.Item1).type.IsFlagSet(GDItemDataType.NotInv)==false).ToList();
		foreach(var item in totalCountableItems){
			totalCnt += item.Item2;
		}

		int curInvCnt = 0;
		var curCountableItems = context.dbContext.GetList<DBItem>()
			.Where(x=>context.staticData.GetByID<GDItemData>(x.itemID).type.IsFlagSet(GDItemDataType.NotInv)==false)
			.Select(x=>Tuple.Create<int,int>(x.itemID,x.count))
			.ToList();
		foreach(var item in curCountableItems){
			curInvCnt+= item.Item2;
		}

		return curInvCnt >= totalCnt;
	}
	static void Storage_InsertItem(FIFakeContext context, params Tuple<int,int>[] args){
		int totalExp = 0;
		foreach(var item in args){
			if(item.Item1 == GDInstKey.ItemData_userExp){
				totalExp+= item.Item2;
				continue;
			}
			var runtimeItem = context.dbContext.GetList<DBItem>().Where(x=>x.itemID==item.Item1).FirstOrDefault();
			runtimeItem = runtimeItem ?? context.dbContext.Create<DBItem>();
			runtimeItem.itemID = item.Item1;
			runtimeItem.count += item.Item2;
			InsertUpdated(context,runtimeItem);
		}
		if(totalExp>0){
			User_InsertExp(context,totalExp);
		}
	}
	static bool Storage_CheckCanDisposeItems(FIFakeContext context, params Tuple<int,int>[] args){
		foreach(var item in args){
			var runtimeItem = context.dbContext.GetList<DBItem>().Where(x=>x.itemID==item.Item1).FirstOrDefault();
			if(runtimeItem == null){
				return false;
			}
			if(runtimeItem.count < item.Item2)
				return false;
		}
		return true;
	}
	static void Storage_DisposeItem(FIFakeContext context, params Tuple<int,int>[] args){
		foreach(var item in args){
			var runtimeItem = context.dbContext.GetList<DBItem>().Where(x=>x.itemID==item.Item1).FirstOrDefault();
			runtimeItem = runtimeItem ?? context.dbContext.Create<DBItem>();
			runtimeItem.itemID = item.Item1;
			UnityEngine.Debug.Log(string.Format("Dispose itemID={0} itemCnt={1} to={2}",item.Item1,item.Item2,runtimeItem.count));
			runtimeItem.count -= item.Item2;
			InsertUpdated(context,runtimeItem);
		}
	}
}
