using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
public static class GDCostItemExtension{
	public static Tuple<int,int>[] GetAsTupleArr(this List<GDCostItemInfo> itemArr){
		return itemArr.GetAsTupleList().ToArray();
	}
	public static List<Tuple<int,int>> GetAsTupleList(this List<GDCostItemInfo> itemArr){
		var dic = new Dictionary<int,int>();
		foreach(var item in itemArr){
			if(dic.ContainsKey(item.item.id) == false){
				dic.Add( item.item.id, item.cnt );
			}else{
				dic[item.item.id]+= item.cnt;
			}
		}
		return dic.Select(x=>Tuple.Create<int,int>(x.Key,x.Value)).ToList();
	}

}
