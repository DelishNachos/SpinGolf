using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Rules", menuName = "ScriptableObjects/Level Rules")]
public class LevelRules : ScriptableObject
{
    public bool hasPutter;
    public bool hasWedge;
    public bool hasIron;
    public bool hasDriver;

    public int startingIndex;

    public void LoadRules()
	{
        DataHolder.hasPutter = hasPutter;
        DataHolder.hasIron = hasIron;
        DataHolder.hasWedge = hasWedge;
        DataHolder.hasDriver = hasDriver;
        DataHolder.currentClubIndex = startingIndex;
        DataHolder.InitClubs();
	}
}
