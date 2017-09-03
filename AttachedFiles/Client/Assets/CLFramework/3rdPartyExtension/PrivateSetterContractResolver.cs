using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;


public class PrivateSetterContractResolver : DefaultContractResolver
{
	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		var jProperty = base.CreateProperty(member, memberSerialization);
		if (jProperty.Writable)
			return jProperty;

		jProperty.Writable = member.IsPropertyWithSetter();

		return jProperty;
	}
}
internal static class MemberInfoExtensions
{
	internal static bool IsPropertyWithSetter(this MemberInfo member)
	{
		var property = member as PropertyInfo;
		if(property != null){
			return property.GetSetMethod(true) != null;
		}else{
			return false;
		}
		//		return property?.GetSetMethod(true) != null;
	}
}