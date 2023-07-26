using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleAudioManager;

public class SoundEffectsManager : MonoBehaviour
{
	#region singleton
	public static SoundEffectsManager Instance;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else
		{
			Destroy(gameObject);
		}
	}
	#endregion

	public float MasterVolume => _masterVolume;
	public float MusicVolume => _musicVolume;
	public float EffectsVolume => _effectsVolume;
	private float _masterVolume, _musicVolume, _effectsVolume;
	[SerializeField] private AudioSource effectsSource;
	[SerializeField] private AudioSource specialEffectsSource;
	private bool masterMuted;

	private void Start()
	{
		_masterVolume = DataHolder.masterVolume;
		_musicVolume = DataHolder.musicVolume;
		_effectsVolume = DataHolder.effectsVolume;
		ChangeMasterVolume(_masterVolume);
		ChangeMusicVolume(_musicVolume);
		ChangeEffectsVolume(_effectsVolume);
	}

	public void PlayEffectAudio(AudioClip clip)
	{
		effectsSource.PlayOneShot(clip);
	}

	public void StopEffectAudio()
	{
		effectsSource.Stop();
	}

	public void PlaySpecialEffectAudio(AudioClip clip, float volume)
	{
		specialEffectsSource.volume = ExtensionMethods.Remap(volume, 0, 1, 0, _effectsVolume);
		specialEffectsSource.PlayOneShot(clip);
	}

	public void LoopSpecialEffectAudio(AudioClip clip, float volume)
	{
		specialEffectsSource.volume = ExtensionMethods.Remap(volume, 0, 1, 0, _effectsVolume);
		specialEffectsSource.loop = true;
		specialEffectsSource.clip = clip;
		specialEffectsSource.Play();
	}

	public void ChangeSpecialEffectsVolume(float volume)
	{
		specialEffectsSource.volume = ExtensionMethods.Remap(volume, 0, 1, 0, _effectsVolume);
	}

	public void StopSpecialEffectAudio()
	{
		specialEffectsSource.Stop();
	}

	public void ChangeMasterVolume(float volume)
	{
		_masterVolume = volume;
		AudioListener.volume = _masterVolume;
		DataHolder.masterVolume = volume;
	}

	public void ChangeMusicVolume(float volume)
	{
		_musicVolume = volume;
		SimpleAudioManager.Manager.instance.maxVolume = _musicVolume;
		SimpleAudioManager.Manager.instance.GetSource().ForEach(s => s.volume = _musicVolume);
		DataHolder.musicVolume = volume;
	}

	public void ChangeEffectsVolume(float volume)
	{
		_effectsVolume = volume;
		effectsSource.volume = _effectsVolume;
		DataHolder.effectsVolume = volume;
	}

	public void ToggleMasterVolume()
	{
		if (DataHolder.masterMute)
		{
			_masterVolume = DataHolder.storedMasterVolume;
			AudioListener.volume = _masterVolume;
			DataHolder.masterMute = false;
			DataHolder.masterVolume = DataHolder.storedMasterVolume;
		} else
		{
			DataHolder.storedMasterVolume = AudioListener.volume;
			AudioListener.volume = 0;
			DataHolder.masterMute = true;
			DataHolder.masterVolume = 0;
		}
	}
	public void ToggleMusicVolume()
	{
		if (DataHolder.musicMute)
		{
			SimpleAudioManager.Manager.instance.maxVolume = DataHolder.storedMusicVolume;
			SimpleAudioManager.Manager.instance.UnMute();
			DataHolder.musicMute = false;
			DataHolder.musicVolume = DataHolder.storedMusicVolume;
		} else
		{
			DataHolder.storedMusicVolume = SimpleAudioManager.Manager.instance.maxVolume;
			SimpleAudioManager.Manager.instance.Mute();
			DataHolder.musicMute = true;
			DataHolder.musicVolume = 0;
		}
	}
	public void ToggleEffectsVolume()
	{
		if (DataHolder.effectsMute)
		{
			effectsSource.volume = DataHolder.storedEffectsVolume;
			effectsSource.mute = false;
			specialEffectsSource.mute = false;
			DataHolder.effectsMute = false;
			DataHolder.effectsVolume = DataHolder.storedEffectsVolume;
		} else
		{
			DataHolder.storedEffectsVolume = effectsSource.volume;
			effectsSource.mute = true;
			specialEffectsSource.mute = true;
			DataHolder.effectsMute = true;
			DataHolder.effectsVolume = 0;
		}
	}

	public bool MasterIsMuted()
	{
		return masterMuted;
	}

	public bool EffectsIsMuted()
	{
		if (effectsSource.mute)
		{
			return true;
		} else
		{
			return false;
		}
	}
}
