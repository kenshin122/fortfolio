using System;
using System.Collections.Generic;

namespace CLLib.Thread
{
	public class CLThreadScheduler{
		object parent;
		string lockObj = "";
		List<Action<object>> actionList = new List<Action<object>>();
		List<System.DateTime> dateList = new List<DateTime>();
		public void Init(object _parent){
			parent = _parent;
		}
		public void Queue(Action<object> del){
			Queue (del, System.DateTime.Now - System.TimeSpan.FromSeconds (2));
		}
		public void Queue(Action<object> del,System.TimeSpan _span){
			Queue (del, System.DateTime.Now + _span);
		}
		public void Queue(Action<object> del,System.DateTime _date){
			lock (lockObj) {
				actionList.Add (del);
				if (_date != default(System.DateTime)) {
					dateList.Add(_date);
				}
			}
		}
		public void Process(){
			lock (lockObj) {
				int idx = 0;
				while (idx < actionList.Count) {
					if (dateList[idx] <= System.DateTime.Now) {
						Action<object> obj = actionList [idx];
						actionList.RemoveAt (idx);
						dateList.RemoveAt (idx);
						obj (parent);
					} else {
						idx++;
					}
				}
			}
		}
	}
}

