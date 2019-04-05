using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	
	public GameObject soundHolderPrefab;

	private Sound ballSpawnSound;
	private Sound ballBumpSound;
	private Sound athleteBumpSound;
	private Sound bumperBumpSound;
	private Sound bumperBreakSound;
	private Sound ballGoalSound;
	private Sound goalSound;
	private Sound goalLightSound;
	private Sound goalHornSound;
	private Sound whistleSound;
	private Sound substituteOutSound;
	private Sound substituteInSound;
	private Sound matchOverLightSound;
	private Sound matchOverSound;
	private Sound lastTurnSound;

	void Start() {
		ballSpawnSound = Resources.Load<Sound>("Sound Resources/ballSpawn");
		ballBumpSound = Resources.Load<Sound>("Sound Resources/ballBump");
		athleteBumpSound = Resources.Load<Sound>("Sound Resources/athleteBump");
		bumperBumpSound = Resources.Load<Sound>("Sound Resources/bumperBump");
		bumperBreakSound = Resources.Load<Sound>("Sound Resources/bumperBreak");
		ballGoalSound = Resources.Load<Sound>("Sound Resources/ballGoal");
		goalSound = Resources.Load<Sound>("Sound Resources/goal");
		goalLightSound = Resources.Load<Sound>("Sound Resources/goalLight");
		goalHornSound = Resources.Load<Sound>("Sound Resources/goalHorn");
		whistleSound = Resources.Load<Sound>("Sound Resources/whistle");
		substituteOutSound = Resources.Load<Sound>("Sound Resources/subOut");
		substituteInSound = Resources.Load<Sound>("Sound Resources/subIn");
		matchOverLightSound = Resources.Load<Sound>("Sound Resources/matchOverLight");
		matchOverSound = Resources.Load<Sound>("Sound Resources/matchOver");
		lastTurnSound = Resources.Load<Sound>("Sound Resources/lastTurn");
	}

	public void PlaySound(string id) {
		PlaySound(GetSound(id), 1f);
	}

	public void PlaySound(string id, float modifier) {
		PlaySound(GetSound(id), modifier);
	}

	public void PlaySound(Sound sound, float pitchModifier) {
		GameObject obj = Instantiate(soundHolderPrefab, Vector3.zero, Quaternion.identity);
		AudioSource source = obj.GetComponent<AudioSource>();

		source.clip = sound.GetClip();
		source.volume = sound.volume;
		source.pitch = sound.GetPitch();
		source.pitch *= pitchModifier;

		source.Play();
	}

	public Sound GetSound(string id) {
		Sound sound = null;

		switch(id) {
			case "ballSpawn":
				sound = ballSpawnSound;
				break;
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
			case "goalLight":
				sound = goalLightSound;
				break;
			case "goalHorn":
				sound = goalHornSound;
				break;
			case "subOut":
				sound = substituteOutSound;
				break;
			case "subIn":
				sound = substituteInSound;
				break;
			case "matchOverLight":
				sound = matchOverLightSound;
				break;
			case "matchOver":
				sound = matchOverSound;
				break;
			case "lastTurn":
				sound = lastTurnSound;
				break;

			//Unused
			case "whistle":
				sound = whistleSound;
				break;

			default:
				Debug.Log("That clip doesn't exist");
				break;
		}

		return sound;
	}
}
