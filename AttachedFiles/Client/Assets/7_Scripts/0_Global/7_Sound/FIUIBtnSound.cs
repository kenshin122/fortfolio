using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
//using UnityEngine.EventSystems;
public class FIUIBtnSound : MonoBehaviour{
	public AudioClip clip;
	FIUISoundPlayer uiSoundPlayer;

	[Inject]
	public void Construct(FIUISoundPlayer _uiSoundPlayer){
		uiSoundPlayer = _uiSoundPlayer;
		GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(()=>{
			uiSoundPlayer.Play(clip);
		}));
	}
}
