using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using CLLib.Thread;
[RequireComponent(typeof(ZenjectBinding))]
public class CLThreadSchedulerMono : MonoBehaviour {
	CLThreadScheduler scheduler = new CLThreadScheduler();
	// Update is called once per frame
	public void Queue(Action<object> del){
		scheduler.Queue (del, System.DateTime.Now - System.TimeSpan.FromSeconds (2));
	}
	public void Queue(Action<object> del,System.TimeSpan _span){
		scheduler.Queue (del, System.DateTime.Now + _span);
	}
	void Update () {
		scheduler.Process();
	}
}
