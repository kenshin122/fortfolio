using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UniRx;

public static partial class FIClientReqHandler{
	static void User_InsertExp(FIFakeContext context, int totalExp){
		var userInfo = context.dbContext.GetSingle<DBUserInfo>();
		var lvInfoList = context.staticData.GetList<GDUserLvInfo>();
		if(userInfo.userLv + 1 >= lvInfoList.Count){
			//Max level! no need exp..
			return;
		}
		var lvInfo = lvInfoList[userInfo.userLv+1];
		userInfo.curExp += totalExp;
		if(userInfo.curExp >= lvInfo.reqExp){
			userInfo.curExp-=lvInfo.reqExp;
			userInfo.userLv++;
		}
		InsertUpdated(context,userInfo);
	}
}