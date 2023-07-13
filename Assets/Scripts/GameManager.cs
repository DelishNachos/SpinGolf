using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private void OnEnable()
	{
		CheckForEnd.levelComplete += EndLevel;
	}

	private void OnDisable()
	{
		CheckForEnd.levelComplete -= EndLevel;
	}

	public void EndLevel()
	{
        Debug.Log("In Hole");
	}
}
