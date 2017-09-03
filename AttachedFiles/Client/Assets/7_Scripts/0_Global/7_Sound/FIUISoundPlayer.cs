using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIUISoundPlayer : MonoBehaviour {
	public void Play(AudioClip clip){
		StartCoroutine(PlayAndDestroy(clip));
	}
	IEnumerator PlayAndDestroy(AudioClip clip){
		var src = new GameObject();
		src.transform.parent = transform;
		var source = src.AddComponent<AudioSource>();
		source.clip = clip;
		source.Play();
		yield return new WaitForSeconds(clip.length);
		GameObject.Destroy(src);
	}
}
