using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UniRx;

public static partial class FIClientReqHandler{
	static int MAX_CUSTOMER_CNT = 3;
	public static JObject enc_sess_customer_getlist(FIFakeContext context){
		var list = context.dbContext.GetList<DBCustomer>();
		if(list.Count <= 0){
			//Create!
			for(int i = 0 ; i < MAX_CUSTOMER_CNT ; i++){
				var single = context.dbContext.Create<DBCustomer>();
				var customerDataList = context.staticData.GetList<GDCustomerData>();
				single.customerID = customerDataList[UnityEngine.Random.Range(0, customerDataList.Count)].id;
				InsertUpdated(context, single);
				AssignCustomerRequest(context,single,true);
			}
		}else{
			InsertUpdated(context,list.ToArray());
		}
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_customer_accept(FIFakeContext context){
		CheckParameterExists(context,"uid");
		int uid = context.body["uid"].Value<int>();
		if( context.dbContext.ContainsID<DBCustomer>(uid) == false ){
			throw new FIException(FIErr.Customer_CannotFindData);
		}
		var customerData = context.dbContext.GetByID<DBCustomer>(uid);
		//Check its right time..
		if(customerData.waitStartedTime + context.easy.GlobalInfo.customerRegenTime > CurrentTime)
			throw new FIException(FIErr.Customer_Cooltime);
		
		//Check if I have all needs..
//		var myItemList = context.dbContext.GetList<DBItem>();
		Tuple<int,int> reqItem = Tuple.Create<int,int>(customerData.itemID,customerData.itemCnt);
		if( Storage_CheckCanDisposeItems(context,reqItem) == false)
			throw new FIException(FIErr.Customer_CannotDisposeItemDoesntHaveEnough);

		var itemData = context.staticData.GetByID<GDItemData>(customerData.itemID);
		int totalGold = itemData.GetCustomerPrice(customerData.itemCnt);
		int totalExp = 3;
		Tuple<int,int>[] rewardItemArr = new Tuple<int, int>[]{
			Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,totalGold),
			Tuple.Create<int,int>(GDInstKey.ItemData_userExp,totalExp)
		};

		//All checks out. dispose item. and give item..
		AssignCustomerRequest(context,customerData,true);
		InsertUpdated(context,customerData);

		Storage_DisposeItem(context,reqItem);
		Storage_InsertItem(context,rewardItemArr);

		//Achievement..
		Achievement_Increase(context, GDAchievementType.CustomerCount, 1);

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_customer_dispose(FIFakeContext context){
		CheckParameterExists(context,"uid");
		int uid = context.body["uid"].Value<int>();
		if( context.dbContext.ContainsID<DBCustomer>(uid) == false ){
			throw new FIException(FIErr.Customer_CannotFindData);
		}
		var customerData = context.dbContext.GetByID<DBCustomer>(uid);
		//Check its right time..
		if(customerData.waitStartedTime + context.easy.GlobalInfo.customerRegenTime > CurrentTime){
			throw new FIException(FIErr.Customer_Cooltime);
		}

		//All checks out. dispose item. and give item..
		AssignCustomerRequest(context,customerData,true);
		InsertUpdated(context,customerData);

		return GetDefaultJObject(context);
	}
	static void AssignCustomerRequest(FIFakeContext context, DBCustomer customer,bool giveDelay){
		
		//First pick which i have..
		List<DBItem> myItems = null;
		myItems = context.dbContext.GetList<DBItem>()
			.Where(x=>x.count>0)
			.Where(x=>{
				var single = context.staticData.GetByID<GDItemData>(x.itemID);
				return single.type.IsFlagSet(GDItemDataType.CustomerEat) == true;
			}).ToList();
		
		if(myItems.Count <= 0){
			//Doesnt have anything.. request for that I can make.
			var listOfAvailable = context.staticData.GetList<GDItemData>()
				.Where(x=>{
					if( x.type.IsFlagSet(GDItemDataType.CustomerEat) && x.baseLv <= context.easy.UserInfo.userLv)
						return true;
					return false;
				}).ToList();
			if(listOfAvailable.Count <= 0)
				throw new FIException(FIErr.Customer_NeedUserCanMakeAtLeastOne);
			int randNum = UnityEngine.Random.Range(0,listOfAvailable.Count);
			customer.itemID = listOfAvailable[randNum].id;
			customer.itemCnt = UnityEngine.Random.Range(1,3+1);
		}else{
			//I have something that i can request!.
			int randNum = UnityEngine.Random.Range(0,myItems.Count);
			customer.itemID = myItems[randNum].itemID;
			int reqCnt = (myItems[randNum].count / 3);
			if(reqCnt <= 0)
				reqCnt = 1;
			customer.itemCnt = reqCnt;
		}

		if(giveDelay == false){
			customer.waitStartedTime = CurrentTime - context.easy.GlobalInfo.customerRegenTime;
		}else{
			customer.waitStartedTime = CurrentTime;
		}
	}
}