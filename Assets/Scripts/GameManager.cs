using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GameManager : MonoBehaviour
{
	[SerializeField] private TransitionSettings transition;

	public LevelRules levelRules;
	public static Action<bool> pausedGame;
	public static Action<bool> openedSettings;
	public static Action<bool> openedControls;
	public static Action isHighScore;
	private bool isPaused;
	private bool inSettings;
	private bool inControls;

	public int[] scores;

	#region Input Actions
	[SerializeField] private PlayerInput playerInput;
	private InputAction restartAction;
	private InputAction pauseAction;
	private InputAction timeAction;
	#endregion

	private void OnEnable()
	{
		restartAction.Enable();
		pauseAction.Enable();
		timeAction.Enable();

		restartAction.started += RestartLevelActionEvent;
		timeAction.started += SpeedUpTime;
		timeAction.canceled += SlowDownTime;
		pauseAction.started += PauseGameActionEvent;

		CheckForEnd.levelComplete += EndLevel;
		WaterScript.StuckInWater += RestartLevel;
		UIManager.pressedButton += DetermineButtonAction;
		BallPhysics.ballOffCameraX += RestartLevel;
		UIManager.inSettings += toggleSettingsBool;
		UIManager.inControls += toggleControlsBool;
	}

	private void OnDisable()
	{
		restartAction.Disable();
		pauseAction.Disable();
		timeAction.Disable();

		restartAction.started -= RestartLevelActionEvent;
		timeAction.started -= SpeedUpTime;
		timeAction.canceled -= SlowDownTime;
		pauseAction.started -= PauseGameActionEvent;

		CheckForEnd.levelComplete -= EndLevel;
		WaterScript.StuckInWater -= RestartLevel;
		UIManager.pressedButton -= DetermineButtonAction;
		BallPhysics.ballOffCameraX -= RestartLevel;
		UIManager.inSettings -= toggleSettingsBool;
		UIManager.inControls -= toggleControlsBool;
	}

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		restartAction = playerInput.actions["RestartLevel"];
		pauseAction = playerInput.actions["Pause"];
		timeAction = playerInput.actions["SpeedUpTime"];
		
		DataHolder.Reset(levelRules);
	}

	private void Start()
	{
		levelRules.LoadRules();
		scores = DataHolder.highScores;
	}

	private void Update()
	{

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
		TransitionManager.Instance().Transition(scene.buildIndex, transition, DataHolder.loadDelay);
		//SceneManager.LoadScene(scene.buildIndex);
		DataHolder.Reset(levelRules);
		DOTween.CompleteAll();
	}
	
	public void RestartLevelActionEvent(InputAction.CallbackContext callbackContext)
	{
		Scene scene = SceneManager.GetActiveScene();
		TransitionManager.Instance().Transition(scene.buildIndex, transition, DataHolder.loadDelay);
		//SceneManager.LoadScene(scene.buildIndex);
		DataHolder.Reset(levelRules);
		DOTween.CompleteAll();
	}

	private void NextLevel()
	{
		Scene scene = SceneManager.GetActiveScene();
		TransitionManager.Instance().Transition(scene.buildIndex + 1, transition, DataHolder.loadDelay);
		//SceneManager.LoadScene(scene.buildIndex + 1);
		DataHolder.Reset(levelRules);
		DOTween.CompleteAll();
	}

	private void ReturnToMenu()
	{
		Time.timeScale = 1;
		TransitionManager.Instance().Transition("MainMenu", transition, DataHolder.loadDelay);
		//SceneManager.LoadScene("MainMenu");
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

	private void toggleControlsBool(bool toggle)
	{
		inControls = toggle;
	}

	private void SpeedUpTime(InputAction.CallbackContext callbackContext)
	{
		if (DataHolder.isPaused)
			return;
		Time.timeScale = 2;
	}

	private void SlowDownTime(InputAction.CallbackContext callbackContext)
	{
		Time.timeScale = 1;
	}

	private void PauseGameActionEvent(InputAction.CallbackContext callbackContext)
	{
		if (!isPaused)
		{
			PauseGame();
			isPaused = !isPaused;
		}
		else
		{
			if (inSettings)
			{
				openedSettings?.Invoke(false);
			} else if (inControls)
			{
				openedControls?.Invoke(false);
			}
			else
			{
				UnpauseGame();
				isPaused = !isPaused;
			}
		}
	}
}
