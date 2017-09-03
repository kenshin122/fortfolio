using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public partial class FIEasy{
	[Inject]
	readonly IRuntimeData runtimeData;
	[Inject]
	readonly GDManager staticData;
	[Inject]
	readonly FIDefaultRequest server;
	[Inject]
	readonly FIPopupManager popupManager;

	public GDManager StaticData{
		get{return staticData;}
	}
	public GDGlobalInfo GlobalInfo{
		get{return staticData.GetSingle<GDGlobalInfo>();}
	}

	public IRuntimeData RuntimeData{
		get{return runtimeData;}
	}

}
