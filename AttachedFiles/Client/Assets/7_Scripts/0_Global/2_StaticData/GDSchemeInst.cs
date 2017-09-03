public class GDInstCreator:IGDInstCreator{
	public static GDInstCreator Inst{
		get{
			return new GDInstCreator();
		}
	}
	public GDDataBase CreateInstanceBySchemeID(int schemeID){
		switch(schemeID){
		case 1:
			return new GDGlobalInfo();
		case 60:
			return new GDUserLvInfo();
		case 86:
			return new GDItemPercentData();
		case 34:
			return new GDConditionData();
		case 35:
			return new GDConditionDataDesc();
		case 37:
			return new GDActionData();
		case 38:
			return new GDActionDataDesc();
		case 63:
			return new GDCostItemInfo();
		case 40:
			return new GDItemData();
		case 51:
			return new GDCraftRecipeData();
		case 115:
			return new GDCraftCategoryInfo();
		case 59:
			return new GDStorageUpgradeData();
		case 64:
			return new GDCustomerData();
		case 70:
			return new GDTradeMerchantSlotData();
		case 71:
			return new GDTradeMerchantLvData();
		case 73:
			return new GDEdibleItemData();
		case 102:
			return new GDAchievementData();
		case 114:
			return new GDInterior();
		case 113:
			return new GDCraftingTable();
		case 112:
			return new GDBuff();
		case 82:
			return new GDTotalMerchantData();
		case 83:
			return new GDTotalMerchantSlotData();
		case 105:
			return new GDShopIngredient();
		case 88:
			return new GDSearchItemShopTicketData();
		case 89:
			return new GDSearchItemShopData();
		case 108:
			return new GDShopItem();
		case 107:
			return new GDShopInterior();
		case 106:
			return new GDShopCraftingTable();
		case 99:
			return new GDOrderNameData();
		case 101:
			return new GDUnlockableContent();
		case 104:
			return new GDAchievementTypeDesc();

		}
		return null;
	}

	public System.Type SchemeIDToType(int schemeID){
		switch(schemeID){
		case 1:
			return typeof(GDGlobalInfo);
		case 60:
			return typeof(GDUserLvInfo);
		case 86:
			return typeof(GDItemPercentData);
		case 34:
			return typeof(GDConditionData);
		case 35:
			return typeof(GDConditionDataDesc);
		case 37:
			return typeof(GDActionData);
		case 38:
			return typeof(GDActionDataDesc);
		case 63:
			return typeof(GDCostItemInfo);
		case 40:
			return typeof(GDItemData);
		case 51:
			return typeof(GDCraftRecipeData);
		case 115:
			return typeof(GDCraftCategoryInfo);
		case 59:
			return typeof(GDStorageUpgradeData);
		case 64:
			return typeof(GDCustomerData);
		case 70:
			return typeof(GDTradeMerchantSlotData);
		case 71:
			return typeof(GDTradeMerchantLvData);
		case 73:
			return typeof(GDEdibleItemData);
		case 102:
			return typeof(GDAchievementData);
		case 114:
			return typeof(GDInterior);
		case 113:
			return typeof(GDCraftingTable);
		case 112:
			return typeof(GDBuff);
		case 82:
			return typeof(GDTotalMerchantData);
		case 83:
			return typeof(GDTotalMerchantSlotData);
		case 105:
			return typeof(GDShopIngredient);
		case 88:
			return typeof(GDSearchItemShopTicketData);
		case 89:
			return typeof(GDSearchItemShopData);
		case 108:
			return typeof(GDShopItem);
		case 107:
			return typeof(GDShopInterior);
		case 106:
			return typeof(GDShopCraftingTable);
		case 99:
			return typeof(GDOrderNameData);
		case 101:
			return typeof(GDUnlockableContent);
		case 104:
			return typeof(GDAchievementTypeDesc);

		}
		return null;
	}

}
