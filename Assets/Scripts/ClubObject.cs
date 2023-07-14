using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Club", menuName = "ScriptableObjects/Create Club")]
public class ClubObject : ScriptableObject
{
    public string clubName;
    public float clubAngle;
    public float clubMaxPower = 1;
    public Sprite clubVisual;
    public Sprite UIVisual;
}
