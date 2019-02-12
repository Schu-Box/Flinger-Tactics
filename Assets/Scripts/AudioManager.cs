using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayGoalSound(bool homeScored) {
		audioSource.Play();
	}
}
