using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
public static partial class FIClientReqHandler{
	
	public static JObject enc_sess_achievement_getlist(FIFakeContext context){
		var achievementList = context.dbContext.GetList<DBAchievementCleared>();
		var achievementTypeList = context.dbContext.GetList<DBAchievementTypeCount>();

		if(achievementList.Count > 0){
			InsertUpdated(context,achievementList.ToArray());
		}
		if(achievementTypeList.Count > 0){
			InsertUpdated(context,achievementTypeList.ToArray());
		}
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_achievement_collect(FIFakeContext context){
		CheckParameterExists(context,"id");
		var id = context.body["id"].Value<int>();
		var staticData = context.staticData.GetByID<GDAchievementData>(id);

		//Check if this is active achievement..
		if( staticData.unlockReq != null){
			var precedingAchievement = context.dbContext.GetList<DBAchievementCleared>().Where(x=>x.achievementID==staticData.unlockReq.id).FirstOrDefault();
			if(precedingAchievement == null){
				throw new FIException(FIErr.Achievement_PrecedingAchievementNotCleared);
			}
		}

		//Check if its available..
		if(staticData.reqAchiev == GDAchievementType.Invalid){
			throw new FIException(FIErr.Achievement_ThisAchievementIsNotForClear);
		}
		var progress = context.dbContext.GetList<DBAchievementTypeCount>()
			.Where(x=>x.type==staticData.reqAchiev).FirstOrDefault();
		if(progress == null){
			throw new FIException(FIErr.Achievement_CannotFindAchievementTypeCountData);
		}
		if(progress.cnt < staticData.reqAchievCnt){
			throw new FIException(FIErr.Achievement_NotEnoughCnt);
		}

		//Can clear
		var clearedData = context.dbContext.Create<DBAchievementCleared>();
		clearedData.achievementID = staticData.id;
		InsertUpdated(context,clearedData);

		//Insert reward!
		Storage_InsertItem(context,Tuple.Create<int,int>(GDInstKey.ItemData_diaPoint,staticData.rewardDiaCnt));

		return GetDefaultJObject(context);
	}
	static void Achievement_Increase(FIFakeContext context,GDAchievementType type,int count = 1){
		var single = context.dbContext.GetList<DBAchievementTypeCount>()
			.Where(x=>x.type==type).FirstOrDefault();
		if(single == null){
			single = context.dbContext.Create<DBAchievementTypeCount>();
			single.type = type;
		}
		single.cnt += count;
		InsertUpdated(context,single);
	}
}