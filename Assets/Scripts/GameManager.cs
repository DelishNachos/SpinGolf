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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.buildIndex);
		}
	}

	public void EndLevel()
	{
        Debug.Log("In Hole");
	}
}
