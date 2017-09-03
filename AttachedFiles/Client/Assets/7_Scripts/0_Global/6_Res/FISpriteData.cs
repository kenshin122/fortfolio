using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//[CreateAssetMenu]
public class FISpriteData : MonoBehaviour {
	public Sprite[] item;
	public Sprite[] icon;
	public Sprite[] people;
	public Sprite[] tPeople;

	Dictionary<string,Sprite> itemDic;
	public Sprite GetItem(string key){
		if(itemDic == null){
			itemDic = new Dictionary<string, Sprite>();
			foreach(var single in item){
				itemDic.Add(single.name,single);
			}
		}
		if(itemDic.ContainsKey(key) == false)
			return null;
		return itemDic[key];
	}
	Dictionary<string,Sprite> iconDic;
	public Sprite GetIcon(string key){
		if(iconDic == null){
			iconDic = new Dictionary<string, Sprite>();
			foreach(var single in icon){
				iconDic.Add(single.name,single);
			}
		}
		if(iconDic.ContainsKey(key) == false)
			return null;
		return iconDic[key];
	}
	Dictionary<string,Sprite> peopleDic;
	public Sprite GetPeople(string key){
		if(peopleDic == null){
			peopleDic = new Dictionary<string, Sprite>();
			foreach(var single in people){
				peopleDic.Add(single.name,single);
			}
		}
		if(peopleDic.ContainsKey(key) == false)
			return null;
		return peopleDic[key];
	}
	Dictionary<string,Sprite> tPeopleDic;
	public Sprite GetThumbPeople(string key){
		if(tPeopleDic == null){
			tPeopleDic = new Dictionary<string, Sprite>();
			foreach(var single in tPeople){
				tPeopleDic.Add(single.name,single);
			}
		}
		if(tPeopleDic.ContainsKey(key) == false)
			return null;
		return tPeopleDic[key];
	}



	List<Tuple<string,Sprite>> cached;
	public List<Tuple<string,Sprite>> GetListOfSpriteForTextPic(){
		if(Application.isEditor == false){
			if(cached != null)
				return cached;
		}

		cached = new List<Tuple<string,Sprite>>();
		foreach(var single in item){
			cached.Add(Tuple.Create<string,Sprite>(string.Format("item_{0}",single.name),single));
		}
		foreach(var single in icon){
			cached.Add(Tuple.Create<string,Sprite>(string.Format("icon_{0}",single.name),single));
		}
		foreach(var single in people){
			cached.Add(Tuple.Create<string,Sprite>(string.Format("people_{0}",single.name),single));
		}
		foreach(var single in tPeople){
			cached.Add(Tuple.Create<string,Sprite>(string.Format("tPeople_{0}",single.name),single));
		}
		return cached;
	}
}
