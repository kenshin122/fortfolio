using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
//using UniRx;

[CLContextAttrib("Title")]
public partial class FISceneTitle : CLSceneContext{
	[Inject]
	readonly GDManager data;
	[Inject]
	readonly FIBuildOptions buildOption;
	[Inject]
	readonly FIDefaultRequest server;
	public override void OnGameStartLoaded (Action afterInit)
	{
		base.OnGameStartLoaded (afterInit);
		afterInit();
	}
	public override void OnInitialize (params object[] args)
	{
		base.OnInitialize(args);
		this.LoadDefaultView();
		container.Resolve<FIRBaseDataTypeContainer>();
		//Load once!

		//LoadContents..
		StartCoroutine(OnContentLoadProcess());
	}
}
