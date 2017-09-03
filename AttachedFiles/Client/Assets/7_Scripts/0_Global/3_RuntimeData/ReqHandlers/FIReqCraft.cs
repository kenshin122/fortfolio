using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UniRx;

public static partial class FIClientReqHandler{
	public static JObject enc_sess_craft_getcraftinfo(FIFakeContext context){
		#region Get DBCraftingTable List
		var list = context.dbContext.GetList<DBCraftingTable>();
		if(list.Count <=0){
			var created = context.dbContext.Create<DBCraftingTable>();
			created.craftingTableID = GDInstKey.CraftingTable__lowTable;
			list.Add(created);
		}
		InsertUpdated(context,list.ToArray());
		#endregion
		#region Get DBCraftingRecipeUnlock List
		var unlockedRecipeList = context.dbContext.GetList<DBCraftingRecipeUnlock>();
		if(unlockedRecipeList.Count <= 0){
			var created = context.dbContext.Create<DBCraftingRecipeUnlock>();
			created.recipeID = GDInstKey.CraftRecipeData_craftRecipe0;
		}
		unlockedRecipeList = context.dbContext.GetList<DBCraftingRecipeUnlock>();
		InsertUpdated(context, unlockedRecipeList.ToArray() );
		#endregion
		#region Get DBCraftingItem List
		var makingList = context.dbContext.GetList<DBCraftingItem>();
		if(makingList.Count > 0){
			InsertUpdated(context, makingList.ToArray() );
		}
		#endregion

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_getcategoryinfo(FIFakeContext context){
		//Category
		var list = context.dbContext.GetList<DBCraftingRecipeCategoryUnlock>();
		if(list.Count <= 0){
			//Create one category opened!
			var firstCategory = context.staticData.GetList<GDCraftCategoryInfo>().FirstOrDefault();
			var created = context.dbContext.Create<DBCraftingRecipeCategoryUnlock>();
			created.categoryID = firstCategory.id;
			list.Add(created);
		}
		InsertUpdated(context,list.ToArray());

		//Recipe
		var recipeList = context.dbContext.GetList<DBCraftingRecipeUnlock>();
		if(recipeList.Count <= 0){
			var firstCategory = context.staticData.GetList<GDCraftCategoryInfo>().FirstOrDefault();
			var firstRecipe = context.staticData.GetList<GDCraftRecipeData>()
				.Where(x=>x.category==firstCategory.type)
				.FirstOrDefault();
			var created = context.dbContext.Create<DBCraftingRecipeUnlock>();
			created.recipeID = firstRecipe.id;
			recipeList.Add(created);
		}
		InsertUpdated(context,recipeList.ToArray());
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_unlockcategory(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var staticCategory = context.staticData.GetByID<GDCraftCategoryInfo>(id);

		var runtimeCategory = context.dbContext.GetList<DBCraftingRecipeCategoryUnlock>()
			.Where(x=>x.categoryID==id)
			.FirstOrDefault();
		//Check already unlocked!
		if(runtimeCategory != null){
			throw new FIException(FIErr.Crafting_AlreadyUnlockedCategory);
		}

		//Check level..
		var userInfo = context.dbContext.GetSingle<DBUserInfo>();
		if(userInfo.userLv < staticCategory.unlockLv){
			throw new FIException(FIErr.Crafting_NotEnoughLevelToUnlockCategory);
		}
		//Check Money!
		if(Storage_CheckCanDisposeItems(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,staticCategory.unlockReqGold)) == false){
			throw new FIException(FIErr.Crafting_NotEnoughMoneyToUnlockCategory);
		}

		//Perform action!
		Storage_DisposeItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,staticCategory.unlockReqGold));
		runtimeCategory = context.dbContext.Create<DBCraftingRecipeCategoryUnlock>();
		runtimeCategory.categoryID = id;
		InsertUpdated(context,runtimeCategory);

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_unlockrecipe(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var staticRecipe = context.staticData.GetByID<GDCraftRecipeData>(id);
		var runtimeRecipe = context.dbContext.GetList<DBCraftingRecipeUnlock>().Where(x=>x.recipeID==id).FirstOrDefault();
		var userLv = context.dbContext.GetSingle<DBUserInfo>().userLv;
		var unlockReqLv = staticRecipe.unlockLv;
		var unlockReqGold = staticRecipe.unlockReqGold;
		//Check
		#region Check I already learned recipe
		if(runtimeRecipe != null){
			throw new FIException(FIErr.Crafting_UnlockRecipeAlreadyHave);
		}
		#endregion
		#region Check My level is confirm
		if( userLv < unlockReqLv ){
			throw new FIException(FIErr.Crafting_UnlockRecipeCannotLevelBelow);
		}
		#endregion
		#region Check I have enough gold
		if( Storage_CheckCanDisposeItems(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,unlockReqGold) ) == false){
			throw new FIException(FIErr.Crafting_UnlockRecipeNotEnoughGold);
		}
		#endregion


		//Action
		#region DisposeItem
		Storage_DisposeItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,unlockReqGold));
		#endregion
		#region Insert unlocked data..
		runtimeRecipe = context.dbContext.Create<DBCraftingRecipeUnlock>();
		runtimeRecipe.recipeID = id;
		InsertUpdated(context,runtimeRecipe);
		#endregion

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_selltable(FIFakeContext context){
		CheckParameterExists(context,"uid");
		var uid = context.body["uid"].Value<int>();

		//Check
		#region TableExists..
		var totalTables = context.dbContext.GetList<DBCraftingTable>();
		var runtimeTable = totalTables.Where(x=>x.uid==uid).FirstOrDefault();
		if(runtimeTable == null){
			throw new FIException(FIErr.Crafting_SellTableCannotFindTable);
		}
		#endregion
		#region IsOnlyOneTable? Then dont sell..
		if(totalTables.Count <= 1){
			throw new FIException(FIErr.Crafting_SellTableCannotSellOnlyOneTable);
		}
		#endregion
		#region Check if there is making item..
		var currMakingItemList = context.dbContext.GetList<DBCraftingItem>().Where(x=>x.tableUID==runtimeTable.uid).ToList();
		if(currMakingItemList.Count > 0){
			throw new FIException(FIErr.Crafting_SellTableCannotSellThereIsSomethingMaking);
		}
		#endregion

		//Action
		#region DisposeTable..
		context.dbContext.Dispose<DBCraftingTable>(uid);
		InsertDeleted(context,runtimeTable);
		#endregion
		#region GiveRewards..
		var staticData = context.staticData.GetByID<GDCraftingTable>(runtimeTable.craftingTableID);
//		staticData.sellPrice
		Storage_InsertItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,staticData.sellPrice));
		#endregion

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_insertrecipe(FIFakeContext context){
		CheckParameterExists(context,"tableUID","recipeID");
		var tableUID = context.body["tableUID"].Value<int>();
		var recipeID = context.body["recipeID"].Value<int>();

		var runtimeTable = context.dbContext.GetByID<DBCraftingTable>(tableUID);
		var staticTable = context.staticData.GetByID<GDCraftingTable>(runtimeTable.craftingTableID);

		var staticRecipe = context.staticData.GetByID<GDCraftRecipeData>(recipeID);
		var reqItemArr = staticRecipe.reqItemArr.Select(x=>{
			return Tuple.Create<int,int>(x.item.id,x.cnt);
		}).ToArray();
		var unlockedRecipe = context.dbContext.GetList<DBCraftingRecipeUnlock>()
			.Where(x=>x.recipeID==staticRecipe.id).FirstOrDefault();
		var currentRecipeList = context.dbContext.GetList<DBCraftingItem>()
			.Where(x=>x.tableUID==tableUID)
			.OrderBy(x=>x.insertedTime)
			.ToList();
		
		var availableQueue = staticTable.minSlot+runtimeTable.upgradeLv;
		var completeQueueSize = context.easy.GlobalInfo.totalCraftedCompleteCnt;

		var curServerTime = CurrentTime;
		var estimateStartedTime = runtimeTable.recipeStartedTime;

		//Check
		#region Check I learned recipe!
		if(unlockedRecipe == null){
			throw new FIException(FIErr.Crafting_CannotInsertRecipeNotUnlockedRecipe);
		}
		#endregion
		#region Check has enough resource..
		if(Storage_CheckCanDisposeItems(context,reqItemArr) == false){
			throw new FIException(FIErr.Crafting_CannotInsertRecipeNotEnoughIngredient);
		}
		#endregion
		#region Check i can insert more
		var passedTime = curServerTime - estimateStartedTime;
		bool isUnhold = false;
		var currentMakingItem = FIShareCraftFunc.GetCurrentCookingRecipe(completeQueueSize,passedTime,currentRecipeList,out isUnhold);
		var currentCompleteList = FIShareCraftFunc.GetMadeDishList(completeQueueSize,passedTime,currentRecipeList);
		var realLeftQueue = availableQueue - (currentRecipeList.Count - currentCompleteList.Count);
		if(realLeftQueue <= 0){
			throw new FIException(FIErr.Crafting_CannotInsertRecipeNoRoomForQueue);
		}
		#endregion




		//Action
		#region Reset time
		if(currentRecipeList.Count <= 0){
			estimateStartedTime = curServerTime;
			runtimeTable.recipeStartedTime = estimateStartedTime;
			InsertUpdated(context,runtimeTable);
		}else if(isUnhold == false & currentMakingItem == null){
			estimateStartedTime = curServerTime - FIShareCraftFunc.CalculateFinishedProductTime(currentCompleteList);
			runtimeTable.recipeStartedTime = estimateStartedTime;
			InsertUpdated(context,runtimeTable);
		}

		#endregion
		#region Dispose ReqItemArr
		Storage_DisposeItem(context,reqItemArr);
		#endregion
		#region Insert Recipe
		var created = context.dbContext.Create<DBCraftingItem>();
		created.tableUID = tableUID;
		created.insertedTime = CurrentTime;
		created.recipeID = recipeID;
		created.reqTime = staticRecipe.reqTime;
		InsertUpdated(context,created);
		#endregion
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_finishproductWithDia(FIFakeContext context){
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_craft_collect(FIFakeContext context){
		CheckParameterExists(context,"uidArr");
		var uidArr = context.body["uidArr"].Values<int>().ToArray();
		var recipeList = context.dbContext.GetList<DBCraftingItem>()
			.Where(x=>uidArr.Contains(x.uid))
			.ToList();

		//Check
		#region Check if requesting recipe actually exists..
		if(recipeList.Count<=0)
			throw new FIException(FIErr.Crafting_CollectThereIsNoRecipeMatch);
		#endregion
		#region Check if its from same table..
		int singleUID = -1;
		foreach(var item in recipeList){
			if(singleUID == -1){
				singleUID = item.tableUID;
				continue;
			}
			if(singleUID != item.tableUID){
				throw new FIException(FIErr.Crafting_CollectingNotMatchTableUID);
			}
		}
		#endregion
		#region Check requesting is all completed..
		var runtimeTable = context.dbContext.GetByID<DBCraftingTable>(singleUID);
		var staticTable = context.staticData.GetByID<GDCraftingTable>(runtimeTable.craftingTableID);
		var currentRecipeList = context.dbContext.GetList<DBCraftingItem>()
			.Where(x=>x.tableUID==runtimeTable.uid)
			.OrderBy(x=>x.insertedTime)
			.ToList();
		int completeQueueSize = context.easy.GlobalInfo.totalCraftedCompleteCnt;
		var curServerTime = CurrentTime;
		var estimateStartedTime = runtimeTable.recipeStartedTime;
		var passedTime = curServerTime - estimateStartedTime;
		bool isUnhold = false;
		var currentMakingItem = FIShareCraftFunc.GetCurrentCookingRecipe(completeQueueSize,passedTime,currentRecipeList,out isUnhold);
		var madeList = FIShareCraftFunc.GetMadeDishList(completeQueueSize,passedTime,currentRecipeList);
		foreach(var item in recipeList){
			Debug.Log("ReqItem="+item.uid+" reqTime="+item.reqTime);
		}
		foreach(var item in madeList){
			Debug.Log("MadeItem="+item.uid+" reqTime="+item.reqTime);
		}
		foreach(var item in recipeList){
			if(madeList.Contains(item) == false){
				throw new FIException(FIErr.Crafting_CollectingNotCompletedOne);
			}
		}
		#endregion
		#region Check I can insert reward.
		Dictionary<int,int> totalRewardDic = new Dictionary<int, int>();
		foreach(var item in recipeList){
			var staticSingle = context.staticData.GetByID<GDCraftRecipeData>(item.recipeID);
			foreach(var single in staticSingle.rewardArr){
				if(totalRewardDic.ContainsKey( single.item.id ) == false){
					totalRewardDic.Add(single.item.id, single.cnt);
				}else{
					totalRewardDic[single.item.id]+= single.cnt;
				}
			}
		}
		var totalRewardArr = totalRewardDic.Select(x=>Tuple.Create<int,int>(x.Key,x.Value)).ToArray();
		if( Storage_CheckCanInsertItems(context,totalRewardArr) == false){
			throw new FIException(FIErr.Crafting_CollectingCannotInsertRewards);
		}
		#endregion

		//Action
		#region Reset time of table started!
		if(isUnhold == true || currentMakingItem == null){
			var onlyLeftItemList = madeList
				.Where(x=>recipeList.Contains(x)==false)
				.ToList();
			var totalTime = System.TimeSpan.Zero;
			//Should calculate the only left things..
			foreach(var item in onlyLeftItemList){
				totalTime += item.reqTime;
			}
			runtimeTable.recipeStartedTime = curServerTime - totalTime;
			InsertUpdated(context, runtimeTable);
		}else{
			var totalTime = System.TimeSpan.Zero;
			foreach(var item in recipeList){
				// totalTime
				totalTime += item.reqTime;
			}
			runtimeTable.recipeStartedTime += totalTime;
			InsertUpdated(context, runtimeTable);
		}
		#endregion
		#region RemoveRecipies from list
		InsertDeleted(context,recipeList.ToArray());
		context.dbContext.Dispose(recipeList.ToArray());
		#endregion
		#region InsertRewards!
		Storage_InsertItem(context, totalRewardArr);
		#endregion

		return GetDefaultJObject(context);
	}
}