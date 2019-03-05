using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	
	public GameObject soundHolderPrefab;

	public Sound ballBumpSound;
	public Sound athleteBumpSound;
	public Sound bumperBumpSound;
	public Sound bumperBreakSound;
	public Sound ballGoalSound;
	public Sound goalSound;
	public Sound whistleSound;

	void Start() {

	}

	public void PlaySound(string id) {
		GameObject obj = Instantiate(soundHolderPrefab, Vector3.zero, Quaternion.identity);
		AudioSource source = obj.GetComponent<AudioSource>();
		Sound sound = null;

		switch(id) {
			case "ballBump":
				sound = ballBumpSound;
				break;
			case "athleteBump":
				sound = athleteBumpSound;
				break;
			case "bumperBump":
				sound = bumperBumpSound;
				break;
			case "bumperBreak":
				sound = bumperBreakSound;
				break;
			case "ballGoal":
				sound = ballGoalSound;
				break;
			case "goal":
				sound = goalSound;
				break;
			case "whistle":
				sound = whistleSound;
				break;

			default:
				Debug.Log("That clip doesn't exist");
				break;
		}

		source.clip = sound.GetClip();
		source.volume = sound.volume;
		source.pitch = sound.GetPitch();

		source.Play();
	}
}
