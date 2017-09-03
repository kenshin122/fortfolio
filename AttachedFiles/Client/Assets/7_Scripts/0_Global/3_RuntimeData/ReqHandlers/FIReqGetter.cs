using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static partial class FIClientReqHandler{
	public static JObject enc_sess_get_itemlist(FIFakeContext context){
		var itemList = context.dbContext.GetList<DBItem>();
		InsertUpdated(context,itemList.ToArray());
		return GetDefaultJObject(context);
	}

}