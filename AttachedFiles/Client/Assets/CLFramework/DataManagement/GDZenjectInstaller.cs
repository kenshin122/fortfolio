using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class GDZenjectInstaller : MonoInstaller {
	public TextAsset textAsset;
	string[] listOfDataName;
	static GDManager manager;
	public override void InstallBindings ()
	{
		if(manager == null){
			manager = new GDManager();
			manager.LoadFromXml(new GDInstCreator(), textAsset.text);
		}
		Container.Bind<GDManager>().FromInstance(manager).AsSingle();
		Container.Resolve<GDManager>();
	}
}
