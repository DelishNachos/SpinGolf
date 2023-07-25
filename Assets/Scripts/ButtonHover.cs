using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHover : MonoBehaviour
{
	private Vector3 startingScale;
	private AudioClip popSound;
	private Transform shadowTransform;
	private Transform buttonTransform;
	private Transform storedTransform;

	private void Start()
	{
		startingScale = transform.localScale;
		popSound = Resources.Load("Sounds/ButtonPop") as AudioClip;
		shadowTransform = transform.GetChild(0).transform;
		buttonTransform = transform.GetChild(1).transform;
		storedTransform = buttonTransform;
	}

	public void PointerEnter()
	{
		transform.localScale = startingScale * 1.2f;
		SoundEffectsManager.Instance.PlayEffectAudio(popSound);
	}

	public void PointerExit()
	{
		transform.localScale = startingScale;
	}
}
