using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIBuildOptions : MonoBehaviour {
	[Header("Global")]
	[Tooltip("Indicates package name of this application. used in in app purchase.")]
	public string packageName;
	[Space(1)]


	[Header("Network")]
	public bool showHttpReqResult = true;
	[Tooltip("Loads GDManager content from network or local. If its local.. It reads from Resources/fidata.xml")]
	public bool loadContentFromNetwork = false;
	public bool useLogin = false;
	[Tooltip("FIClientDataProvider uses this filename")]
	public string clientDataFileName;
	[Space(1)]

	[Header("Testing FakeServer")]
	public float fakeDelay = 0.001f;
	[Tooltip("FIFakeServerDataProvider saves his fake srcContainer file to disc")]
	public bool fakeSaveServerData;
	[Tooltip("FIFakeServerDataProvider uses this filename")]
	public string fakeServerFileName = "data.json";
	[Space(1)]

	[Header("Etc")]
	public float loadingThreshold = 0.2f;
	public FIHttp.FIHttpSettings httpSettings;
//	[Space(10)]
}