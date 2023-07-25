using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class GameInitiation : MonoBehaviour
{
    public int[] scores;
    public bool resetSave;
    public bool resetVolume;

    // Start is called before the first frame update
    void Awake()
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

        try
		{
            QuickSaveReader quickSaveReader = QuickSaveReader.Create("Volume");

            if (resetVolume)
			{
                QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create("Volume");
                quickSaveWriter.Delete("Volume");
                PreLoadVolume();
			}

            LoadVolume();
		} catch (QuickSaveException e)
		{
            PreLoadVolume();
            LoadVolume();
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

    private void PreLoadVolume()
	{
        QuickSaveWriter.Create("Volume")
            .Write("MasterVolume", .5f)
            .Write("MusicVolume", 1f)
            .Write("EffectsVolume", 1f)
            .Write("StoredMasterVolume", .5f)
            .Write("StoredMusicVolume", 1f)
            .Write("StoredEffectsVolume", 1f)
            .Write("MasterMute", false)
            .Write("MusicMute", false)
            .Write("EffectsMute", false)
            .Commit();

    }

    private void LoadVolume()
	{
        QuickSaveReader quickSaveReader = QuickSaveReader.Create("Volume");
        DataHolder.masterVolume = quickSaveReader.Read<float>("MasterVolume");
        DataHolder.musicVolume = quickSaveReader.Read<float>("MusicVolume");
        DataHolder.effectsVolume = quickSaveReader.Read<float>("EffectsVolume");
        DataHolder.storedMasterVolume = quickSaveReader.Read<float>("StoredMasterVolume");
        DataHolder.storedMusicVolume = quickSaveReader.Read<float>("StoredMusicVolume");
        DataHolder.storedEffectsVolume = quickSaveReader.Read<float>("StoredEffectsVolume");
        DataHolder.masterMute = quickSaveReader.Read<bool>("MasterMute");
        DataHolder.musicMute = quickSaveReader.Read<bool>("MusicMute");
        DataHolder.effectsMute = quickSaveReader.Read<bool>("EffectsMute");
	}
}
