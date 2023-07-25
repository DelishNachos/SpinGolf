using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	private AudioClip clip;

	public GameObject settingsMenu;
	public Slider masterVolume;
	public Slider musicVolume;
	public Slider effectsVolume;
	public GameObject masterSlash;
	public GameObject musicSlash;
	public GameObject effectsSlash;
	private float storedMaster;
	private float storedMusic;
	private float storedEffects;

	private void Start()
	{
		clip = Resources.Load("Sounds/ButtonClick") as AudioClip;

		if (DataHolder.storedMasterVolume == -1)
		{
			DataHolder.storedMasterVolume = DataHolder.masterVolume;
			DataHolder.storedMusicVolume = DataHolder.musicVolume;
			DataHolder.storedEffectsVolume = DataHolder.effectsVolume;
		}

		masterVolume.onValueChanged.AddListener(val => SoundEffectsManager.Instance.ChangeMasterVolume(val));
		musicVolume.onValueChanged.AddListener(val => SoundEffectsManager.Instance.ChangeMusicVolume(val));
		effectsVolume.onValueChanged.AddListener(val => SoundEffectsManager.Instance.ChangeEffectsVolume(val));
		masterVolume.value = DataHolder.masterVolume;
		musicVolume.value = DataHolder.musicVolume;
		effectsVolume.value = DataHolder.effectsVolume;
		SoundEffectsManager.Instance.ChangeMasterVolume(masterVolume.value);
		SoundEffectsManager.Instance.ChangeMusicVolume(musicVolume.value);
		SoundEffectsManager.Instance.ChangeEffectsVolume(effectsVolume.value);

		if (DataHolder.masterMute)
		{
			//ToggleMasterVolume();
			masterSlash.SetActive(true);
		}
		if (DataHolder.musicMute)
		{
			//ToggleMusicVolume();
			musicSlash.SetActive(true);
		}
		if (DataHolder.effectsMute)
		{
			//ToggleEffectsVolume();
			effectsSlash.SetActive(true);
		}
	}

	public void PlayButton()
	{
		SceneManager.LoadScene("Level 1");
	}

	public void LevelsButton()
	{
		SceneManager.LoadScene("LevelSelect");
	}

	public void SettingsButton(bool toggle)
	{
		if (toggle)
		{
			settingsMenu.SetActive(true);
			settingsMenu.transform.GetChild(0).transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBounce);
		}
		else
		{
			settingsMenu.transform.GetChild(0).transform.DOScale(Vector3.one * .2f, .2f).OnComplete(() => settingsMenu.SetActive(false));
		}
	}

	public void QuitButton()
	{
		Application.Quit();
	}

	public void PlaySound()
	{
		SoundEffectsManager.Instance.PlayEffectAudio(clip);
	}

	public void ToggleMasterVolume()
	{
		SoundEffectsManager.Instance.ToggleMasterVolume();
		if (DataHolder.masterMute)
		{
			//storedMaster = SoundEffectsManager.Instance.MasterVolume;
			masterVolume.value = 0;
			masterSlash.SetActive(true);
		}
		else
		{
			masterVolume.value = DataHolder.storedMasterVolume;
			masterSlash.SetActive(false);
		}
	}

	public void ToggleMusicVolume()
	{
		SoundEffectsManager.Instance.ToggleMusicVolume();
		if (SimpleAudioManager.Manager.instance.IsMuted())
		{
			//storedMusic = SoundEffectsManager.Instance.MusicVolume;
			musicVolume.value = 0;
			musicSlash.SetActive(true);
		}
		else
		{
			musicVolume.value = DataHolder.storedMusicVolume;
			musicSlash.SetActive(false);
		}
	}

	public void ToggleEffectsVolume()
	{
		SoundEffectsManager.Instance.ToggleEffectsVolume();
		if (SoundEffectsManager.Instance.EffectsIsMuted())
		{
			//storedEffects = SoundEffectsManager.Instance.EffectsVolume;
			effectsVolume.value = 0;
			effectsSlash.SetActive(true);
		}
		else
		{
			effectsVolume.value = DataHolder.storedEffectsVolume;
			effectsSlash.SetActive(false);
		}
	}

	private void OnApplicationQuit()
	{
		DataHolder.SaveVolumeData();
	}
}
