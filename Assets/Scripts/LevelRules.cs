using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Rules", menuName = "ScriptableObjects/Level Rules")]
public class LevelRules : ScriptableObject
{
    public bool hasPutter = true;
    public bool hasWedge = true;
    public bool hasIron = true;
    public bool hasDriver = true;

    public int startingIndex = 0;

    public void LoadRules()
	{
        //DataHolder.Reset(this);
        DataHolder.hasPutter = hasPutter;
        DataHolder.hasIron = hasIron;
        DataHolder.hasWedge = hasWedge;
        DataHolder.hasDriver = hasDriver;
        DataHolder.currentClubIndex = startingIndex;
        DataHolder.InitClubs();
	}
}
