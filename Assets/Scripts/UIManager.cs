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
	public TextMeshProUGUI hitsText;
	public static Action<int> pressedButton;

	private void OnEnable()
	{
		BallPhysics.ballOffCamera += ToggleBallIndicator;
		ClubController.changedClub += ChangeClubIcon;
		DataHolder.addedClub += SpriteEventCatcher;
		DataHolder.hitBall += UpdateHitCounter;
		CheckForEnd.VisibleEvent += ToggleFlagIndicator;
		CheckForEnd.levelComplete += EnableEndScreen;
		GameManager.pausedGame += TogglePauseMenu;
	}

	private void OnDisable()
	{
		BallPhysics.ballOffCamera -= ToggleBallIndicator;
		ClubController.changedClub -= ChangeClubIcon;
		DataHolder.addedClub -= SpriteEventCatcher;
		DataHolder.hitBall -= UpdateHitCounter;
		CheckForEnd.VisibleEvent -= ToggleFlagIndicator;
		CheckForEnd.levelComplete -= EnableEndScreen;
		GameManager.pausedGame -= TogglePauseMenu;
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
		} else
		{
			pauseMenu.SetActive(false);
		}
	}

	private void EnableEndScreen(int hits)
	{
		endMenu.SetActive(true);
		hitsText.text = "Hits:" + hits.ToString();
	}

	public void ButtonPressed(int buttonIndex)
	{
		pressedButton?.Invoke(buttonIndex);
	}
}
