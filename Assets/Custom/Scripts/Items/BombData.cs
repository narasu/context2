﻿using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "ScriptableObjects/Bomb", order = 0)]
public class BombData : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    [Range(.0f, 32.0f)]public float ThrowForce;
    public GameObject SpawnOnImpact;
    public float ImpactEffectTime;

}
