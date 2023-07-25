using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	public LevelRules levelRules;
	public static Action<bool> pausedGame;
	public static Action<bool> openedSettings;
	public static Action isHighScore;
	private bool isPaused;
	private bool inSettings;

	public int[] scores;

	private void OnEnable()
	{
		CheckForEnd.levelComplete += EndLevel;
		WaterScript.StuckInWater += RestartLevel;
		UIManager.pressedButton += DetermineButtonAction;
		BallPhysics.ballOffCameraX += RestartLevel;
		UIManager.inSettings += toggleSettingsBool;
	}

	private void OnDisable()
	{
		CheckForEnd.levelComplete -= EndLevel;
		WaterScript.StuckInWater -= RestartLevel;
		UIManager.pressedButton -= DetermineButtonAction;
		BallPhysics.ballOffCameraX -= RestartLevel;
		UIManager.inSettings -= toggleSettingsBool;
	}

	private void Start()
	{
		levelRules.LoadRules();
		scores = DataHolder.highScores;
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
				isPaused = !isPaused;
			} else
			{
				if (inSettings)
				{
					openedSettings?.Invoke(false);
				} else
				{
					UnpauseGame();
					isPaused = !isPaused;
				}
			}
			
		}
	}

	public void EndLevel(int hits)
	{
		bool isHigh = DataHolder.SaveData(hits, SceneManager.GetActiveScene().buildIndex);
		if (isHigh)
			isHighScore?.Invoke();
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
		DOTween.CompleteAll();
	}

	private void NextLevel()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.buildIndex + 1);
		DataHolder.Reset(levelRules);
		DOTween.CompleteAll();
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

	private void toggleSettingsBool(bool toggle)
	{
		inSettings = toggle;
	}
}
