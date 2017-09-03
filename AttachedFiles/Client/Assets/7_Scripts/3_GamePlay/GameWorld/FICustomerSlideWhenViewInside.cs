using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
public class FICustomerSlideWhenViewInside : MonoBehaviour {
	public float minX;
	public float maxX;
	Transform cameraTrans;
	RectTransform rect;
	ReactiveProperty<bool> onSight = new ReactiveProperty<bool>(false);
	void Awake(){
		rect = GetComponent<RectTransform>();
		onSight.Subscribe(isOnSight=>{
			if(isOnSight == true){
				rect.DOAnchorPosX(0,0.1f);
//				rect.DOLocalMoveX(0+adjustVal,0.1f);
			}else{
				rect.DOAnchorPosX(300,0.1f);
//				rect.DOLocalMoveX(300+adjustVal,0.1f);
			}
		});
	}
	// Update is called once per frame
	void Update () {
		if(cameraTrans == null){
			var go = GameObject.Find("WorldCamera");
			if(go == null)
				return;
			cameraTrans = go.GetComponent<Transform>();
		}
		if(cameraTrans.localPosition.x >= minX &&
			cameraTrans.localPosition.x <= maxX){
			onSight.Value = true;
		}else{
			onSight.Value = false;
		}
	}
}
