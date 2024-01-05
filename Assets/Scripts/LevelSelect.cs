using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using EasyTransition;

public class LevelSelect : MonoBehaviour
{
    public TextMeshProUGUI[] highScores;
	public TextMeshProUGUI[] lowTimes;
	[SerializeField] private TransitionSettings transition;

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
                if (DataHolder.highScores[i] == -1)
				{
                    highScores[i].text = "Hits:-";
				} else
				{
					highScores[i].text = "Hits:" + DataHolder.highScores[i].ToString();
				}
		    }
		}

		if (DataHolder.lowTimes == null)
		{
			for (int i = 0; i < lowTimes.Length; i++)
			{
				lowTimes[i].text = "----";
			}
		}
		else
		{
			for (int i = 0; i < lowTimes.Length; i++)
			{
				if (DataHolder.lowTimes[i] == -1)
				{
					lowTimes[i].text = "----";
				}
				else
				{
					lowTimes[i].text = DataHolder.lowTimes[i].ToString("#.00").Replace(".", ":");
				}
			}
		}
	}

    public void GoToLevel(int level)
	{
		TransitionManager.Instance().Transition(level, transition, DataHolder.loadDelay);
		//SceneManager.LoadScene(level);
	}
}
