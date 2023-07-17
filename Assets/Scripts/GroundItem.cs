using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundItem : MonoBehaviour
{
    public enum CLUBTYPE
	{
		putter,
		wedge,
		driver
	};

	public CLUBTYPE clubType;

    // Start is called before the first frame update
    void Start()
    {
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

		switch (clubType)
		{
			case (CLUBTYPE.putter):
				DataHolder.hasPutter = true;
				break;
			case (CLUBTYPE.wedge):
				DataHolder.hasWedge = true;
				break;
			case (CLUBTYPE.driver):
				DataHolder.hasDriver = true;
				break;
		}
		Debug.Log(clubType);
		DataHolder.UpdateClub();
		Destroy(gameObject);
	}
}
