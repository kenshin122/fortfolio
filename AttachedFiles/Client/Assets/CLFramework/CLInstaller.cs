using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(CLContextManager))]
[RequireComponent(typeof(CLViewManager))]
public class CLInstaller : MonoInstaller {
//	public Canvas canvas;
	public override void InstallBindings ()
	{
//		base.InstallBindings ();
		Container.Bind<CLContextManager>().FromInstance(this.GetComponent<CLContextManager>()).AsSingle();
		Container.Bind<CLViewManager>().FromInstance(this.GetComponent<CLViewManager>()).AsSingle();
	}
}
