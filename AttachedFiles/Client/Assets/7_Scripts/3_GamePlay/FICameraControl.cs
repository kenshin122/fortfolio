using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class FICameraControl : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler{
	public float Width;
	public Camera gameCamera;
	Vector2 hitPos;
//	void Awake(){
//		gameCamera = GameObject.Find("GameCamera").GetComponent<Camera>();
//	}
	public void OnBeginDrag(PointerEventData eventData){
		hitPos = eventData.position;
	}
	public void OnDrag(PointerEventData eventData){
		//			if(TCTableOrderControl.
//		TCTableOrderControl.SetSelection(null);

		Vector2 curPos = eventData.position;
		float delta = GetPointToWorldPos(hitPos).x - GetPointToWorldPos(curPos).x;
		hitPos = curPos;
		var camPos = gameCamera.transform.localPosition;
		camPos += new Vector3(delta,0,0);
		camPos.x = Mathf.Clamp(camPos.x, 0,Width);
		gameCamera.transform.localPosition = camPos;
//		Debug.Log("CamPos="+gameCamera.transform.localPosition+" Delta="+delta+" after="+camPos);
//		TCScrollTarget.Singleton.Move(delta);
	}
	public void OnEndDrag(PointerEventData eventData){

	}
	Vector3 GetPointToWorldPos(Vector3 point){
		return gameCamera.ScreenToWorldPoint(point);
	}
}