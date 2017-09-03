using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class FIMonoBehaviour : MonoBehaviour {
	protected DiContainer container;
	protected virtual void Awake(){
		container = GameObject.Find("SceneContext").GetComponent<SceneContext>().Container;
	}
}
