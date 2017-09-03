using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
public static partial class FIClientReqHandler{
	static int MAX_ORDER_CNT = 3;
	public static JObject enc_sess_order_getlist(FIFakeContext context){
		var orderList = context.dbContext.GetList<DBOrder>();
		if(orderList.Count <= 0){
			//Create!
			for(int i = 0 ; i < MAX_ORDER_CNT ; i++){
				var singleOrder = context.dbContext.Create<DBOrder>();
				var nameList = context.staticData.GetList<GDOrderNameData>();
				singleOrder.baseID = nameList[UnityEngine.Random.Range(0, nameList.Count)].id;
				InsertUpdated(context, singleOrder);
				AssignOrderRequest(context,singleOrder,true);
			}
		}else{
			InsertUpdated(context,orderList.ToArray());
		}
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_order_accept(FIFakeContext context){
		CheckParameterExists(context,"uid");
		int uid = context.body["uid"].Value<int>();
		if( context.dbContext.ContainsID<DBOrder>(uid) == false ){
			throw new FIException(FIErr.Order_CannotFindData);
		}
		var orderData = context.dbContext.GetByID<DBOrder>(uid);
		//Check its right time..
		if(orderData.waitStartedTime + context.easy.GlobalInfo.orderRegenTime > CurrentTime)
			throw new FIException(FIErr.Order_Cooltime);


		//Check if I have all needs..
		var orderItemList = context.dbContext.GetList<DBOrderItem>().Where(x=>x.orderUID==uid).ToList();
		var myItemList = context.dbContext.GetList<DBItem>();
		var toDisposeList = new List<Tuple<int,int>>();
//		var toGiveList = new List<Tuple<int,int>>();
		int totalGold = 0;
		int totalExp = 0;
		foreach(var item in orderItemList){
			var foundItem = myItemList.Where(x=>x.itemID==item.itemID).FirstOrDefault();
			if(foundItem == null){
				throw new FIException(FIErr.Order_CannotDisposeItemDoesntHave);
			}
			if(foundItem.count < item.itemCnt)
				throw new FIException(FIErr.Order_CannotDisposeItemDoesntHaveEnough);
			toDisposeList.Add(Tuple.Create<int,int>(foundItem.itemID,item.itemCnt));
			totalGold += item.CalcOrderPrice(context.staticData);
			totalExp += item.CalcOrderRewardExp(context.staticData);
		}

		//All checks out. dispose item. and give item..
		AssignOrderRequest(context,orderData,false);
		InsertUpdated(context,orderData);

		Storage_DisposeItem(context,toDisposeList.ToArray());
		Storage_InsertItem(context,
			Tuple.Create<int,int>(GDInstKey.ItemData_goldPoint,totalGold),
			Tuple.Create<int,int>(GDInstKey.ItemData_userExp,totalExp)
		);

		//Achievement..
		Achievement_Increase(context, GDAchievementType.OrderCount, 1);

		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_order_dispose(FIFakeContext context){
		CheckParameterExists(context,"uid");
		int uid = context.body["uid"].Value<int>();
		if( context.dbContext.ContainsID<DBOrder>(uid) == false ){
			throw new FIException(FIErr.Order_CannotFindData);
		}
		var orderData = context.dbContext.GetByID<DBOrder>(uid);
		//Check its right time..
		if(orderData.waitStartedTime + context.easy.GlobalInfo.orderRegenTime > CurrentTime)
			throw new FIException(FIErr.Order_Cooltime);
		
		//All checks out. dispose item. and give item..
		AssignOrderRequest(context,orderData,true);
		InsertUpdated(context,orderData);

		return GetDefaultJObject(context);
	}

	static void AssignOrderRequest(FIFakeContext context, DBOrder order,bool giveDelay){
		//Remove existing items first...
		var existOrderItemList = context.dbContext.GetList<DBOrderItem>().Where(x=>x.orderUID == order.uid).ToList();
		foreach(var item in existOrderItemList){
//			Debug.LogWarning("Found existing item="+item.orderUID+" id="+item.uid);
			context.dbContext.Dispose<DBOrderItem>(item.uid);
		}
		if(existOrderItemList.Count > 0){
			InsertDeleted(context,existOrderItemList.ToArray());
		}

		int availableTypeCnt = UnityEngine.Random.Range(1,4);
		var listOfAvailable = context.staticData.GetList<GDItemData>()
			.Where(x=>{
				if( x.type.IsFlagSet(GDItemDataType.CustomerEat) && x.baseLv <= context.easy.UserInfo.userLv)
					return true;
				return false;
			}).ToList();
		availableTypeCnt = System.Math.Min( availableTypeCnt, listOfAvailable.Count );

		for(int i = 0 ; i < availableTypeCnt ; i++){
			var itemData = context.dbContext.Create<DBOrderItem>();
			itemData.orderUID = order.uid;
			int randNum = Random.Range(0,listOfAvailable.Count);
			itemData.itemID = listOfAvailable[randNum].id;
			itemData.itemCnt = Random.Range(listOfAvailable[randNum].baseReqMin,listOfAvailable[randNum].baseReqMax+1);
			listOfAvailable.RemoveAt( randNum );
			InsertUpdated(context,itemData);
		}

		if(giveDelay == false){
			order.waitStartedTime = CurrentTime - context.easy.GlobalInfo.orderRegenTime;
		}else{
			order.waitStartedTime = CurrentTime;
		}
	}
}