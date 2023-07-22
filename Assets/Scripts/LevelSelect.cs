using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    public TextMeshProUGUI[] highScores;

    // Start is called before the first frame update
    void Start()
    {
        if (DataHolder.highScores == null)
		{
            for (int i = 0; i < highScores.Length; i++)
			{
                highScores[i].text = "Hits:-";
			}
		} else
		{
            for(int i = 0; i < highScores.Length; i++ )
		    {
                highScores[i].text = "Hits:" + DataHolder.highScores[i].ToString();
		    }
		}
    }

    public void GoToLevel(int level)
	{
        SceneManager.LoadScene(level);
	}
}
