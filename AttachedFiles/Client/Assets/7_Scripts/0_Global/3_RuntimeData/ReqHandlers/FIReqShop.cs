using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
public static partial class FIClientReqHandler{
	
	public static JObject enc_sess_shop_buyingredient(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var single = context.staticData.GetByID<GDShopIngredient>(id);

		//Check my level..
		var user = context.dbContext.GetSingle<DBUserInfo>();
		if(user.userLv < single.unlockLv){
			throw new FIException(FIErr.Shop_Ingredient_NotEnoughLevel);
		}

		//Check if i have enough money..
		if( Storage_CheckCanDisposeItems(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold)) == false ){
			throw new FIException(FIErr.Shop_Ingredient_NotEnoughGold);
		}

		//Insert and dispose..
		Storage_DisposeItem(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold));
		Storage_InsertItem(context, Tuple.Create<int,int>(single.item.id,1));


		return GetDefaultJObject(context);
	}

	public static JObject enc_sess_shop_buycraftingtable(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var single = context.staticData.GetByID<GDShopCraftingTable>(id);

		//Check my level..
		var user = context.dbContext.GetSingle<DBUserInfo>();
		if(user.userLv < single.unlockLv){
			throw new FIException(FIErr.Shop_CraftingTable_NotEnoughLevel);
		}

		//Check if i have enough money..
		if( Storage_CheckCanDisposeItems(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold)) == false ){
			throw new FIException(FIErr.Shop_CraftingTable_NotEnoughGold);
		}

		//Check My table is full..
		var listOfTables = context.dbContext.GetList<DBCraftingTable>();
		if(listOfTables.Count >= context.easy.GlobalInfo.totalCraftingTableCnt){
			throw new FIException(FIErr.Shop_CraftingTable_AlreadyFull);
		}

		//Insert and dispose..
		Storage_DisposeItem(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold));

		//Create table..
		var createdTable = context.dbContext.Create<DBCraftingTable>();
		createdTable.craftingTableID = single.table.id;
		createdTable.createdTime = CurrentTime;
		InsertUpdated(context,createdTable);

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_shop_buyinterior(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var single = context.staticData.GetByID<GDShopInterior>(id);

		//Check my level..
		var user = context.dbContext.GetSingle<DBUserInfo>();
		if(user.userLv < single.unlockLv){
			throw new FIException(FIErr.Shop_Interior_NotEnoughLevel);
		}

		//Check if i have enough money..
		if( Storage_CheckCanDisposeItems(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold)) == false ){
			throw new FIException(FIErr.Shop_Interior_NotEnoughGold);
		}

		//Check if i already bought..
		var myInterior = context.dbContext.GetList<DBInterior>()
			.Where(x=>x.interiorID==single.interior.id)
			.FirstOrDefault();
		if(myInterior != null){
			throw new FIException(FIErr.Shop_Interior_AlreadyHas);
		}

		//Dispose..
		Storage_DisposeItem(context, Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,single.reqGold));

		myInterior = context.dbContext.Create<DBInterior>();
		myInterior.interiorID = single.interior.id;
		InsertUpdated(context,myInterior);

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_shop_buyinteriorwithdia(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var single = context.staticData.GetByID<GDShopInterior>(id);

		//Check if i have enough money..
		if( Storage_CheckCanDisposeItems(context, Tuple.Create<int,int>(GDInstKey.ItemData_diaPoint,single.unlockDia)) == false ){
			throw new FIException(FIErr.Shop_Interior_NotEnoughDia);
		}

		//Check if i already bought..
		var myInterior = context.dbContext.GetList<DBInterior>()
			.Where(x=>x.interiorID==single.interior.id)
			.FirstOrDefault();
		if(myInterior != null){
			throw new FIException(FIErr.Shop_Interior_AlreadyHasWithDia);
		}

		//Dispose..
		Storage_DisposeItem(context, Tuple.Create<int,int>(GDInstKey.ItemData_diaPoint,single.unlockDia));

		myInterior = context.dbContext.Create<DBInterior>();
		myInterior.interiorID = single.interior.id;
		InsertUpdated(context,myInterior);

		return GetDefaultJObject(context);
	}
}