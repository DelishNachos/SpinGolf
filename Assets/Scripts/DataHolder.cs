using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class DataHolder
{
    public static Action<ClubObject> addedClub;
    public static Action<int> hitBall;

    public static bool hasPutter;
    private static bool _hasputter;
    public static bool hasWedge;
    private static bool _haswedge;
    public static bool hasIron;
    private static bool _hasiron;
    public static bool hasDriver;
    public static bool _hasdriver;

    public static ClubObject putter = Resources.Load("Clubs/Putter") as ClubObject;
    public static ClubObject wedge = Resources.Load("Clubs/Wedge") as ClubObject;
    public static ClubObject iron = Resources.Load("Clubs/Iron") as ClubObject;
    public static ClubObject driver = Resources.Load("Clubs/Driver") as ClubObject;

    public static int currentClubIndex = 0;
    public static int hits = 0;

    public static bool isInWater = false;
    public static bool isInHole = false;

    public static void InitClubs()
	{
        if (hasPutter != _hasputter)
		{
            addedClub?.Invoke(putter);
            _hasputter = true;
		} 
        else
		{
            addedClub?.Invoke(null);
		}

        if (hasWedge != _haswedge)
        {
            addedClub?.Invoke(wedge);
            _haswedge = true;
        }
        else
        {
            addedClub?.Invoke(null);
        }

        if (hasIron != _hasiron)
        {
            addedClub?.Invoke(iron);
            _hasiron = true;
        }
        else
        {
            addedClub?.Invoke(null);
        }

        if (hasDriver != _hasdriver)
        {
            addedClub?.Invoke(driver);
            _hasdriver = true;
        }
        else
        {
            addedClub?.Invoke(null);
        }
    }

    public static void Reset(LevelRules LR)
	{
        hasPutter = false;
        _hasputter = false;
        hasWedge = false;
        _haswedge = false;
        hasIron = false;
        _hasiron = false;
        hasDriver = false;
        _hasdriver = false;
        currentClubIndex = LR.startingIndex;
        isInWater = false;
        isInHole = false;
        hits = 0;
	}

    public static void addHit()
	{
        hits++;
        hitBall?.Invoke(hits);
	}

    public static void initVars()
	{
        _hasputter = hasPutter;
        _haswedge = hasWedge;
        _hasiron = hasIron;
        _hasdriver = hasDriver;
	}

    public static void UpdateClub()
	{
        if (hasPutter != _hasputter)
        {
            addedClub?.Invoke(putter);
            _hasputter = true;
        } else if (hasWedge != _haswedge)
        {
            addedClub?.Invoke(wedge);
            _haswedge = true;
        } else if (hasIron != _hasiron)
        {
            addedClub?.Invoke(iron);
            _hasiron = true;
        } else if (hasDriver != _hasdriver)
        {
            addedClub?.Invoke(driver);
            _hasdriver = true;
        }
    }
}
