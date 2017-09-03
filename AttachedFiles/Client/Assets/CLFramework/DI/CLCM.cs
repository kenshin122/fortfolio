using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLCM : MonoBehaviour {
	public void DelayedRun(float afterTime,System.Action onRes){
		StartCoroutine(DelayedRunAsync(afterTime,onRes));
	}
	IEnumerator DelayedRunAsync(float afterTime,System.Action onRes){
		yield return new WaitForSeconds(afterTime);
		onRes();
	}
}
