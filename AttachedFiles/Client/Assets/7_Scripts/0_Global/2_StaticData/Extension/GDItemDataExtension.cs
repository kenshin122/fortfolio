using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GDItemData : GDDataBase{
	public int GetSellPrice(int count = 1){
		var percData = Manager.GetSingle<GDItemPercentData>();
		float sellPrice = (float)this.baseGold + (float)this.baseGold * ( (float)percData.sellItemPerc / 100.0f );
		return (int)( sellPrice * count );
	}
	public int GetOrderPrice(int count = 1){
		var percData = Manager.GetSingle<GDItemPercentData>();
		float sellPrice = (float)this.baseGold + (float)this.baseGold * ( (float)percData.orderPerc / 100.0f );
		return (int)( sellPrice * count );
	}
	public int GetCustomerPrice(int count = 1){
		var percData = Manager.GetSingle<GDItemPercentData>();
		float sellPrice = (float)this.baseGold + (float)this.baseGold * ( (float)percData.customerPerc / 100.0f );
		return (int)( sellPrice * count );
	}
}