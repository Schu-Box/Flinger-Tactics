using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Sound", order = 2)]
public class Sound : ScriptableObject {

	public List<AudioClip> clips;
	public float volume = 0.5f;
	public Vector2 pitchRange = new Vector2(1, 1);

	public AudioClip GetClip() {
		if(clips.Count > 0) {
			return clips[Random.Range(0, clips.Count)];
		} else {
			return null;
		}
	}

	public float GetPitch() {
		return Random.Range(pitchRange.x, pitchRange.y);
	}
}
