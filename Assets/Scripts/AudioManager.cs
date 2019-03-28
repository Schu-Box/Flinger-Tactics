using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	
	public GameObject soundHolderPrefab;

	private Sound ballBumpSound;
	private Sound athleteBumpSound;
	private Sound bumperBumpSound;
	private Sound bumperBreakSound;
	private Sound ballGoalSound;
	private Sound goalSound;
	private Sound goalLightSound;
	private Sound whistleSound;
	private Sound substituteOutSound;
	private Sound substituteInSound;

	void Start() {
		ballBumpSound = Resources.Load<Sound>("Sound Resources/ballBump");
		athleteBumpSound = Resources.Load<Sound>("Sound Resources/athleteBump");
		bumperBumpSound = Resources.Load<Sound>("Sound Resources/bumperBump");
		bumperBreakSound = Resources.Load<Sound>("Sound Resources/bumperBreak");
		ballGoalSound = Resources.Load<Sound>("Sound Resources/ballGoal");
		goalSound = Resources.Load<Sound>("Sound Resources/goal");
		goalLightSound = Resources.Load<Sound>("Sound Resources/goalLight");
		whistleSound = Resources.Load<Sound>("Sound Resources/whistle");
		substituteOutSound = Resources.Load<Sound>("Sound Resources/subOut");
		substituteInSound = Resources.Load<Sound>("Sound Resources/subIn");
	}

	public void PlaySound(string id) {
		PlaySound(GetSound(id));
	}

	public void PlaySound(Sound sound) {
		GameObject obj = Instantiate(soundHolderPrefab, Vector3.zero, Quaternion.identity);
		AudioSource source = obj.GetComponent<AudioSource>();

		source.clip = sound.GetClip();
		source.volume = sound.volume;
		source.pitch = sound.GetPitch();

		source.Play();
	}

	public Sound GetSound(string id) {
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
			case "goalLight":
				sound = goalLightSound;
				break;
			case "subOut":
				sound = substituteOutSound;
				break;
			case "subIn":
				sound = substituteInSound;
				break;

			default:
				Debug.Log("That clip doesn't exist");
				break;
		}

		return sound;
	}
}
