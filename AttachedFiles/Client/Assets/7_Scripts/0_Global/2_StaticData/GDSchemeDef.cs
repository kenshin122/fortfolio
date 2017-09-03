using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
public partial class GDGlobalInfo:GDDataBase{
	public int diaPerMinute{get;private set;}
	public int tradeServantCnt{get;private set;}
	public int specialCustomerHeartCap{get;private set;}
	public TimeSpan orderRegenTime{get;private set;}
	public TimeSpan customerRegenTime{get;private set;}
	public int totalCraftingTableCnt{get;private set;}
	public int totalCraftedCompleteCnt{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		diaPerMinute = xml.GetIntFromAttrib("diaPerMinute");
		tradeServantCnt = xml.GetIntFromAttrib("tradeServantCnt");
		specialCustomerHeartCap = xml.GetIntFromAttrib("specialCustomerHeartCap");
		orderRegenTime = xml.GetTimeSpanFromAttrib("orderRegenTime");
		customerRegenTime = xml.GetTimeSpanFromAttrib("customerRegenTime");
		totalCraftingTableCnt = xml.GetIntFromAttrib("totalCraftingTableCnt");
		totalCraftedCompleteCnt = xml.GetIntFromAttrib("totalCraftedCompleteCnt");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDUserLvInfo:GDDataBase{
	public int reqExp{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		reqExp = xml.GetIntFromAttrib("reqExp");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDItemPercentData:GDDataBase{
	public int orderPerc{get;private set;}
	public int sellItemPerc{get;private set;}
	public int searchItemPerc{get;private set;}
	public int customerPerc{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		orderPerc = xml.GetIntFromAttrib("orderPerc");
		sellItemPerc = xml.GetIntFromAttrib("sellItemPerc");
		searchItemPerc = xml.GetIntFromAttrib("searchItemPerc");
		customerPerc = xml.GetIntFromAttrib("customerPerc");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public enum GDConditionDataType{
	Invalid,
	UserLvOver,
	CostItem,
	StarPointOver,
}
public partial class GDConditionData:GDDataBase{
	public GDConditionDataType type{get;private set;}
	public string value0{get;private set;}
	public string value1{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDConditionDataType>("type");
		value0 = xml.GetStringFromAttrib("value0");
		value1 = xml.GetStringFromAttrib("value1");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDConditionDataDesc:GDDataBase{
	public GDConditionDataType type{get;private set;}
	public string desc{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDConditionDataType>("type");
		desc = xml.GetStringFromAttrib("desc");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public enum GDActionDataType{
	Invalid,
	InsertItem,
	ConsumeItem,
}
public partial class GDActionData:GDDataBase{
	public GDActionDataType type{get;private set;}
	public string value0{get;private set;}
	public string value1{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDActionDataType>("type");
		value0 = xml.GetStringFromAttrib("value0");
		value1 = xml.GetStringFromAttrib("value1");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDActionDataDesc:GDDataBase{
	public GDActionDataType type{get;private set;}
	public string desc{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDActionDataType>("type");
		desc = xml.GetStringFromAttrib("desc");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDCostItemInfo:GDDataBase{
	public GDItemData item{get;private set;}
	public int cnt{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		cnt = xml.GetIntFromAttrib("cnt");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		item = xml.GetRefFromAttrib<GDItemData>(manager,"item");

	}
}
public enum GDItemDataType{
	Current=1<<0,
	CustomerEat=1<<1,
	Crop=1<<2,
	DropFromMission=1<<3,
	AcquireFromTrade=1<<4,
	NotInv=1<<5,
}
public partial class GDItemData:GDDataBase{
	public GDItemDataType type{get;private set;}
	public string name{get;private set;}
	public string imageName{get;private set;}
	public int diaPrice{get;private set;}
	public int baseGold{get;private set;}
	public int baseLv{get;private set;}
	public int baseReqMin{get;private set;}
	public int baseReqMax{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetFlagFromAttrib<GDItemDataType>("type");
		name = xml.GetStringFromAttrib("name");
		imageName = xml.GetStringFromAttrib("imageName");
		diaPrice = xml.GetIntFromAttrib("diaPrice");
		baseGold = xml.GetIntFromAttrib("baseGold");
		baseLv = xml.GetIntFromAttrib("baseLv");
		baseReqMin = xml.GetIntFromAttrib("baseReqMin");
		baseReqMax = xml.GetIntFromAttrib("baseReqMax");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public enum GDCraftCategory{
	Invalid,
	Bread,
	Japanese,
	Western,
}
public partial class GDCraftRecipeData:GDDataBase{
	public string name{get;private set;}
	public TimeSpan reqTime{get;private set;}
	public GDCraftCategory category{get;private set;}
	public int unlockLv{get;private set;}
	public int unlockReqGold{get;private set;}
	public List<GDCostItemInfo> reqItemArr{get;private set;}
	public List<GDCostItemInfo> rewardArr{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		reqTime = xml.GetTimeSpanFromAttrib("reqTime");
		category = xml.GetEnumFromAttrib<GDCraftCategory>("category");
		unlockLv = xml.GetIntFromAttrib("unlockLv");
		unlockReqGold = xml.GetIntFromAttrib("unlockReqGold");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		reqItemArr = xml.GetRefFromAttribArr<GDCostItemInfo>(manager,"reqItemArr");
		rewardArr = xml.GetRefFromAttribArr<GDCostItemInfo>(manager,"rewardArr");

	}
}
public partial class GDCraftCategoryInfo:GDDataBase{
	public GDCraftCategory type{get;private set;}
	public string imageName{get;private set;}
	public int unlockLv{get;private set;}
	public int unlockReqGold{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDCraftCategory>("type");
		imageName = xml.GetStringFromAttrib("imageName");
		unlockLv = xml.GetIntFromAttrib("unlockLv");
		unlockReqGold = xml.GetIntFromAttrib("unlockReqGold");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDStorageUpgradeData:GDDataBase{
	public int size{get;private set;}
	public List<GDCostItemInfo> reqItemArr{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		size = xml.GetIntFromAttrib("size");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		reqItemArr = xml.GetRefFromAttribArr<GDCostItemInfo>(manager,"reqItemArr");

	}
}
public partial class GDCustomerData:GDDataBase{
	public string name{get;private set;}
	public string image{get;private set;}
	public GDUnlockableContentType unlockableType{get;private set;}
	public List<string> speechArr{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		image = xml.GetStringFromAttrib("image");
		unlockableType = xml.GetEnumFromAttrib<GDUnlockableContentType>("unlockableType");
		speechArr = xml.GetStringFromAttribArr("speechArr");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDTradeMerchantSlotData:GDDataBase{
	public List<GDConditionData> condArr{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		condArr = xml.GetRefFromAttribArr<GDConditionData>(manager,"condArr");

	}
}
public partial class GDTradeMerchantLvData:GDDataBase{
	public TimeSpan returnTime{get;private set;}
	public int reqItemMin{get;private set;}
	public int reqItemMax{get;private set;}
	public int rewardItemMin{get;private set;}
	public int rewardItemMax{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		returnTime = xml.GetTimeSpanFromAttrib("returnTime");
		reqItemMin = xml.GetIntFromAttrib("reqItemMin");
		reqItemMax = xml.GetIntFromAttrib("reqItemMax");
		rewardItemMin = xml.GetIntFromAttrib("rewardItemMin");
		rewardItemMax = xml.GetIntFromAttrib("rewardItemMax");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public enum GDUnlockableContentType{
	Invalid,
	Order,
	GlobalMerchant,
	LocalMerchant,
	SearchItem,
	BingoMerchant,
	MagicBuff,
	Research,
}
public partial class GDEdibleItemData:GDDataBase{
	public int rewardUserExp{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		rewardUserExp = xml.GetIntFromAttrib("rewardUserExp");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDAchievementData:GDDataBase{
	public string name{get;private set;}
	public string image{get;private set;}
	public GDAchievementType reqAchiev{get;private set;}
	public int reqAchievCnt{get;private set;}
	public int rewardDiaCnt{get;private set;}
	public GDAchievementData unlockReq{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		image = xml.GetStringFromAttrib("image");
		reqAchiev = xml.GetEnumFromAttrib<GDAchievementType>("reqAchiev");
		reqAchievCnt = xml.GetIntFromAttrib("reqAchievCnt");
		rewardDiaCnt = xml.GetIntFromAttrib("rewardDiaCnt");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		unlockReq = xml.GetRefFromAttrib<GDAchievementData>(manager,"unlockReq");

	}
}
public partial class GDInterior:GDDataBase{
	public string name{get;private set;}
	public string desc{get;private set;}
	public List<GDBuff> buffArr{get;private set;}
	public string prefab{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		desc = xml.GetStringFromAttrib("desc");
		prefab = xml.GetStringFromAttrib("prefab");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		buffArr = xml.GetRefFromAttribArr<GDBuff>(manager,"buffArr");

	}
}
public partial class GDCraftingTable:GDDataBase{
	public string name{get;private set;}
	public string desc{get;private set;}
	public int minSlot{get;private set;}
	public int maxSlot{get;private set;}
	public List<GDBuff> buffArr{get;private set;}
	public string prefab{get;private set;}
	public int slotExpandReqGold{get;private set;}
	public int sellPrice{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		desc = xml.GetStringFromAttrib("desc");
		minSlot = xml.GetIntFromAttrib("minSlot");
		maxSlot = xml.GetIntFromAttrib("maxSlot");
		prefab = xml.GetStringFromAttrib("prefab");
		slotExpandReqGold = xml.GetIntFromAttrib("slotExpandReqGold");
		sellPrice = xml.GetIntFromAttrib("sellPrice");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		buffArr = xml.GetRefFromAttribArr<GDBuff>(manager,"buffArr");

	}
}
public partial class GDBuff:GDDataBase{
	public GDBuffType type{get;private set;}
	public int value{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDBuffType>("type");
		value = xml.GetIntFromAttrib("value");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDTotalMerchantData:GDDataBase{
	public TimeSpan refreshTime{get;private set;}
	public int maxGivenItemCnt{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		refreshTime = xml.GetTimeSpanFromAttrib("refreshTime");
		maxGivenItemCnt = xml.GetIntFromAttrib("maxGivenItemCnt");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDTotalMerchantSlotData:GDDataBase{
	public int slotCnt{get;private set;}
	public List<GDConditionData> unlockCondArr{get;private set;}
	public int reqDia{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		slotCnt = xml.GetIntFromAttrib("slotCnt");
		reqDia = xml.GetIntFromAttrib("reqDia");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		unlockCondArr = xml.GetRefFromAttribArr<GDConditionData>(manager,"unlockCondArr");

	}
}
public enum GDAchievementType{
	Invalid,
	OrderCount,
	CraftCount,
	CustomerCount,
}
public partial class GDShopIngredient:GDDataBase{
	public GDItemData item{get;private set;}
	public string desc{get;private set;}
	public int unlockLv{get;private set;}
	public int reqGold{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		desc = xml.GetStringFromAttrib("desc");
		unlockLv = xml.GetIntFromAttrib("unlockLv");
		reqGold = xml.GetIntFromAttrib("reqGold");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		item = xml.GetRefFromAttrib<GDItemData>(manager,"item");

	}
}
public partial class GDSearchItemShopTicketData:GDDataBase{
	public string name{get;private set;}
	public GDCostItemInfo reward{get;private set;}
	public int reqDia{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");
		reqDia = xml.GetIntFromAttrib("reqDia");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		reward = xml.GetRefFromAttrib<GDCostItemInfo>(manager,"reward");

	}
}
public partial class GDSearchItemShopData:GDDataBase{
	public TimeSpan restingTime{get;private set;}
	public TimeSpan gettingTime{get;private set;}
	public int gettingCnt{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		restingTime = xml.GetTimeSpanFromAttrib("restingTime");
		gettingTime = xml.GetTimeSpanFromAttrib("gettingTime");
		gettingCnt = xml.GetIntFromAttrib("gettingCnt");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public enum GDBuffType{
	Invalid,
	IncCraftSpd,
}
public enum GDShopTabType{
	Invalid,
	Gold,
	Dia,
	Package,
	Free,
}
public partial class GDShopItem:GDDataBase{
	public GDShopTabType type{get;private set;}
	public string name{get;private set;}
	public string iconImage{get;private set;}
	public string desc{get;private set;}
	public string productID{get;private set;}
	public TimeSpan coolTime{get;private set;}
	public List<GDCostItemInfo> rewardArr{get;private set;}
	public string price{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDShopTabType>("type");
		name = xml.GetStringFromAttrib("name");
		iconImage = xml.GetStringFromAttrib("iconImage");
		desc = xml.GetStringFromAttrib("desc");
		productID = xml.GetStringFromAttrib("productID");
		coolTime = xml.GetTimeSpanFromAttrib("coolTime");
		price = xml.GetStringFromAttrib("price");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		rewardArr = xml.GetRefFromAttribArr<GDCostItemInfo>(manager,"rewardArr");

	}
}
public partial class GDShopInterior:GDDataBase{
	public GDInterior interior{get;private set;}
	public int unlockLv{get;private set;}
	public int unlockDia{get;private set;}
	public int reqGold{get;private set;}
	public string iconImage{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		unlockLv = xml.GetIntFromAttrib("unlockLv");
		unlockDia = xml.GetIntFromAttrib("unlockDia");
		reqGold = xml.GetIntFromAttrib("reqGold");
		iconImage = xml.GetStringFromAttrib("iconImage");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		interior = xml.GetRefFromAttrib<GDInterior>(manager,"interior");

	}
}
public partial class GDShopCraftingTable:GDDataBase{
	public GDCraftingTable table{get;private set;}
	public int unlockLv{get;private set;}
	public int reqGold{get;private set;}
	public string iconImage{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		unlockLv = xml.GetIntFromAttrib("unlockLv");
		reqGold = xml.GetIntFromAttrib("reqGold");
		iconImage = xml.GetStringFromAttrib("iconImage");
		isLinkNeeded = true;

	}
	public override void LoadReferences(GDManager manager,XElement xml){
		table = xml.GetRefFromAttrib<GDCraftingTable>(manager,"table");

	}
}
public partial class GDOrderNameData:GDDataBase{
	public string name{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		name = xml.GetStringFromAttrib("name");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDUnlockableContent:GDDataBase{
	public GDUnlockableContentType type{get;private set;}
	public int reqLv{get;private set;}
	public int reqGold{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDUnlockableContentType>("type");
		reqLv = xml.GetIntFromAttrib("reqLv");
		reqGold = xml.GetIntFromAttrib("reqGold");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public partial class GDAchievementTypeDesc:GDDataBase{
	public GDAchievementType type{get;private set;}
	public string desc{get;private set;}

	public override void LoadFromXml(XElement xml,out bool isLinkNeeded){
		base.LoadFromXml(xml,out isLinkNeeded);
		type = xml.GetEnumFromAttrib<GDAchievementType>("type");
		desc = xml.GetStringFromAttrib("desc");

	}
	public override void LoadReferences(GDManager manager,XElement xml){

	}
}
public static class GDEnumExtension{
	public static bool IsFlagSet(this GDItemDataType current, GDItemDataType value){
		return (current&value)!=0;
	}
	public static GDItemDataType AddFlag(this GDItemDataType current, GDItemDataType value){
		return current | value;
	}
	public static GDItemDataType RemoveFlag(this GDItemDataType current, GDItemDataType value){
		return current & ~value;
	}

}