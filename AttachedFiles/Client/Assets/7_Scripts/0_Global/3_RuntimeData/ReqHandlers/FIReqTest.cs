using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static partial class FIClientReqHandler{
	public static JObject enc_sess_inctime(FIFakeContext context){
		CheckParameterExists(context,"time");
		var timespan = context.body["time"].Value<System.TimeSpan>();
		loginTime += timespan;
		return GetDefaultJObject(context);
	}
}
