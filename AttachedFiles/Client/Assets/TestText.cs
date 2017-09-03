using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestText : MonoBehaviour {
	public FITextPic textPic;
	// Use this for initialization
	void Start () {
		textPic.onHrefClick.AddListener( str=>{
			Debug.Log("Clicked!="+str);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
