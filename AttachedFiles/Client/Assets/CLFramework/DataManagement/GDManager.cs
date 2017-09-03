using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading;

public class GDManager{
	
	Dictionary<int,GDDataBase> objectDic;
	Dictionary<System.Type,Dictionary<string,GDDataBase>> keyDic;
	Dictionary<System.Type,object> cachedListDic;
	List<System.Action> PrepareFunctionList = new List<Action>();
	public bool IsLoaded{get;private set;}

	public IGDLoadingAsync LoadFromXmlAsync(IGDInstCreator instCreator, string xmlString){
		var asyncData = new GDLoadingAsync();
		var thread = new Thread(()=>{
			try{
				LoadFromXml(instCreator,xmlString,asyncData);
			}catch( Exception e){
				UnityEngine.Debug.Log(e.Message);
				UnityEngine.Debug.Log(e.StackTrace);
			}
		});
		thread.Start();
		return asyncData;
	}
	public void LoadFromXml(IGDInstCreator instCreator, string xmlString){
		LoadFromXml(instCreator,xmlString,null);
	}
	void LoadFromXml(IGDInstCreator instCreator, string xmlString, GDLoadingAsync loadAsync){
		if(IsLoaded == true){
			return;
		}
		if(loadAsync != null){
			loadAsync.SetDone(false);
			loadAsync.SetPerc(0);
			loadAsync.SetState("Initializing Data");
		}
		XElement xmlData = XElement.Parse(xmlString);
		XElement contentDic = xmlData.Element("contentDic");
		objectDic = new Dictionary<int, GDDataBase>();
		Dictionary<GDDataBase,XElement> objectThatNeedsRefLink = new Dictionary<GDDataBase, XElement>();
		keyDic = new Dictionary<System.Type, Dictionary<string, GDDataBase>>();

		float totalCnt = 0;
		foreach(var item in contentDic.Elements()){
			totalCnt++;
		}
//		float totalCnt = contentDic.Elements().Count;
		float elementInterval = (1.0f / 3.0f) / totalCnt;
		float currentPerc = 0.0f;
		foreach(var item in contentDic.Elements()){
			bool needLink = false;
			int schemeTypeID = System.Convert.ToInt32(item.Attribute("schemeTypeID").Value);
			GDDataBase inst = instCreator.CreateInstanceBySchemeID(schemeTypeID);
			inst.Manager = this;
			if(loadAsync != null){
				
				loadAsync.SetPerc(currentPerc);
			}
			inst.LoadFromXml(item,out needLink);
			objectDic.Add(inst.id,inst);
			if(needLink == true){
				objectThatNeedsRefLink.Add(inst,item);
			}

			if(inst.isStruct == false){
				System.Type schemeType = instCreator.SchemeIDToType(inst.schemeTypeID);
				if(keyDic.ContainsKey( schemeType ) == false){
					keyDic.Add( schemeType, new Dictionary<string, GDDataBase>() );
				}
				var tempKeyDic = keyDic[schemeType];
				if(tempKeyDic.ContainsKey(inst.key) == true){
					throw new Exception("This cannot happen. Same key exists in same scheme!");
				}
				tempKeyDic.Add(inst.key,inst);
			}
			currentPerc += elementInterval;
		}
		if(loadAsync != null){
			loadAsync.SetPerc(currentPerc);
			loadAsync.SetState("Linking Data");
		}
		//Link it
		foreach(var pair in objectThatNeedsRefLink){
			pair.Key.LoadReferences(this, pair.Value);
		}
		cachedListDic = new Dictionary<Type, object>();
		currentPerc += 1.0f/3.0f;

		if(loadAsync != null){
			loadAsync.SetPerc(currentPerc);
			loadAsync.SetState("Preparing Data");
		}
		//Prepare it
		foreach(var tempAction in PrepareFunctionList){
			tempAction();
		}
		currentPerc += 1.0f/3.0f;
		IsLoaded = true;
		if(loadAsync != null){
			loadAsync.SetDone(true);
			loadAsync.SetPerc(currentPerc);
			loadAsync.SetState("Done");
		}
	}
	public T GetSingle<T>()where T:GDDataBase{
		return (T)keyDic[typeof(T)].Single().Value;
	}
	public T GetByID<T>(int id)where T:GDDataBase{
		return (T)objectDic[id];
	}
	public T GetByKey<T>(string key)where T:GDDataBase{
		return (T)keyDic[typeof(T)][key];
	}
	public List<T> GetList<T>()where T:GDDataBase{
		if(cachedListDic.ContainsKey(typeof(T)) == true){
			return (List<T>)cachedListDic[typeof(T)];
		}

		List<T> tempList = new List<T>();
		var itemDic = keyDic[typeof(T)];
		foreach(var pair in itemDic){
			tempList.Add( (T)pair.Value );
		}
		cachedListDic.Add(typeof(T),tempList);
		return tempList;
	}
	public bool ContainsKey<T>(string key)where T:GDDataBase{
		if(keyDic.ContainsKey(typeof(T)) == false)
			return false;
		if(keyDic[typeof(T)].ContainsKey(key) == false)
			return false;
		return true;
	}

}
public interface IGDInstCreator{
	GDDataBase CreateInstanceBySchemeID(int schemeID);
	System.Type SchemeIDToType(int schemeID);
}
public class GDDataBase{
	public GDManager Manager;
	public int id{get;private set;}
	public string key{get;private set;}
	public bool isStruct{get;private set;}
	public int schemeTypeID{get;private set;}
	private bool isLoaded = false;
	public virtual void LoadFromXml(XElement xml,out bool isLinkNeeded){
		if(isLoaded == true){
			throw new Exception("Cannot modify it again!");
		}
		isLoaded = true;
		isLinkNeeded = false;
		id = System.Convert.ToInt32(xml.Attribute("id").Value);
		key = xml.Name.ToString();
		isStruct = System.Convert.ToBoolean(xml.Attribute("isStruct").Value);
		schemeTypeID = System.Convert.ToInt32( xml.Attribute("schemeTypeID").Value );

		//		UnityEngine.Debug.Log("GDDataBase LoadFromXml id="+id);
	}
	public virtual void LoadReferences(GDManager manager,XElement xml){}
}
public class GDNameAttrib : Attribute{
	public string Name;
	public GDNameAttrib(string _name){
		Name = _name;
	}
}
public interface IGDLoadingAsync{
	bool IsDone{get;}
	float Perc{get;}
	string State{get;}
}
public class GDLoadingAsync:IGDLoadingAsync{
	object lockObj = new object();
	bool _isDone;
	float _perc;
	string _state;
	public void SetDone(bool _value){
		lock(lockObj){
			_isDone = _value;
		}
	}
	public void SetPerc(float _value){
		lock(lockObj){
			_perc = _value;
//			UnityEngine.Debug.Log("Setting="+_perc);
		}
	}
	public void SetState(string _value){
		lock(lockObj){
			_state = _value;
		}
	}
	public bool IsDone{
		get{
			bool temp;
			lock(lockObj){
				temp = _isDone;
			}
			return temp;
		}
	}
	public float Perc{
		get{
			float temp;
			lock(lockObj){
				temp = _perc;
			}
			return temp;
		}
	}
	public string State{
		get{
			string temp = null;
			lock(lockObj){
				temp = _state;
			}
			return temp;
		}
	}
}

//	public void LoadFromXml(string name, string xmlString){
//		if(IsLoaded == true){
//			return;
//		}
//		var listOfThings = (from a in AppDomain.CurrentDomain.GetAssemblies()
//			from t in a.GetTypes()
//			where t.IsDefined(typeof(GDNameAttrib),false)
//			select t).ToList();
//
//		System.Type found = null;
//		foreach(var type in listOfThings){
//			GDNameAttrib attrib = (GDNameAttrib)Attribute.GetCustomAttribute( type, typeof(GDNameAttrib));
//			if(attrib.Name == name){
//				found = type;
//				break;
//			}
//		}
//		if(found == null){
//			throw new Exception("Cannot find GDNameAttrib="+name);
//		}
//		var item = (IGDInstCreator)Activator.CreateInstance(found);
//		LoadFromXml(item,xmlString);
//	}