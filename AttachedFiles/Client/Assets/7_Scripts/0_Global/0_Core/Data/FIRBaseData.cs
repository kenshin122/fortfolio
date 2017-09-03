using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
public interface IRuntimeData{
	T GetSingle<T>()where T:FIRBaseData;
	T GetByID<T>(int id)where T:FIRBaseData;
	List<T> GetList<T>()where T:FIRBaseData;
	bool ContainsID<T>(int id)where T:FIRBaseData;
	IObservable<Tuple<FIRBaseData,FIRBaseData>> OnChangeObserver{get;}
	IObservable<FIRBaseData> GetAfterUpdateObserver();
	IObservable<Unit> GetAfterTotalUpdateObserver();
	IObservable<T> GetAfterUpdateObserver<T>()where T:FIRBaseData;
	IObservable<Tuple<FIRBaseData,FIRBaseData>> GetObserver(System.Type type);
	IObservable<Tuple<T,T>> GetObserver<T>()where T:FIRBaseData;
}
public class FIRData:IRuntimeData{
	public IObservable<Tuple<FIRBaseData,FIRBaseData>> OnChangeObserver{
		get{
			return notifier;
		}
	}
	public IObservable<Tuple<FIRBaseData,FIRBaseData>> GetObserver(System.Type type){
		return notifier.Where(x=>x.Item2.GetType()==type);
	}
	public IObservable<Tuple<T,T>> GetObserver<T>()where T:FIRBaseData{
		return notifier.Where(x=>x.Item2.GetType()==typeof(T)).Select(x=>{
			return Tuple.Create<T,T>( (T)x.Item1, (T)x.Item2 );
		});
	}
	public IObservable<FIRBaseData> GetAfterUpdateObserver(){
		return afterNotifier;
	}
	public IObservable<T> GetAfterUpdateObserver<T>()where T:FIRBaseData{
		return afterNotifier.Where(x=>x.GetType()==typeof(T)).Select(x=>(T)x);
	}
	public IObservable<Unit> GetAfterTotalUpdateObserver(){
		return totalAfterNotifier;
	}

	public Subject<Tuple<FIRBaseData,FIRBaseData>> notifier = new Subject<Tuple<FIRBaseData, FIRBaseData>>();
	public Subject<FIRBaseData> afterNotifier = new Subject<FIRBaseData>();
	public Subject<Unit> totalAfterNotifier = new Subject<Unit>();
	public Dictionary<System.Type,Dictionary<int,FIRBaseData>> objectDic = new Dictionary<Type, Dictionary<int, FIRBaseData>>();
	Dictionary<System.Type,int> typeCounter = new Dictionary<Type, int>();
	Dictionary<int,FIRBaseData> getDic(System.Type type){
		if(objectDic.ContainsKey(type) == false){
			objectDic.Add( type, new Dictionary<int,FIRBaseData>() );
		}
		return objectDic[type];
	}
	Dictionary<int,FIRBaseData> getDic<T>(){
		return getDic(typeof(T));
	}
	public T GetSingle<T>()where T:FIRBaseData{
		return (T)getDic<T>().FirstOrDefault().Value;
	}
	public FIRBaseData GetSingle(System.Type type){
		return getDic(type).FirstOrDefault().Value;
	}
	public T GetByID<T>(int id)where T:FIRBaseData{
		return (T)getDic<T>()[id];
	}
	public FIRBaseData GetByID(System.Type type, int id){
		return getDic(type)[id];
	}
	public Dictionary<int,T> GetDic<T>()where T:FIRBaseData{
		Dictionary<int,T> dic = new Dictionary<int, T>();
		foreach(var pair in getDic<T>()){
			dic.Add(pair.Key,(T)pair.Value);
		}
		return dic;
	}
	public List<T> GetList<T>()where T:FIRBaseData{
		List<T> listOfItem = new List<T>();
		foreach(var item in getDic<T>()){
			listOfItem.Add( (T)item.Value );
		}
		return listOfItem;
	}
	public List<FIRBaseData> GetList(System.Type type){
		return (from item in getDic(type)
			select item.Value).ToList();
	}
	public bool ContainsID<T>(int id)where T:FIRBaseData{
		return getDic<T>().ContainsKey(id);
	}
	public bool ContainsID(System.Type type, int id){
		return getDic(type).ContainsKey(id);
	}
	public int IncreaseIDCnt<T>(){
		return IncreaseIDCnt(typeof(T));
	}
	public int IncreaseIDCnt(System.Type type){
		if( typeCounter.ContainsKey(type) == false){
			typeCounter.Add( type, 0);
			return 0;
		}
		typeCounter[type] = typeCounter[type] + 1;
		return typeCounter[type];
	}
	int GetCounterCnt<T>(){
		return GetCounterCnt(typeof(T));
	}
	int GetCounterCnt(System.Type type){
		if(typeCounter.ContainsKey(type) == false){
			return 0;
		}
		return typeCounter[type];
	}
	public T Create<T>()where T:FIRBaseData,new(){
		var dic = getDic<T>();
		int id = IncreaseIDCnt<T>();
		T created = new T();
		created.uid = id;
		dic.Add(id,created);
		return created;
	}
	public FIRBaseData Create(System.Type type){
		var dic = getDic(type);
		int id = IncreaseIDCnt(type);
		var created = (FIRBaseData)Activator.CreateInstance(type);
		created.uid = id;
		dic.Add(id,created);
		return created;
	}
	public void InsertWithID(FIRBaseData created){
		var type = created.GetType();
		var dic = getDic(type);
		int id = IncreaseIDCnt(type);
		created.uid = id;
		dic.Add(id,created);
	}
	public void Dispose(params FIRBaseData[] args){
		if(args.Length<=0)
			throw new Exception("Your trying to dispose empty!");
		foreach(var item in args){
			Dispose( item.GetType(),item.uid );
		}
	}
	public void Dispose(System.Type type, int uid){
		var dic = getDic(type);
		dic.Remove(uid);
	}
	public void Dispose<T>(int uid){
		var dic = getDic<T>();
		dic.Remove(uid);
	}
	public JObject SerializeToJson(){
		JObject root = new JObject();
		foreach(var item in this.objectDic){
			JArray instArr = new JArray();
			JProperty arrayProp = new JProperty("instArr",instArr);
			JProperty classProp = new JProperty("name",item.Key.Name);
			JProperty instCounterProp = new JProperty("cnt",GetCounterCnt(item.Key));
			JObject obj = new JObject( classProp, instCounterProp, arrayProp );
			foreach(var inst in item.Value){
				instArr.Add( JObject.FromObject( inst.Value ) );
			}
			root.Add(obj);
		}
		return root;
	}
	public void DeserializeFromJson(FIRBaseDataTypeContainer dataTypeContainer,JObject json){
		this.objectDic.Clear();
		this.typeCounter.Clear();
		foreach(var classProp in json.Properties()){
			var className = (classProp.Value as JObject)["name"].Value<string>();
			var cnt = (classProp.Value as JObject)["cnt"].Value<int>();
			var instArr = (classProp.Value as JObject)["instArr"].Value<JArray>();
			System.Type typeObj = dataTypeContainer.TypeDic[ className ];

			this.typeCounter.Add(typeObj,cnt);

			var dic = new Dictionary<int,FIRBaseData>();
			foreach(JObject inst in instArr){
				var created = (FIRBaseData)inst.ToObject(typeObj);
				dic.Add(created.uid,created);
			}
			this.objectDic.Add(typeObj,dic);
		}
	}
}
public class FIRBaseData{
	public int uid;
}
public class FIRBaseDataTypeContainer{
	public Dictionary<string,System.Type> TypeDic;
	public FIRBaseDataTypeContainer(){
		TypeDic = new Dictionary<string, System.Type>();
		foreach(var item in Assembly.GetExecutingAssembly().GetTypes()){
			if(item.IsSubclassOf(typeof(FIRBaseData)) == true){
				TypeDic.Add(item.Name,item);
//				UnityEngine.Debug.Log("Type Added="+item.Name);
			}
		}
	}
}
