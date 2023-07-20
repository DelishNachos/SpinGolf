using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayButton()
	{
		SceneManager.LoadScene("Level 1");
	}

	public void LevelsButton()
	{
		SceneManager.LoadScene("LevelSelect");
	}

	public void SettingsButton()
	{
		Debug.Log("Settings");
	}

	public void QuitButton()
	{
		Application.Quit();
	}
}
