using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
	public LevelRules levelRules;
	public static Action<bool> pausedGame;
	private bool isPaused;

	private void OnEnable()
	{
		CheckForEnd.levelComplete += EndLevel;
		WaterScript.StuckInWater += RestartLevel;
		UIManager.pressedButton += DetermineButtonAction;
	}

	private void OnDisable()
	{
		CheckForEnd.levelComplete -= EndLevel;
		WaterScript.StuckInWater -= RestartLevel;
		UIManager.pressedButton -= DetermineButtonAction;
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

		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
		{
			if (!isPaused)
			{
				PauseGame();				
			} else
			{
				UnpauseGame();
			}
			isPaused = !isPaused;
		}
	}

	public void EndLevel(int hits)
	{
        Debug.Log("In Hole");
	}

	private void DetermineButtonAction(int index)
	{
		switch(index)
		{
			case 0:
				NextLevel();
				break;
			case 1:
				RestartLevel();
				break;
			case 2:
				ReturnToMenu();
				break;
			case 3:
				UnpauseGame();
				isPaused = false;
				break;
			case 4:
				break;
		}
	}

	public void RestartLevel()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.buildIndex);
		DataHolder.Reset(levelRules);
	}

	private void NextLevel()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.buildIndex + 1);
		DataHolder.Reset(levelRules);
	}

	private void ReturnToMenu()
	{
		SceneManager.LoadScene("MainMenu");
		DataHolder.Reset(levelRules);
	}

	private void PauseGame()
	{
		Time.timeScale = 0;
		DataHolder.isPaused = true;
		pausedGame?.Invoke(true);
	}

	private void UnpauseGame()
	{
		Time.timeScale = 1;
		DataHolder.isPaused = false;
		pausedGame?.Invoke(false);
	}
}
