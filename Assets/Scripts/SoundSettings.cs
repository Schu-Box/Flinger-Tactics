using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings {

	public Sound sound;
	private AudioSource ambienceAudioSource;
    private int pentatonicCounter = 0;
    private int pentatonicIncrement = 1;
    private int lastHalftoneIndex = -1;
    private int lastAudioClipIndex = -1;

    private float ambientVolume = 0f;
    private bool ambientActive = false;



	public SoundSettings(Sound s) {
        this.sound = s;

        if (sound.isAmbientLoop) {
            GameObject go = new GameObject(sound.name + "_ambience");
            go.transform.SetParent(AudioManager.Instance.transform);
            ambienceAudioSource = go.AddComponent<AudioSource>();
            SetupAmbientAudioSource(ambienceAudioSource, sound.pitchFixed, sound.clips[0]);
        }
    }

    public void UpdateAmbience() {
        if (!sound.isAmbientLoop) {
            return;
        }

        AdjustAmbientVolume();
    }



    private void SetupAmbientAudioSource(AudioSource newAudioSource, float pitch, AudioClip audioClip) {
        newAudioSource.playOnAwake = false;
        newAudioSource.pitch = pitch;
        newAudioSource.volume = sound.volumeFixed;
        newAudioSource.outputAudioMixerGroup = sound.channel;
        newAudioSource.clip = audioClip;
        newAudioSource.loop = sound.isAmbientLoop;
    }



    // ---------------------------------------------
    // ---------- external call functions ----------
    // ---------------------------------------------

    /// Play this sound once.
    public void Play() {
        if (!HasAudioClips()) {
            return;
        }

        float sendPitch = GetPitch();
        float sendVolume = GetVolume();
        AudioClip sendClip = GetClip();

        AudioManager.Instance.PlayInPool(sendPitch, sendVolume, sendClip, sound.channel);
    }

    /// Play a specific halftone value of this sound once.
    public void Play(int id) {
        if (!HasAudioClips()) {
            return;
        }

        AudioManager.Instance.PlayInPool(GetSpecificPitch(id), GetVolume(), GetClip(), sound.channel);
    }

    public void AmbientPlay() {
        if (!ambienceAudioSource.isPlaying) {
            ambienceAudioSource.Play();
        }
        ambientActive = true;
    }

    public void AmbientStop() {
        ambientActive = false;
    }

    public void PentatonicCounterReset() {
        pentatonicCounter = 0;
        pentatonicIncrement = 1;
    }
	



    // ---------------------------------------------
    // ---------- internal help functions ----------
    // ---------------------------------------------

    private bool HasAudioClips()
    {
        if (sound.clips.Count == 0)
        {
            Debug.LogError(sound.name + " has no AudioClips loaded!");
            return false;
        }

        return true;
    }

    private float GetPitch() {
        if (sound.pitchMode == PitchMode.Fixed) {
            return sound.pitchFixed;
        }
        else if (sound.pitchMode == PitchMode.Halftones) {
            int halftoneIndex;

            if (sound.halftonesRandom) {
                halftoneIndex = GetRandomExcludingLastIndex(sound.halftonesVariations, lastHalftoneIndex);
                lastHalftoneIndex = halftoneIndex;
            }
            else {
                halftoneIndex = pentatonicCounter;
                if (pentatonicCounter + pentatonicIncrement >= sound.halftonesVariations || pentatonicCounter + pentatonicIncrement < 0) {
                    pentatonicIncrement *= -1;
                }
                pentatonicCounter += pentatonicIncrement;
            }

            return Mathf.Pow(1.05946f, GetPentatonic(halftoneIndex));
        } else {
            return Random.Range(sound.pitchRange.x, sound.pitchRange.y);
        }
    }

    private float GetSpecificPitch(int id) {
        return Mathf.Pow(1.05946f, GetPentatonic(id));
    }

    private float GetVolume() {
        if (sound.volumeMode == VolumeMode.Fixed) {
            return sound.volumeFixed;
        } else {
            return Random.Range(sound.volumeRange.x, sound.volumeRange.y);
        }
    }

    private AudioClip GetClip() {
        if (sound.clips.Count == 1) {
            return sound.clips[0];
        }
        else {
            lastAudioClipIndex = GetRandomExcludingLastIndex(sound.clips.Count, lastAudioClipIndex);
            return sound.clips[lastAudioClipIndex];
        }
    }

	private int GetRandomExcludingLastIndex(int count, int lastIndex) {
		int rando = lastIndex;
		while(rando == lastIndex) {
			rando = Random.Range(0, count);
		}
		return rando;
	}

    private void AdjustAmbientVolume() {
        if (ambientActive) {
            ambientVolume += (Time.deltaTime * sound.volumeFixed) / sound.ambientFadeInSeconds;
        } else {
            ambientVolume -= (Time.deltaTime * sound.volumeFixed) / (sound.ambientFadeOutSeconds);
            if (ambientVolume < 0f) {
                ambienceAudioSource.Stop();
            }
        }

        ambientVolume = Mathf.Clamp(ambientVolume, 0f, sound.volumeFixed);

        ambienceAudioSource.volume = ambientVolume;
    }

	public static int GetPentatonic(int input){
		int offset = 6;


        if (input >= 20){
            input = input % 10;
        }
        if (input > 10){
            input = 10 - (input % 10);
        } 

		switch(input){
			case 0: return 0 - offset;
			case 1: return 1 - offset;
			case 2: return 5 - offset;
			case 3: return 7 - offset;
			case 4: return 8 - offset;
			case 5: return 12 - offset;
			case 6: return 13 - offset;
			case 7: return 17 - offset;
			case 8: return 19 - offset;
			case 9: return 20 - offset;
			case 10: return 24 - offset;
		}
		return 0;
	}
}
