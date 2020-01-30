using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Planet Stats", menuName = "Game Objects/Planet Stats")]
public class PlanetStats : ScriptableObject
{
    public float size;
    public int regeneration;
    public float defenseBoost;
}
