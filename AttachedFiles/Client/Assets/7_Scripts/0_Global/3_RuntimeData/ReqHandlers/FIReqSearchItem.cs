using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;

public static partial class FIClientReqHandler{
	public static JObject enc_sess_searchitem_getinfo(FIFakeContext context){
		var searchInfo = context.dbContext.GetList<DBSearchItemInfo>().FirstOrDefault();
		if(searchInfo == null){
			searchInfo = context.dbContext.Create<DBSearchItemInfo>();
			searchInfo.isProcessing = false;
			searchInfo.startedTime = System.DateTime.Now;
			searchInfo.foundItemID = -1;
			searchInfo.foundCnt = -1;
		}
		InsertUpdated(context,searchInfo);
		return GetDefaultJObject(context);
	}
	public static JObject enc_sess_searchitem_search(FIFakeContext context){
		CheckParameterExists(context,"itemID");

		return GetDefaultJObject(context);
	}
}
