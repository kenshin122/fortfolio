using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Newtonsoft.Json.Linq;
public static class FIShare{
	public static Tuple<T,K> ToTuple<T,K>(this JToken obj){
		T item1 = obj["Item1"].Value<T>();
		K item2 = obj["Item2"].Value<K>();
		return Tuple.Create<T,K>(item1,item2);
	}
	public static Tuple<T,K>[] ToTupleArr<T,K>(this JToken obj){
		var arr = obj as JArray;
		List<Tuple<T,K>> list = new List<Tuple<T,K>>();
		foreach(var item in arr){
			list.Add( item.ToTuple<T,K>() );
		}
		return list.ToArray();
	}
}