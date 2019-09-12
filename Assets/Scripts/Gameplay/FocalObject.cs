using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalObject : MonoBehaviour {

	public List<Crowd> watchers = new List<Crowd>();

	public void AddWatcher(Crowd w) {
		watchers.Add(w);
	}

	public void RemoveWatcher(Crowd w) {
		watchers.Remove(w);
	}

	public void TriggerWatchers() {
		for(int i = 0; i < watchers.Count; i++) {
			watchers[i].StartWatching();
		}
	}

	public void StopWatchers() {
		for(int i = 0; i < watchers.Count; i++) {
			watchers[i].StopWatching();
		}
	}

	public void RemoveAllWatchers() {
		
		for(int i = watchers.Count - 1; i >= 0; i--) {
			watchers[i].StopFocusing();
			watchers.Remove(watchers[i]);
		}

		watchers = new List<Crowd>();
	}
}
