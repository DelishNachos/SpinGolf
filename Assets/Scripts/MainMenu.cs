using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using EasyTransition;

public class MainMenu : MonoBehaviour
{
	private AudioClip clip;

	public GameObject settingsMenu;
	public GameObject controlsMenu;
	public GameObject conformationMenu;
	public Slider masterVolume;
	public Slider musicVolume;
	public Slider effectsVolume;
	public GameObject masterSlash;
	public GameObject musicSlash;
	public GameObject effectsSlash;
	private float storedMaster;
	private float storedMusic;
	private float storedEffects;

	[SerializeField] private TransitionSettings transition;

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
		TransitionManager.Instance().Transition("Level 1", transition, DataHolder.loadDelay);
	}

	public void LevelsButton()
	{
		TransitionManager.Instance().Transition("LevelSelect", transition, DataHolder.loadDelay);
		//SceneManager.LoadScene("LevelSelect");
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

	public void ControlsButton(bool toggle)
	{
		if (toggle)
		{
			controlsMenu.SetActive(true);
			controlsMenu.transform.GetChild(0).transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBounce);
		}
		else
		{
			controlsMenu.transform.GetChild(0).transform.DOScale(Vector3.one * .2f, .2f).OnComplete(() => controlsMenu.SetActive(false));
		}
	}

	public void ConfirmationButton(bool toggle)
	{
		if (toggle)
		{
			conformationMenu.SetActive(true);
			conformationMenu.transform.GetChild(0).transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBounce);
		}
		else
		{
			conformationMenu.transform.GetChild(0).transform.DOScale(Vector3.one * .2f, .2f).OnComplete(() => conformationMenu.SetActive(false));
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
