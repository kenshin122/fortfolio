using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CLScrollView{
	GameObject go;
	GameObject sample;
	List<GameObject> createdList;
	System.Action<int,GameObject> itemDelegate;
	System.Action<int,GameObject> itemUpdateDelegate;
	System.Func<int> itemCntDelegate;
	public event System.Action OnRefreshFinished;
	bool isReverse = false;
	DiContainer container;
	public void Init(GameObject _go, System.Action<int,GameObject> _itemDelegate, System.Func<int> _itemCntDelegate, System.Action<int,GameObject> _itemUpdateDelegate = null, bool _isReverse = false){
		go = _go;
		itemDelegate = _itemDelegate;
		itemCntDelegate = _itemCntDelegate;
		itemUpdateDelegate = _itemUpdateDelegate;
		isReverse = _isReverse;
		createdList = new List<GameObject>();
		sample = _go.CLGetGameObject("Sample");
		sample.SetActive(false);
	}

	public void InitWithZenject(DiContainer _container,GameObject _go, System.Action<int,GameObject> _itemDelegate, System.Func<int> _itemCntDelegate, System.Action<int,GameObject> _itemUpdateDelegate = null, bool _isReverse = false){
		container = _container;
		Init(_go,_itemDelegate,_itemCntDelegate,_itemUpdateDelegate,_isReverse);
	}
	public void OnRefresh(){
		for(int i = 0 ; i < createdList.Count ; i++){
			GameObject.Destroy(createdList[i]);
		}
		createdList.Clear();

		sample.SetActive(true);
		int itemCnt = itemCntDelegate();
		if(isReverse == false){
			for(int i = 0 ; i < itemCnt ; i++){
				GameObject obj = GameObject.Instantiate(sample,go.transform,false);
				if(container != null){
					container.InjectGameObject(obj);
				}
//				CLTools.AttachToParent(go.transform,obj.transform);
				itemDelegate(i,obj);
				createdList.Add(obj);
			}	
		}else{
			for(int i = itemCnt-1 ; i >= 0 ; i--){
//				GameObject obj = GameObject.Instantiate(sample);
				GameObject obj = GameObject.Instantiate(sample,go.transform,false);
				if(container != null){
					container.InjectGameObject(obj);
				}
//				CLTools.AttachToParent(go.transform,obj.transform);
				itemDelegate(i,obj);
				createdList.Add(obj);
			}
		}

		sample.SetActive(false);
		if(OnRefreshFinished != null)
			OnRefreshFinished.Invoke();
	}
	public GameObject this[int index]{
		get{
			return createdList[index];
		}
	}
	public List<GameObject> CreatedList{
		get{
			return createdList;
		}
	}
	public void Update(){
		if(itemUpdateDelegate == null)
			return;

		for(int i = 0 ; i < createdList.Count ; i++){
			itemUpdateDelegate(i, createdList[i]);
		}
	}
}