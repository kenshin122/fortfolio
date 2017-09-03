using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
public partial class FIEasy{
	public System.DateTime ServerTime{
		get{
			return runtimeData.GetSingle<DBGlobalData>().Now;
		}
	}
	public float CalcRemainingRatio(System.DateTime started, System.TimeSpan reqTime){
		return (float)PassedTimespan(started).TotalSeconds / (float)reqTime.TotalSeconds;
	}
	public float CalcRemainingRatio(System.TimeSpan total, System.TimeSpan remaining){
		return (float)remaining.TotalSeconds / (float)total.TotalSeconds;
	}
	public System.TimeSpan PassedTimespan(System.DateTime started){
		return ServerTime - started; 
	}
	public System.TimeSpan RemainingTimespan(System.DateTime started, System.TimeSpan reqTime){
		return reqTime - PassedTimespan(started);
	}




	public DBUserInfo UserInfo{
		get{
			return runtimeData.GetSingle<DBUserInfo>();
		}
	}
	public float GetUserCurrentExpRatio(DBUserInfo _usrInfo){
		int needExp = staticData.GetList<GDUserLvInfo>()[_usrInfo.userLv+1].reqExp;
		return (float)_usrInfo.curExp / (float)needExp;
	}
	public float User_CurrentExpRatio{
		get{
			int needExp = staticData.GetList<GDUserLvInfo>()[UserInfo.userLv+1].reqExp;
			return (float)UserInfo.curExp / (float)needExp;
		}
	}
	public int StarPoint{
		get{
			return 0;
//			var listOfTraining = (from item in GDRManager.GetList<GDRManageTrainingInfo>()
//				where item.isUnlocked == true
//				select item);
//
//			int totalStarPoint = 0;
//			foreach(var item in listOfTraining){
//				GDManageTrainingData trainingData = GDManager.GetByKey<GDManageTrainingData>(string.Format("_{0}_{1}",item.baseIdx,item.upgradeLv));
//				totalStarPoint += trainingData.starPoint;
//			}
//			return totalStarPoint;
		}
	}

	public int CurrentStorageItemCnt{
		get{
			int totalCnt = 0;
			runtimeData.GetList<DBItem>().ForEach( item=>{
				if(staticData.GetByID<GDItemData>( item.itemID ).type.IsFlagSet(GDItemDataType.NotInv) == false)
					totalCnt += item.count;
			});
			return totalCnt;
		}
	}
	public int MaxStorageItemCnt{
		get{
			var userInfo = runtimeData.GetSingle<DBUserInfo>();
			return staticData.GetList<GDStorageUpgradeData>()[userInfo.storageUpgradeLv].size;
		}
	}
	public DBItem GetDBItemByID(int itemID){
		var itemData = (from item in runtimeData.GetList<DBItem>()
			where item.itemID == itemID
			select item).FirstOrDefault();
		return itemData;
	}
	public int GetItemCnt(int itemID){
		var itemData = GetDBItemByID(itemID);
		return itemData != null ? itemData.count : 0;
	}
	public bool CanDisposeItems(params GDCostItemInfo[] args){
		return CanDisposeItems( args.Select(x=>Tuple.Create<int,int>(x.item.id,x.cnt)).ToArray());
	}
	public bool CanDisposeItems(params Tuple<int,int>[] args){
//		var listOfItems = new List<Tuple<DBItem,int>>();
		foreach(var item in args){
			var foundItem = runtimeData.GetList<DBItem>().Where(x=>x.itemID==item.Item1).FirstOrDefault();
			if(foundItem == null)
				return false;
			if(foundItem.count < item.Item2)
				return false;
		}
		return true;
	}
}
