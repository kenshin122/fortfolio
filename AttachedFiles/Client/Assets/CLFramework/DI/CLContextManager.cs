using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

//[RequireComponent(typeof(CLCM))]
public class CLContextManager : MonoBehaviour {
	DiContainer container;
	Dictionary<string,System.Type> typeDic;
	[Inject]
	public void Construct(DiContainer _container){
		container = _container;
		typeDic = new Dictionary<string, Type>();
		foreach(var item in Assembly.GetExecutingAssembly().GetTypes()){
			
			var attrib = Attribute.GetCustomAttribute(item,typeof(CLContextAttrib));
			if(attrib == null){
				continue;
			}
//			Debug.Log($"Scene Found={item.Name}");
			typeDic.Add( ((CLContextAttrib)attrib).Name , item );
			foreach(var bindFunc in item.GetMethods(BindingFlags.NonPublic|BindingFlags.Static)){
				bool hasAttrib = false;
				foreach(var funcAttrib in bindFunc.GetCustomAttributes(false)){
					if(funcAttrib.GetType() == typeof(CLContextBindAttrib)){
						hasAttrib = true;
						break;
					}
				}
				if(hasAttrib == true){
//					Debug.Log("Binding..");
					bindFunc.Invoke(null,new object[]{container});
				}
			}
		}

		container.Bind<CLCM>().FromInstance(gameObject.AddComponent<CLCM>()).AsSingle();
	}

	public string startContextName;
	ICLContext startContext;
	void Start(){
		if(useDebug == true)
			return;
		if(startContextName != null){
			startContext = CreateFirstContext(startContextName);
		}
	}
	//Only used for testing purpose..
	ICLContext CreateFirstContext(string name, params object[] args){
		if( typeDic.ContainsKey(name) == false)
			throw new Exception($"There is no context name called {name}");

		var createdContext = (CLContextBase)Activator.CreateInstance(typeDic[name]);
		container.Inject(createdContext);
		createdContext.isTesting = true;
		createdContext.OnGameStartLoaded(()=>{
			createdContext.OnInitialize(args);
		});
		return (ICLContext)createdContext;
	}
	public ICLContext CreateContext(string name, params object[] args){
		return CreateContext(name,null,args);
	}
	public ICLContext CreateContext(string name,System.Action<object[]> returnFunc,params object[] args){
		if( typeDic.ContainsKey(name) == false)
			throw new Exception($"There is no context name called {name}");

		var createdContext = (CLContextBase)Activator.CreateInstance(typeDic[name]);
		container.Inject(createdContext);
		createdContext.returnFunc = returnFunc;
		createdContext.OnInitialize(args);
		return (ICLContext)createdContext;
	}
	public T CreateContext<T>(params object[] args)where T:CLContextBase,new(){
//		return (T)CreateContext(typeof(T).Name,null,args);
		var createdContext = new T();
		container.Inject(createdContext);
//		createdContext.returnFunc = returnFunc;
		createdContext.OnInitialize(args);
		return createdContext;
	}
	public bool useDebug = false;
	public string[] listOfTestable;
	ICLContext currContext;
	void OnGUI(){
		if(useDebug == false)
			return;

		if(currContext != null){
			if( GUI.Button(new Rect(0,0,200,200),"Back") == true){
				currContext.Dispose();
				currContext = null;
			}
		}else{
//			Rect temp = new Rect(300,50);
			int offset = 0;
			for(int i = 0 ; i < listOfTestable.Length ; i++){
				if(GUI.Button(new Rect(0,offset,300,50),listOfTestable[i]) == true){
					currContext = CreateFirstContext(listOfTestable[i]);
				}
				offset += 60;
			}
		}
	}
}
public interface ICLContext{
	void Dispose();
	void DisposeWithReturn(params object[] args);
}
public class CLContextBase:IDisposable,ICLContext{
	[Inject]
	protected CLContextManager cManager;
	[Inject]
	protected DiContainer container;
	public System.Action<object[]> returnFunc;

	//Only used for testing purpose..
	public bool isTesting;
	public virtual void OnGameStartLoaded(System.Action afterInit){}
	public virtual void OnInitialize(params object[] args){}
	public virtual void Dispose(){}
	public void DisposeWithReturn(params object[] args){
		if(returnFunc == null)
			throw new Exception("There is no return method!");
		returnFunc(args);
		Dispose();
	}
}
public class CLContextAttrib:Attribute{
	public string Name;
	public CLContextAttrib(string _name){
		Name = _name;
	}
}
public class CLContextBindAttrib:Attribute{}