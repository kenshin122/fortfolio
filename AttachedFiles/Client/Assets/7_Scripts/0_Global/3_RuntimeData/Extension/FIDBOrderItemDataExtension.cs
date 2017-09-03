using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DBOrderItem: FIRBaseData{
	public GDItemData GetItem(GDManager staticData){
		return staticData.GetByID<GDItemData>(itemID);
	}
	public int CalcOrderPrice(GDManager staticData){
		return GetItem(staticData).GetOrderPrice(itemCnt);
	}
	public int CalcOrderRewardExp(GDManager staticData){
		return staticData.GetByKey<GDEdibleItemData>(this.GetItem(staticData).key).rewardUserExp * this.itemCnt;
	}
}