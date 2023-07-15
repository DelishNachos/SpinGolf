using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private void Awake()
	{
		//Time.timeScale = .5f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.buildIndex);
		}

		if (Input.GetKey(KeyCode.T))
			Time.timeScale = 2f;
		else
			Time.timeScale = 1f;

		if (Input.GetKeyDown(KeyCode.N))
		{
			DataHolder.hasWedge = true;
			DataHolder.UpdateClub();
		}
	}

	public void EndLevel()
	{
        Debug.Log("In Hole");
	}
}
