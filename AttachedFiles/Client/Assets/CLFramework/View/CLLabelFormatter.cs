using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CLLabelFormat:MonoBehaviour{
	Text src;
	string origFormat;
	public void Init(){
//		Debug.Log("Called!");
		src = GetComponent<Text>();
		if(src == null){
			throw new System.Exception("CLLabelFormat cannot find Text Component!");
		}
		origFormat = src.text;
	}
	public void SetText(params object[] arg){
		src.text = string.Format(origFormat,arg);
	}
}
//public class CLLabelFormatContext{
//	Dictionary<UnityEngine.UI.Text,CLLabelFormatter> formatter = new Dictionary<UnityEngine.UI.Text, CLLabelFormatter>();
//	public void SetText(UnityEngine.UI.Text textComp,params object[] args){
//		if(formatter.ContainsKey( textComp ) == false){
//			var temp = CLLabelFormatter.CreateFormatter(textComp);
//			formatter.Add( textComp , temp );
//		}
//		formatter[textComp].SetText(args);
//	}
//}
//public class CLLabelFormatter{
//	string origFormat;
//	UnityEngine.UI.Text src;
//	public static CLLabelFormatter CreateFormatter(UnityEngine.UI.Text _src){
//		CLLabelFormatter temp = new CLLabelFormatter();
//		temp.origFormat = _src.text;
//		temp.src = _src;
//		return temp;
//	}
//	public void SetText(params object[] arg){
//		src.text = string.Format(origFormat,arg);
//	}
//}