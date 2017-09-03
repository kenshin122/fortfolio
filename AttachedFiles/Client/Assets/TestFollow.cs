using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TestFollow : MonoBehaviour {
	public Transform trans;
	// Use this for initialization
	void OnGUI(){
		if( GUI.Button(new Rect(300,300,100,100),"Click")){
			GetComponent<RectTransform>().DOLocalMoveX(0,1);
		}
		if( GUI.Button(new Rect(300,400,100,100),"Again")){
			GetComponent<RectTransform>().DOLocalMoveX(300,1);
		}
	}
}
