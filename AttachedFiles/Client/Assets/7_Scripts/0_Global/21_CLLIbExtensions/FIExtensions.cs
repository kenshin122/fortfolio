using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class FIExtensions{
	public static void FISetSlider(this GameObject go,string path,System.TimeSpan passed,System.TimeSpan total){
		float ratio = (float)passed.TotalMilliseconds / (float)total.TotalMilliseconds;
		go.CLGetComponent<Slider>(path).value = ratio;
		var remainingTime = total-passed;
		go.CLSetFormattedText(path+"/Text",(remainingTime.ToShortVersion()));
	}
	public static void FISetSlider(this GameObject go,System.TimeSpan passed,System.TimeSpan total){
		FISetSlider(go,null,passed,total);
	}
	public static string ToShortVersion(this System.TimeSpan src){
		List<string> totalStr = new List<string>();
		src += System.TimeSpan.FromSeconds(1);
		if(src.Days > 0){
			totalStr.Add( string.Format("{0}일",src.Days) );
		}
		if(src.Hours > 0){
			totalStr.Add( string.Format("{0}시간",src.Hours) );
		}
		if(src.Minutes > 0){
			totalStr.Add( string.Format("{0}분",src.Minutes) );
		}
		if(src.Seconds > 0){
			totalStr.Add( string.Format("{0}초",src.Seconds) );
		}

		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		for(int i = 0 ; i < totalStr.Count ; i++){
			builder.Append( totalStr[i] );
			if(i < totalStr.Count-1){
				builder.Append(" ");
			}
		}
		return builder.ToString();
	}
//	public static float CalcRemainingRatio(this System.DateTime started, System.TimeSpan reqTime){
//		System.TimeSpan passed = FIServerTime.Now - started;
//		return (float)started.PassedTimespan().TotalSeconds / (float)reqTime.TotalSeconds;
//	}
}
