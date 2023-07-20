using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHover : MonoBehaviour
{
	private Vector3 startingScale;

	private void Start()
	{
		startingScale = transform.localScale;
	}

	public void PointerEnter()
	{
		transform.localScale = startingScale * 1.2f;
	}

	public void PointerExit()
	{
		transform.localScale = startingScale;
	}
}
