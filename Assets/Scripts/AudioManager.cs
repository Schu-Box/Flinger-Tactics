using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
	
	//[SerializeField] AudioMixerGroup fallbackAudioMixerGroup;
	[SerializeField] AudioMixerGroup audioMixerGroup;
	[SerializeField] int minimumAudioPool = 20; // number of preloaded AudioSources for the pool
	private List<AudioSource> audioPool = new List<AudioSource>();
	private List<AudioSource> availablePool = new List<AudioSource>();



	private Sound[] sounds; // all audio setups, loaded from Resources
	private SoundSettings[] settings; // all runtime instances of the setups + ambience GameObjects

    private static AudioManager instance;
    public static AudioManager Instance {
        get { return instance ?? (instance = new GameObject("AudioManager").AddComponent<AudioManager>()); }
    }
	
	
	// channel-through call functions
	public void Play(string key){ /* Debug.Log(key); */ GetSoundSettings(key).Play(); }
	public void Play(string key, int id){ GetSoundSettings(key).Play(id); }

	public void AmbientPlay(string key){ GetSoundSettings(key).AmbientPlay(); }
	public void AmbientStop(string key){ GetSoundSettings(key).AmbientStop(); }
	public void PentatonicCounterReset(string key){ GetSoundSettings(key).PentatonicCounterReset(); }

	void Awake() {
		Debug.Log("Woken");
		instance = this;
		// load audio resources
		sounds = LoadAudioResources();

		// create all AudioSettings for the AudioSetups
		settings = CreateSettings(sounds);

		// preload a number of audioPools from the start
		PreloadAudioPool(minimumAudioPool);
	}

	void Update() {
		// update ambience volumes
		for (int i = 0; i < settings.Length; i++){
			settings[i].UpdateAmbience();
		}

		for (int i = 0; i < audioPool.Count; i++) {
			if (!audioPool[i].isPlaying){
				availablePool.Add(audioPool[i]);
				audioPool.RemoveAt(i);
			}
		}

		for (int i = 0; i < availablePool.Count; i++) {
			if (availablePool[i].isPlaying){
				audioPool.Add(availablePool[i]);
				availablePool.RemoveAt(i);
			}
		}
	}

	private static Sound[] LoadAudioResources(){
		Sound[] soundSetups = Resources.LoadAll<Sound>("Sounds");
		Debug.Log("[SoundManager] loaded " + soundSetups.Length + " sounds!");
		return soundSetups;
	}

	/// Create the SoundSettings array from 
	private static SoundSettings[] CreateSettings(Sound[] soundSetups){
		SoundSettings[] soundSettings = new SoundSettings[soundSetups.Length];
		for (int i = 0; i < soundSetups.Length; i++){
			soundSettings[i] = new SoundSettings(soundSetups[i]);
		}
		return soundSettings;
	}

	private Sound GetSound(string key){
		for (int i = 0; i < sounds.Length; i++){
			if (sounds[i].name == key){
				return sounds[i];
			}
		}

		Debug.Log("[SoundManager] No SoundSetup for this key found.");
		return null;
	}

	public SoundSettings GetSoundSettings(string key){
		for (int i = 0; i < sounds.Length; i++){
			if (sounds[i].name == key){
				return settings[i];
			}
		}

		Debug.Log("[SoundManager] No SoundSettings for this key found.");
		return null;
	}



	// audio pool management
	private void PreloadAudioPool(int count){
		for (int i = 0; i < count; i++){
			availablePool.Add(this.gameObject.AddComponent<AudioSource>());
		}
	}	

	public void PlayInPool(float volume, AudioClip audioClip){
		float pitch = 1f;
		PlayInPool(pitch, volume, audioClip, audioMixerGroup);
	}

	public void PlayInPool(float pitch, float volume, AudioClip audioClip, AudioMixerGroup channel){
		if (availablePool.Count > 0){
			SetupAudioSource(availablePool[0], pitch, volume, audioClip, channel);
			audioPool.Add(availablePool[0]);
			availablePool.RemoveAt(0);
		} else {
			AudioSource newAudioSource = this.gameObject.AddComponent<AudioSource>();
			SetupAudioSource(newAudioSource, pitch, volume, audioClip, channel);
			audioPool.Add(newAudioSource);
		}
	}

	private static void SetupAudioSource(AudioSource newAudioSource, float pitch, float volume, AudioClip audioClip, AudioMixerGroup channel){
		newAudioSource.playOnAwake = false;
		newAudioSource.pitch = pitch;
		newAudioSource.volume = volume;
		newAudioSource.outputAudioMixerGroup = channel;
		newAudioSource.clip = audioClip;
		newAudioSource.loop = false;

		newAudioSource.Play();
	}
}
