using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public enum GrantType{
	DeviceID,
	Facebook,
	Google,
}
//ServerSide
public partial class DBAuthData : FIRBaseData{
	public DBAuthData(){}
	public DBAuthData(GrantType _grantType,string _grantValue){
		this.grantType = _grantType;
		this.grantValue = _grantValue;
	}
	public GrantType grantType{get;set;}
	public string grantValue{get;set;}
}
public partial class DBGlobalData : FIRBaseData {
	public System.DateTime ServerTime{
		get{
			return _serverTime;
		}set{
			_serverTime = value;
			_clientTime = System.DateTime.Now;
		}
	}
	[JsonIgnore]
	private System.DateTime _serverTime;
	[JsonIgnore]
	private System.DateTime _clientTime;
	[JsonIgnore]
	public System.DateTime Now{
		get{
			return _serverTime + (System.DateTime.Now - _clientTime);
		}
	}
}
public partial class DBUserInfo : FIRBaseData{
	public int authUID;
	public string nick;
	public int userLv;
	public int curExp;
	public int missionSlotUpgradeLv;
	public int storageUpgradeLv;
}
public partial class DBItem: FIRBaseData{
//	public GDItemData baseData;
	public int itemID;
	public int count;
}

//Order
public partial class DBOrder: FIRBaseData{
	//	public GDOrderNameData baseData;
	public int baseID;
	public System.DateTime waitStartedTime;
}
public partial class DBOrderItem: FIRBaseData{
	//public DBOrder baseData
	public int orderUID;
	//public GDCostItemInfo itemInfo
	public int itemID;
	public int itemCnt;
}

//Search
public partial class DBSearchItemInfo:FIRBaseData{
	public bool isProcessing;
	public System.DateTime startedTime;
//	public GDItemData foundItem;
	public int foundItemID;
	public int foundCnt;
}

//Customer
public partial class DBCustomer: FIRBaseData{
	//	public GDCustomerData baseData;
	public int customerID;
	public System.DateTime waitStartedTime;
	public int itemID;
	public int itemCnt;
}

//Achievement
public partial class DBAchievementCleared: FIRBaseData{
	public int achievementID;
}
public partial class DBAchievementTypeCount: FIRBaseData{
	public GDAchievementType type;
	public int cnt;
}

//Crafting
public partial class DBCraftingTable:FIRBaseData{
	public int craftingTableID;
	public int upgradeLv;
	public System.DateTime createdTime;
//	public bool isMaking;
	public System.DateTime recipeStartedTime;
}
public partial class DBCraftingItem:FIRBaseData{
	public int tableUID;
//	public int idx;
	public int recipeID;
	public System.DateTime insertedTime;
	public System.TimeSpan reqTime;
}
public partial class DBCraftingRecipeCategoryUnlock:FIRBaseData{
	public int categoryID;
}
public partial class DBCraftingRecipeUnlock:FIRBaseData{
	public int recipeID;
}

//Interior
public partial class DBInterior:FIRBaseData{
	public int interiorID;
}

//ShopItem
public partial class DBShopItemCoolTime:FIRBaseData{
	public int shopItemID;
	public System.DateTime endTime;
}

//LocalTrade
public partial class DBLocalTrade:FIRBaseData{
	public enum State{
		Invalid = -1,
		WaitingToBeFilled,
		WaitingTime,
	}
	public State curState;
	public System.DateTime startedTime;
}
public partial class DBLocalTradeSingle:FIRBaseData{
	public int localTradeUID;
	public int slotIdx;
	public int itemID;
	public int itemCnt;
	public bool isFilled;
	public int rewardID;
	public int rewardCnt;
	public bool isCollected;
}

//GlobalTrade
public partial class DBGlobalTrade:FIRBaseData{
	public bool isProcessing;
	public System.DateTime startedTime;
}
public partial class DBGlobalTradeSingle:FIRBaseData{
	public int idx;
	public GDItemData item;
	public int cnt;
	public bool isFilled;
}










public partial class DBFarmSlotUpgrade:FIRBaseData{
	public bool isUpgrading;
	public int farmSlotUpgradeLv;
	public System.DateTime startedTime;
}
public partial class DBFarmSegmentSlotUpgrade:FIRBaseData{
	public int slotIdx;
	public int upgradeLv;
}
public partial class DBFarmRecipeLearn:FIRBaseData{
//	public GDFarmRecipeData baseData;
	public string baseKey;
	public bool isLearned;
	public System.DateTime startedTime;
}



