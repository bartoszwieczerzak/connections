using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Planet Stats", menuName = "Game Objects/Planet Stats")]
public class PlanetStats : ScriptableObject
{
    [Range(0.5f, 2.0f)]
    public float size = 1.0f;

    [Range(1, 10)]
    public int populationGrowth = 1;

    [Range(1.0f, 5.0f)]
    public float populationCycleTime = 3.0f;

    [Range(-5.0f, 5.0f)]
    public float defenseBonus = 0.0f;
}
