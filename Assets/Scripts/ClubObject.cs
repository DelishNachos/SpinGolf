using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Club", menuName = "ScriptableObjects/Create Club")]
public class ClubObject : ScriptableObject
{
    public string clubName;
    public float clubAngle;
    public float clubPower = 1;
}
