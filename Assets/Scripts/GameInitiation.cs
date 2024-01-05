using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class GameInitiation : MonoBehaviour
{
    [SerializeField] private int[] scores;
    [SerializeField] private float[] times;
    public Texture2D cursor;
    public bool resetSave;
    public bool resetVolume;
    public int levelCount = 9;

    // Start is called before the first frame update
    void Awake()
    {
        scores = new int[levelCount];
        times = new float[levelCount];
        DataHolder.levelCount = levelCount - 1;

        //load scores
        try
		{
            QuickSaveReader quickSaverReader = QuickSaveReader.Create("Scores");

            if (resetSave)
            {
                ResetSave();
            } else
			{
                LoadScores();
			}

        } catch (QuickSaveException e)
		{
            PreLoadScores();
            LoadScores();
		}

        //load times
        try
        {
            QuickSaveReader quickSaverReader = QuickSaveReader.Create("Times");

            if (resetSave)
            {
                ResetSave();
            }
            else
            {
                LoadTimes();
            }

        }
        catch (QuickSaveException e)
        {
            PreLoadTimes();
            LoadTimes();
        }

        //load volume
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

    void Start()
	{
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
	}

    private void PreLoadScores()
	{
        QuickSaveWriter.Create("Scores")
            .Write("Level1", -1)
            .Write("Level2", -1)
            .Write("Level3", -1)
            .Write("Level4", -1)
            .Write("Level5", -1)
            .Write("Level6", -1)
            .Write("Level7", -1)
            .Write("Level8", -1)
            .Write("Level9", -1)
            .Commit();
    }

    public void LoadScores()
	{
        QuickSaveReader quickSaveReader = QuickSaveReader.Create("Scores");
        IEnumerable<string> keys = quickSaveReader.GetAllKeys();
        int counter = DataHolder.levelCount;
        foreach (string key in keys)
		{
            scores[counter] = quickSaveReader.Read<int>(key);
            counter--;
		}
        DataHolder.highScores = scores;
	}

    private void PreLoadTimes()
    {
        QuickSaveWriter.Create("Times")
            .Write("Level1", -1)
            .Write("Level2", -1)
            .Write("Level3", -1)
            .Write("Level4", -1)
            .Write("Level5", -1)
            .Write("Level6", -1)
            .Write("Level7", -1)
            .Write("Level8", -1)
            .Write("Level9", -1)
            .Commit();
    }

    public void LoadTimes()
	{
        QuickSaveReader quickSaveReader = QuickSaveReader.Create("Times");
        IEnumerable<string> keys = quickSaveReader.GetAllKeys();
        int counter = DataHolder.levelCount;
        foreach (string key in keys)
        {
            times[counter] = quickSaveReader.Read<float>(key);
            counter--;
        }
        DataHolder.lowTimes = times;
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

    public void ResetSave()
	{
        QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create("Volume");
        quickSaveWriter.Delete("Volume");
        PreLoadScores();
        LoadScores();
        PreLoadTimes();
        LoadTimes();
    }
}
