using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour {

	private BoxCollider2D box;

	private List<float> orderedHeights = new List<float>();

	public void SetSpawnBox(int numSpawns) {
		box = GetComponent<BoxCollider2D>();
		
		List<float> unorderedHeights = new List<float>();
		float yUnitLength = box.size.y / numSpawns;

		for(int i = 0; i < numSpawns; i++) {
			unorderedHeights.Add((box.size.y / 2) - (yUnitLength / 2) - (yUnitLength * i));
		}

		for(int i = 0; i < numSpawns; i++) {
			if(unorderedHeights.Count == 0) {
				break;
			} else {
				if(unorderedHeights.Count % 2 == 1) {
					float spawnHeight = unorderedHeights[unorderedHeights.Count / 2];
					orderedHeights.Add(spawnHeight);
					unorderedHeights.Remove(spawnHeight);
				} else {
					float spawnHeight1 = unorderedHeights[unorderedHeights.Count / 2];
					float spawnHeight2 = unorderedHeights[(unorderedHeights.Count / 2) - 1];
					orderedHeights.Add(spawnHeight1);
					orderedHeights.Add(spawnHeight2);

					unorderedHeights.Remove(spawnHeight1);
					unorderedHeights.Remove(spawnHeight2);
				}
			}
		}

		/*
		for(int i = 0; i < orderedHeights.Count; i++) {
			Debug.Log(orderedHeights[i]);
		}
		*/
	}

	public List<float> GetSpawnHeights() {
		return orderedHeights;
	}

	public void Disable() {
		box.enabled = false;
	}

	public void Enable() {
		box.enabled = true;
	}
}
