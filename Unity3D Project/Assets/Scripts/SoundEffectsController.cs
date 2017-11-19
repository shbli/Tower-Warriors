using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectsController : MonoBehaviour {
	#region singletonImplementation
	static SoundEffectsController instance = null;
	public static SoundEffectsController Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<SoundEffectsController>();
			}
			return instance;
		}
	}
	SoundEffectsController() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original SoundEffectsController instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	Dictionary<string,AudioSource> soundEffectsHashTable = new Dictionary<string, AudioSource>();
	Dictionary<string,float> soundEffectsLastPlayedHashTable = new Dictionary<string, float>();

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		foreach(Transform _child in transform) {
			_child.GetComponent<AudioSource>().Stop();
			_child.GetComponent<AudioSource>().playOnAwake = false;
			soundEffectsHashTable[_child.name] = _child.GetComponent<AudioSource>();
			soundEffectsLastPlayedHashTable[_child.name] = Time.realtimeSinceStartup;
		}
	}

	public void playSoundEffectOneShot(string pSoundEffectName) {
		soundEffectsHashTable[pSoundEffectName].PlayOneShot( soundEffectsHashTable[pSoundEffectName].clip );
		//record last time the sound has been played
		soundEffectsLastPlayedHashTable[pSoundEffectName] = Time.realtimeSinceStartup;
	}

	public void playSoundEffectOneShotIfAllowancePassedSinceLastPlayed(string pSoundEffectName, float allowanceDelaySeconds) {
		float lastPlayedTime = soundEffectsLastPlayedHashTable[pSoundEffectName];
		if ( ( Time.realtimeSinceStartup - lastPlayedTime ) > allowanceDelaySeconds ) {
			//time since allowance delay has really passed
			soundEffectsHashTable[pSoundEffectName].PlayOneShot( soundEffectsHashTable[pSoundEffectName].clip );
			//record last time the sound has been played
			soundEffectsLastPlayedHashTable[pSoundEffectName] = Time.realtimeSinceStartup;
		}
	}

	public void playSoundEffectsLoop(string pSoundEffectName) {
		if (!soundEffectsHashTable[pSoundEffectName].isPlaying) {
			soundEffectsHashTable[pSoundEffectName].volume = 0f;
			soundEffectsHashTable[pSoundEffectName].Play();
		}
		iTween.Stop(soundEffectsHashTable[pSoundEffectName].gameObject);
		iTween.AudioTo(soundEffectsHashTable[pSoundEffectName].gameObject,0.3f,soundEffectsHashTable[pSoundEffectName].pitch,1f);
	}
	public void stopSoundEffectLoop(string pSoundEffectName) {
		iTween.Stop(soundEffectsHashTable[pSoundEffectName].gameObject);
		iTween.AudioTo(soundEffectsHashTable[pSoundEffectName].gameObject,0f,soundEffectsHashTable[pSoundEffectName].pitch,1f);
	}
}
