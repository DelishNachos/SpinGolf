using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject ballIndicator;
    public RectTransform ballTransform;

	public RectTransform clubSpriteHolder;
	public RectTransform[] clubTransforms;
	private float[] clubSpriteXLocations;

	public Sprite[] lockedClubSprites;
	public Sprite[] unlockedClubSprites;
	private int currentIndex = 0;
	[SerializeField] private float animationTime;

	private float ballHeight;


	[SerializeField] private float minBallSize;
	[SerializeField] private float maxBallHeight;

    private bool showIndicator;

	private void OnEnable()
	{
		BallPhysics.ballOffCamera += ToggleIndicator;
		ClubController.changedClub += ChangeClubIcon;
		DataHolder.addedClub += SpriteEventCatcher;
	}

	private void OnDisable()
	{
		BallPhysics.ballOffCamera -= ToggleIndicator;
		ClubController.changedClub -= ChangeClubIcon;
		DataHolder.addedClub -= SpriteEventCatcher;
	}

	private void Start()
	{
		currentIndex = DataHolder.currentClubIndex;
		clubSpriteXLocations = new float[clubTransforms.Length];

		for (int i = 0; i < clubTransforms.Length; i++)
		{
			UpdateClubSprites(i, lockedClubSprites[i]);
			clubSpriteXLocations[i] = (i * -50) + 50f;
		}

		if (DataHolder.hasPutter)
		{
			UpdateClubSprites(0, unlockedClubSprites[0]);
		}
		if (DataHolder.hasWedge)
		{
			UpdateClubSprites(1, unlockedClubSprites[1]);
		}
		if (DataHolder.hasDriver)
		{
			UpdateClubSprites(2, unlockedClubSprites[2]);
		}
	}

	private void Update()
	{
		ShowIndicator();
	}

	private void ToggleIndicator(bool isOff, float height)
	{
		showIndicator = isOff;
		ballHeight = height;

		ballIndicator.SetActive(showIndicator);
	}

	private void ShowIndicator()
	{
		if (!showIndicator)
			return;

		float ballSize = ExtensionMethods.Remap(ballHeight, 0, maxBallHeight, .5f, minBallSize);
		ballTransform.localScale = new Vector3(ballSize, ballSize, ballSize);
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
		if (club == DataHolder.putter)
		{
			UpdateClubSprites(0, unlockedClubSprites[0]);
		}
		else if (club == DataHolder.wedge)
		{
			UpdateClubSprites(1, unlockedClubSprites[1]);
		}
		else if (club == DataHolder.driver)
		{
			UpdateClubSprites(2, unlockedClubSprites[2]);
		}
	}
}
