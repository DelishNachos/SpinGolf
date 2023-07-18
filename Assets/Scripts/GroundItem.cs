using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GroundItem : MonoBehaviour
{
	public static Action<ClubObject> pickedUp;

    public enum CLUBTYPE
	{
		putter,
		iron,
		wedge,
		driver
	};

	public CLUBTYPE clubType;
	public Transform spawnLocation;
	public GameObject spawnObject;

	private SpriteRenderer spriteVisual;

    // Start is called before the first frame update
    void Start()
    {
		spriteVisual = GetComponent<SpriteRenderer>();
        transform.DOMoveY(transform.position.y + .5f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

	private void OnDisable()
	{
		DOTween.Clear();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag != "Ball")
			return;
		ClubObject co = null;
		switch (clubType)
		{
			case (CLUBTYPE.putter):
				DataHolder.hasPutter = true;
				co = DataHolder.putter;
				break;
			case (CLUBTYPE.wedge):
				DataHolder.hasWedge = true;
				co = DataHolder.wedge;
				break;
			case (CLUBTYPE.iron):
				DataHolder.hasIron = true;
				co = DataHolder.iron;
				break;
			case (CLUBTYPE.driver):
				DataHolder.hasDriver = true;
				co = DataHolder.driver;
				break;
		}
		DataHolder.UpdateClub();
		PickUpSpawn(co);
		pickedUp?.Invoke(co);
		spriteVisual.enabled = false;
	}

	private void PickUpSpawn(ClubObject club)
	{
		SpriteRenderer sr = spawnObject.GetComponent<SpriteRenderer>();
		sr.sprite = club.clubSprite;
		TweenCallback tween = null;
		tween += ReturnObject;
		spawnObject.transform.DOMove(Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0)), 3f).OnComplete(tween);
		sr.DOFade(0f, 1.5f);
	}

	private void ReturnObject()
	{
		spawnObject.transform.position = spawnLocation.position;
		Debug.Log("Returned");
	}
}
