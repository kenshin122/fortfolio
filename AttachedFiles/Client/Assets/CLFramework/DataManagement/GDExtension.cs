using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
//using CLLib;

public static class GDExtension{
	//String
	public static XElement TryGetAttrib(this XElement xml,string name){
		return xml.Element(name);
	}
	public static string GetStringFromAttrib(this XElement xml,string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return "";
		}
		return value.Value;
	}
	public static List<string> GetStringFromAttribArr(this XElement xml,string name){
		List<string> tempList = new List<string>();
		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( item.Value );
		}
		return tempList;
	}
	//Int
	public static int GetIntFromAttrib(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return 0;
		}
		return System.Convert.ToInt32(value.Value);
	}
	public static List<int> GetIntFromAttribArr(this XElement xml, string name){
		List<int> tempList = new List<int>();
		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( System.Convert.ToInt32(item.Value) );
		}
		return tempList;
	}
	//Float
	public static float GetFloatFromAttrib(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return 0;
		}
		return (float)System.Convert.ToDouble(value.Value);
	}
	public static List<float> GetFloatFromAttribArr(this XElement xml, string name){
		List<float> tempList = new List<float>();
		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( (float)System.Convert.ToDouble(item.Value) );
		}
		return tempList;
	}
	//Bool
	public static bool GetBoolFromAttrib(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return false;
		}
		return System.Convert.ToBoolean(value.Value);
	}
	public static List<bool> GetBoolFromAttribArr(this XElement xml, string name){
		List<bool> tempList = new List<bool>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( System.Convert.ToBoolean(item.Value) );
		}
		return tempList;
	}
	//Timespan
	public static TimeSpan GetTimeSpanFromAttrib(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return TimeSpan.Zero;
		}
		return System.TimeSpan.Parse(value.Value);
	}
	public static List<TimeSpan> GetTimeSpanFromAttribArr(this XElement xml, string name){
		List<TimeSpan> tempList = new List<TimeSpan>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( System.TimeSpan.Parse(item.Value) );
		}
		return tempList;
	}
	//DateTime
	public static DateTime GetDateTimeFromAttrib(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return DateTime.Now;
		}
		return System.Convert.ToDateTime(value.Value);
	}
	public static List<DateTime> GetDateTimeFromAttribArr(this XElement xml, string name){
		List<DateTime> tempList = new List<DateTime>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( System.Convert.ToDateTime(item.Value) );
		}
		return tempList;
	}
	//Flag
	public static T GetFlagFromAttrib<T>(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return default(T);
		}
		return GetFlagFromAttribSingle<T>(value.Value);
	}
	public static T GetFlagFromAttribSingle<T>(string value){
		if(string.IsNullOrEmpty(value) == true){
			return (T)(object)0;
		}
		var names = System.Enum.GetNames(typeof(T));
		var values = System.Enum.GetValues(typeof(T));
		int resVal = 0;
		string[] splitted = value.Split(',');
		foreach(var item in splitted){
			string valueItem = item;
			bool isHas = false;
			for(int i = 0 ; i < names.Length ; i++){
				if(names[i] == valueItem){
					isHas = true;
					//					UnityEngine.Debug.Log("Type="+values.GetValue(i).GetType().Name);
					resVal |= (int)values.GetValue(i);
					break;
				}
			}
			if( isHas == false){
				throw new Exception("Error! there is no enum called="+valueItem);
			}
		}
		return (T)(object)resVal;
	}
	public static List<T> GetFlagFromAttribArr<T>(this XElement xml, string name){
		List<T> tempList = new List<T>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( GetFlagFromAttribSingle<T>(item.Value) );
		}
		return tempList;
	}
	//Enum
	public static T GetEnumFromAttrib<T>(this XElement xml, string name){
		var value = xml.TryGetAttrib(name);
		if(value == null){
			return default(T);
		}
		return (T)System.Enum.Parse(typeof(T),value.Value);
	}
	public static List<T> GetBoolFromAttribArr<T>(this XElement xml, string name){
		List<T> tempList = new List<T>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			tempList.Add( (T)System.Enum.Parse(typeof(T),item.Value) );
		}
		return tempList;
	}
	//Ref
	public static T GetRefFromAttrib<T>(this XElement xml,GDManager manager, string name)where T:GDDataBase{
		var value = xml.TryGetAttrib(name);
		if(value == null){
			throw new Exception("There is no attrib value!");
		}

		int instID = System.Convert.ToInt32( value.Value );
		if(instID == 0){
			return null;
		}
		return manager.GetByID<T>(instID);
	}
	public static List<T> GetRefFromAttribArr<T>(this XElement xml, GDManager manager, string name)where T:GDDataBase{
		List<T> tempList = new List<T>();

		var value = xml.TryGetAttrib(name);
		if(value == null)
			return tempList;

		foreach(var item in value.Elements()){
			int instID = System.Convert.ToInt32( item.Value );
			tempList.Add( manager.GetByID<T>(instID) );
		}
		return tempList;
	}
}
