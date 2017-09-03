using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
public class CLViewManager : MonoBehaviour {
	[Inject]
	DiContainer container;

	[SerializeField]
	Transform[] Layers;
//	HashSet<CLView> hash = new HashSet<CLView>();
	public CLView CreateView(string name,string layer=null){
		if(layer == null)
			layer = Layers[0].name;
		
		var layerTrans = (from item in Layers
			where item.name == layer
			select item).FirstOrDefault();

		if(layerTrans == null)
			throw new System.Exception($"No layer name called={layer}");

		var res = Resources.Load<GameObject>(name);
		var created = GameObject.Instantiate(res,layerTrans,false);
		var view = new CLView(created.GetComponent<Transform>());
//		created.AddComponent<CanvasGroup>();
		container.InjectGameObject(created);
		return view;
	}
}
public class CLView:IDisposable{
	Transform trans;
	CanvasGroup cGroup;
	public CanvasGroup CGroup{
		get{
			return cGroup;
		}
	}
	public Transform Trans{
		get{
			return trans;
		}
	}
	public CLView(Transform _trans){
		trans = _trans;
		cGroup = trans.gameObject.AddComponent<CanvasGroup>();
		Visible = false;
		BlocksRaycast = false;
	}
	public bool Visible{
		get{
			return cGroup.alpha == 0.0f ? false : true;
		}set{
			if(value == true){
				cGroup.alpha = 1.0f;
			}else{
				cGroup.alpha = 0.0f;
			}
		}
	}
	public bool BlocksRaycast{
		get{
			return cGroup.blocksRaycasts;
		}set{
			cGroup.blocksRaycasts = value;
		}
	}
	public bool Interactable{
		get{
			return cGroup.interactable;
		}set{
			cGroup.interactable = value;
		}
	}
	public void Dispose(){
		GameObject.Destroy(trans.gameObject);
	}
	public GameObject CLGetGameObject(string path){
		return trans.gameObject.CLGetGameObject(path);
	}
	public T CLGetComponent<T>(){
		return trans.gameObject.CLGetComponent<T>();
	}
	public T CLGetComponent<T>(string path){
		return trans.gameObject.CLGetComponent<T>(path);
	}
	public IObservable<Unit> CLUpdateAsObservable(string path = null){
		return trans.gameObject.CLUpdateAsObservable(path);
	}
	public IObservable<Unit> CLOnThrottleClickAsObservable(string path = null){
		return trans.gameObject.CLOnThrottleClickAsObservable(path);
	}
	public IObservable<Unit> CLOnClickAsObservable(string path = null){
		return trans.gameObject.CLOnClickAsObservable(path);
	}
	public IObservable<PointerEventData> CLOnPointerClickAsObservable(string path=null){
		return trans.gameObject.CLOnPointerClickAsObservable(path);
	}
	public IObservable<PointerEventData> CLOnBeginDragAsObservable(string path=null){
		return trans.gameObject.CLOnBeginDragAsObservable(path);
	}
	public IObservable<PointerEventData> CLOnDragAsObservable(string path=null){
		return trans.gameObject.CLOnDragAsObservable(path);
	}
	public IObservable<PointerEventData> CLOnEndDragAsObservable(string path=null){
		return trans.gameObject.CLOnEndDragAsObservable(path);
	}
	public CLUnityExtensionToggleGroup CLAddToggleGroup(string path, System.Action<string> del, string initialSelected){
		return trans.gameObject.CLAddToggleGroup(path,del,initialSelected);
	}
	public CLUnityExtensionToggleGroup CLAddToggleGroup(System.Action<string> del, string initialSelected){
		return trans.gameObject.CLAddToggleGroup(del,initialSelected);
	}
	public void CLSetFormattedText(string path, params object[] args){
		trans.gameObject.CLSetFormattedText(path,args);
	}
}
public static class CLGameObjectExtensions{
	public static GameObject CLGetGameObject(this GameObject go, string path){
		return go.transform.FindChild(path).gameObject;
	}
	public static T CLGetComponent<T>(this GameObject go){
		return go.GetComponent<T>();
	}
	public static T CLGetComponent<T>(this GameObject go, string path){
		return go.transform.FindChild(path).GetComponent<T>();
	}
	public static IObservable<Unit> CLUpdateAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return (go.CLGetComponent<ObservableUpdateTrigger>()??go.AddComponent<ObservableUpdateTrigger>()).UpdateAsObservable();
	}
	public static IObservable<Unit> CLOnThrottleClickAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return go.CLGetComponent<Button>().onClick.AsObservable().Throttle(System.TimeSpan.FromMilliseconds(100));
		//		return (go.CLGetComponent<ObservablePointerClickTrigger>()??go.AddComponent<ObservablePointerClickTrigger>()).OnPointerClickAsObservable();
	}
	public static IObservable<Unit> CLOnClickAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return go.CLGetComponent<Button>().onClick.AsObservable();
//		return (go.CLGetComponent<ObservablePointerClickTrigger>()??go.AddComponent<ObservablePointerClickTrigger>()).OnPointerClickAsObservable();
	}
	public static IObservable<PointerEventData> CLOnPointerClickAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return (go.CLGetComponent<ObservablePointerClickTrigger>()??go.AddComponent<ObservablePointerClickTrigger>()).OnPointerClickAsObservable();
	}
	public static IObservable<PointerEventData> CLOnBeginDragAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return (go.CLGetComponent<ObservableBeginDragTrigger>()??go.AddComponent<ObservableBeginDragTrigger>()).OnBeginDragAsObservable();
	}
	public static IObservable<PointerEventData> CLOnDragAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return (go.CLGetComponent<ObservableDragTrigger>()??go.AddComponent<ObservableDragTrigger>()).OnDragAsObservable();
	}
	public static IObservable<PointerEventData> CLOnEndDragAsObservable(this GameObject view, string path = null){
		GameObject go = null;
		if(path == null){
			go = view;
		}else{
			go = view.CLGetGameObject(path);
		}
		return (go.CLGetComponent<ObservableEndDragTrigger>()??go.AddComponent<ObservableEndDragTrigger>()).OnEndDragAsObservable();
	}
	public static CLUnityExtensionToggleGroup CLAddToggleGroup(this GameObject go, string path, System.Action<string> del, string initialSelected){
		var comp = go.CLGetGameObject(path).GetComponent<CLUnityExtensionToggleGroup>();
		if(comp == null)
			comp = go.CLGetGameObject(path).AddComponent<CLUnityExtensionToggleGroup>();
		comp.Init(del,initialSelected);
		return comp;
	}
	public static CLUnityExtensionToggleGroup CLAddToggleGroup(this GameObject go, System.Action<string> del, string initialSelected){
		var comp = go.gameObject.GetComponent<CLUnityExtensionToggleGroup>();
		if(comp == null)
			comp = go.gameObject.AddComponent<CLUnityExtensionToggleGroup>();
		comp.Init(del,initialSelected);
		return comp;
	}
	public static void CLSetFormattedText(this GameObject go, string path, params object[] args){
		GameObject rootGameObject = path == null ? go : go.CLGetGameObject(path);
		var format = rootGameObject.GetComponent<CLLabelFormat>();
		if(format == null){
			format = rootGameObject.AddComponent<CLLabelFormat>();
			format.Init();
		}
		format.SetText(args);
	}
//	public static void SetContextText(this Text comp, CLLabelFormatContext context,params object[] args){
//		context.SetText(comp, args);
//	}


}
public class CLUnityExtensionToggleGroup:MonoBehaviour{
	System.Action<string> del;
	public string Current;
	public T GetCurrent<T>(){
		return (T)System.Enum.Parse(typeof(T),Current);
	}
	public void Refresh(){
		del(Current);
	}
	public void Init(System.Action<string> _del,string initial){
		del = _del;
		foreach(Transform item in transform){
			string name = item.name;
			transform.FindChild(name).gameObject.CLGetGameObject("Selected").SetActive(false);
			item.gameObject.CLOnClickAsObservable().Subscribe(_=>{
				if(Current == name){
					return;
				}
				transform.FindChild(Current).gameObject.CLGetGameObject("Selected").SetActive(false);
				Current = name;
				transform.FindChild(Current).gameObject.CLGetGameObject("Selected").SetActive(true);
				del(Current);
			});
//			item.GetComponent<Toggle>().onValueChanged.AddListener( value=>{
//				if(value == true){
//					Current = name;
//					del(name);
//				}
//			});
		}
		Current = initial;
//		transform.FindChild(initial).GetComponent<Toggle>().isOn = true;
		transform.FindChild(initial).gameObject.CLGetGameObject("Selected").SetActive(true);
		del(Current);
	}
//	IEnumerator First(string initial){
//		transform.FindChild(initial).GetComponent<Toggle>().isOn = true;
//		yield return new WaitForEndOfFrame();
//		transform.FindChild(initial).GetComponent<Toggle>().isOn = true;
//	}
}