using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject ballIndicator;
	public RectTransform indicatorTransform;
    public RectTransform ballTransform;
	public float YOffset;

	public GameObject flagIndicator;
	private GameObject flagObject;
	public float XOffset;

	public RectTransform clubSpriteHolder;
	public RectTransform[] clubTransforms;
	public float[] clubSpriteXLocations;

	public Sprite[] lockedClubSprites;
	public Sprite[] unlockedClubSprites;
	private int currentIndex = 0;
	[SerializeField] private float animationTime;

	private float ballHeight;
	private float ballX;


	[SerializeField] private float minBallSize;
	[SerializeField] private float maxBallHeight;

    private bool showBallIndicator;
	private bool showFlagIndicator;

	public TextMeshProUGUI hitCounter;

	public GameObject pauseMenu;

	public GameObject endMenu;
	public GameObject highScore;
	public TextMeshProUGUI hitsText;
	public static Action<int> pressedButton;


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
	public static Action<bool> inSettings;

	private void OnEnable()
	{
		BallPhysics.ballOffCameraY += ToggleBallIndicator;
		ClubController.changedClub += ChangeClubIcon;
		DataHolder.addedClub += SpriteEventCatcher;
		DataHolder.hitBall += UpdateHitCounter;
		CheckForEnd.VisibleEvent += ToggleFlagIndicator;
		CheckForEnd.levelComplete += EnableEndScreen;
		GameManager.isHighScore += EnableHighScoreScreen;
		GameManager.pausedGame += TogglePauseMenu;
		GameManager.openedSettings += ToggleSettingsMenu;
	}

	private void OnDisable()
	{
		BallPhysics.ballOffCameraY -= ToggleBallIndicator;
		ClubController.changedClub -= ChangeClubIcon;
		DataHolder.addedClub -= SpriteEventCatcher;
		DataHolder.hitBall -= UpdateHitCounter;
		CheckForEnd.VisibleEvent -= ToggleFlagIndicator;
		CheckForEnd.levelComplete -= EnableEndScreen;
		GameManager.isHighScore -= EnableHighScoreScreen;
		GameManager.pausedGame -= TogglePauseMenu;
		GameManager.openedSettings -= ToggleSettingsMenu;
	}

	private void Start()
	{
		currentIndex = DataHolder.currentClubIndex;
		//clubSpriteXLocations = new float[clubTransforms.Length];

		for (int i = 0; i < clubTransforms.Length; i++)
		{
			UpdateClubSprites(i, lockedClubSprites[i]);
			//clubSpriteXLocations[i] = (i * -50) + 25f;
		}

		UpdateHitCounter(DataHolder.hits);

		if (DataHolder.hasPutter)
		{
			SpriteEventCatcher(DataHolder.putter);
		}
		if (DataHolder.hasWedge)
		{
			SpriteEventCatcher(DataHolder.wedge);
		}
		if (DataHolder.hasIron)
		{
			SpriteEventCatcher(DataHolder.iron);
		}
		if (DataHolder.hasDriver)
		{
			SpriteEventCatcher(DataHolder.driver);
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

	private void Update()
	{
		ShowBallIndicator();
		ShowFlagIndicator();		
	}

	private void ToggleBallIndicator(bool isOff, Vector2 height)
	{
		showBallIndicator = isOff;
		ballHeight = height.y;
		ballX = height.x;

		ballIndicator.SetActive(showBallIndicator);
	}


	private void ShowBallIndicator()
	{
		if (!showBallIndicator)
			return;

		float ballSize = ExtensionMethods.Remap(ballHeight, 0, maxBallHeight, .5f, minBallSize);
		ballTransform.localScale = new Vector3(ballSize, ballSize, ballSize);
		
		Vector2 pos = Camera.main.WorldToScreenPoint(new Vector2(ballX, ballHeight));
		pos.y = Screen.height - YOffset;
		ballIndicator.transform.position = pos;
	}

	private void ToggleFlagIndicator(bool TurnOn, GameObject flag)
	{
		showFlagIndicator = TurnOn;
		flagObject = flag;

		flagIndicator.SetActive(TurnOn);
	}
	
	private void ShowFlagIndicator()
	{
		if (!showFlagIndicator)
			return;

		float offset = XOffset;
		Vector2 pos = Camera.main.WorldToScreenPoint(new Vector2(flagObject.transform.position.x, flagObject.transform.position.y));
		if (flagObject.transform.position.x > Camera.main.transform.position.x)
		{
			offset = -Mathf.Abs(offset);
		}
		pos.x = Screen.width + XOffset;
		flagIndicator.transform.position = pos;
	}

	private void ChangeClubIcon(int index)
	{
		clubSpriteHolder.DOLocalMoveX(clubSpriteXLocations[index], animationTime);
		clubTransforms[currentIndex].DOScale(1, animationTime);
		clubTransforms[index].DOScale(2, animationTime);
		currentIndex = index;
	}

	private void UpdateClubSprites(int index, Sprite sprite)
	{
		clubTransforms[index].gameObject.GetComponent<Image>().sprite = sprite;
	}

	private void SpriteEventCatcher(ClubObject club)
	{
		if (club == null)
			return;
		UpdateClubSprites(club.referenceIndex, unlockedClubSprites[club.referenceIndex]);
	}

	private void UpdateHitCounter(int hits)
	{
		hitCounter.text = hits.ToString();
	}

	private void TogglePauseMenu(bool pause)
	{
		if (pause)
		{
			pauseMenu.SetActive(true);
			pauseMenu.transform.GetChild(0).transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBounce);
		} else
		{
			pauseMenu.transform.GetChild(0).transform.DOScale(Vector3.one * .2f, .1f).OnComplete(() => pauseMenu.SetActive(false));
		}
	}

	private void EnableEndScreen(int hits)
	{
		endMenu.SetActive(true);
		hitsText.text = "Hits:" + hits.ToString();
		endMenu.transform.GetChild(0).transform.DOScale(Vector3.one , .2f).SetEase(Ease.OutBounce);
	}

	private void EnableHighScoreScreen()
	{
		highScore.SetActive(true);
		highScore.transform.DOLocalRotate(new Vector3(0, 0, -7f), .5f, RotateMode.Fast).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
	}

	public void ButtonPressed(int buttonIndex)
	{
		pressedButton?.Invoke(buttonIndex);
	}

	public void ToggleSettingsMenu(bool toggle)
	{
		if (toggle)
		{
			settingsMenu.SetActive(true);
			pauseMenu.SetActive(false);
			settingsMenu.transform.GetChild(0).transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBounce);
			inSettings?.Invoke(true);
		} else
		{
			settingsMenu.SetActive(false);
			pauseMenu.SetActive(true);
			inSettings?.Invoke(false);
		}
	}

	public void ToggleMasterVolume()
	{
		SoundEffectsManager.Instance.ToggleMasterVolume();
		if (DataHolder.masterMute)
		{
			//storedMaster = SoundEffectsManager.Instance.MasterVolume;
			masterVolume.value = 0;
			masterSlash.SetActive(true);
		} else
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
}
