using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class GameInitiation : MonoBehaviour
{
    public int[] scores;
    public bool resetSave;

    // Start is called before the first frame update
    void Start()
    {
        try
		{
            QuickSaveReader quickSaverReader = QuickSaveReader.Create("Scores");

            if (resetSave)
            {
                QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create("Scores");
                quickSaveWriter.Delete("Scores");
                PreLoadScores();
            }

            LoadScores();
        } catch (QuickSaveException e)
		{
            PreLoadScores();
            LoadScores();
		}
    }

    private void PreLoadScores()
	{
        QuickSaveWriter.Create("Scores")
            .Write("Level1", 99)
            .Write("Level2", 99)
            .Write("Level3", 99)
            .Write("Level4", 99)
            .Write("Level5", 99)
            .Write("Level6", 99)
            .Write("Level7", 99)
            .Write("Level8", 99)
            .Write("Level9", 99)
            .Commit();
    }

    private void LoadScores()
	{
        QuickSaveReader quickSaveReader = QuickSaveReader.Create("Scores");
        IEnumerable<string> keys = quickSaveReader.GetAllKeys();
        int counter = 8;
        foreach (string key in keys)
		{
            scores[counter] = quickSaveReader.Read<int>(key);
            counter--;
		}
        DataHolder.highScores = scores;
	}
}
