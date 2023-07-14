using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject ballIndicator;
    public RectTransform ballTransform;

	public Image clubSR;

	private float ballHeight;


	[SerializeField] private float minBallSize;
	[SerializeField] private float maxBallHeight;

    private bool showIndicator;

	private void OnEnable()
	{
		BallPhysics.ballOffCamera += ToggleIndicator;
		ClubController.changedClub += ChangeClubIcon;
	}

	private void OnDisable()
	{
		BallPhysics.ballOffCamera -= ToggleIndicator;
		ClubController.changedClub -= ChangeClubIcon;
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

	private void ChangeClubIcon(Sprite sprite)
	{
		clubSR.sprite = sprite;
	}
}
