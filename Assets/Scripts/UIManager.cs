using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject ballIndicator;
    public RectTransform ballTransform;

	public RectTransform clubSpriteHolder;
	public RectTransform[] clubTransforms;
	public float[] clubSpriteXLocations;

	public Sprite[] lockedClubSprites;
	public Sprite[] unlockedClubSprites;
	private int currentIndex = 0;
	[SerializeField] private float animationTime;

	private float ballHeight;


	[SerializeField] private float minBallSize;
	[SerializeField] private float maxBallHeight;

    private bool showIndicator;

	public TextMeshProUGUI hitCounter;

	private void OnEnable()
	{
		BallPhysics.ballOffCamera += ToggleIndicator;
		ClubController.changedClub += ChangeClubIcon;
		DataHolder.addedClub += SpriteEventCatcher;
		DataHolder.hitBall += UpdateHitCounter;
	}

	private void OnDisable()
	{
		BallPhysics.ballOffCamera -= ToggleIndicator;
		ClubController.changedClub -= ChangeClubIcon;
		DataHolder.addedClub -= SpriteEventCatcher;
		DataHolder.hitBall -= UpdateHitCounter;
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
		if (club == null)
			return;
		UpdateClubSprites(club.referenceIndex, unlockedClubSprites[club.referenceIndex]);
	}

	private void UpdateHitCounter(int hits)
	{
		hitCounter.text = hits.ToString();
	}
}
