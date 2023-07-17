using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class DataHolder
{
    public static Action<ClubObject> addedClub;

    public static bool hasPutter;
    private static bool _hasputter;
    public static bool hasWedge;
    private static bool _haswedge;
    public static bool hasDriver;
    public static bool _hasdriver;

    public static ClubObject putter = Resources.Load("Clubs/Putter") as ClubObject;
    public static ClubObject wedge = Resources.Load("Clubs/Wedge") as ClubObject;
    public static ClubObject driver = Resources.Load("Clubs/Driver") as ClubObject;

    public static int currentClubIndex = 0;

    public static void UpdateClub()
	{
        if (hasPutter != _hasputter)
		{
            addedClub?.Invoke(putter);
            _hasputter = true;
		}
        if (hasWedge != _haswedge)
        {
            addedClub?.Invoke(wedge);
            _haswedge = true;
        }
        if (hasDriver != _hasdriver)
        {
            addedClub?.Invoke(driver);
            _hasdriver = true;
        }
    }
}
