using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum PitchMode {
	Fixed, 
	Halftones, 
	Range
};

public enum VolumeMode {
	Fixed, 
	Range
};

[CreateAssetMenu(fileName = "New Sound", menuName = "Sound", order = 2)]
public class Sound : ScriptableObject {

	public AudioMixerGroup channel;
	public List<AudioClip> clips;

	// pitch
	public PitchMode pitchMode;
	[Range(0.25f, 2f)] public float pitchFixed = 1f;
	public bool halftonesRandom = true; // false = in pentatonic order, true = in random order
	public int halftonesVariations = 11;
	public Vector2 pitchRange = new Vector2(0.9f, 1.2f);



	// volume
	public VolumeMode volumeMode;
	public float volumeFixed = 0.5f;
	public Vector2 volumeRange = new Vector2(0.1f, 0.3f);



	// ambience
	[Header ("Ambience")]
	public bool isAmbientLoop = false;
	public float ambientFadeInSeconds = 4f;
	public float ambientFadeOutSeconds = 2f;
}
