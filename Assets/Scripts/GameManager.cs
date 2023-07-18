using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public LevelRules levelRules;

	private void OnEnable()
	{
		CheckForEnd.levelComplete += EndLevel;
		WaterScript.StuckInWater += RestartLevel;
	}

	private void OnDisable()
	{
		CheckForEnd.levelComplete -= EndLevel;
		WaterScript.StuckInWater -= RestartLevel;
	}

	private void Start()
	{
		levelRules.LoadRules();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			RestartLevel();
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

	public void RestartLevel()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.buildIndex);
		DataHolder.Reset(levelRules);
	}
}
