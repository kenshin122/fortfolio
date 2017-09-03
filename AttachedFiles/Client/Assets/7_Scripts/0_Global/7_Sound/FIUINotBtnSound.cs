using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
public class FIUINotBtnSound : MonoBehaviour,IPointerClickHandler{
	public AudioClip clip;
	FIUISoundPlayer uiSoundPlayer;

	[Inject]
	public void Construct(FIUISoundPlayer _uiSoundPlayer){
		uiSoundPlayer = _uiSoundPlayer;
	}
	public void OnPointerClick (PointerEventData eventData){
		uiSoundPlayer.Play(clip);
	}
}
