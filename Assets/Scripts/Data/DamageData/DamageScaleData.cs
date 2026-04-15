using System;
using UnityEngine;

[Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    public float physical = 1;
    public float elemental = 1;

    [Header("Accelerate")]
    public float accDuration = 5;
    public float accMultiplier = 1f;

    [Header("Burn")]
    public float burnDuration = 3;
    public float burnDamageScale = 1;

    [Header("Shock")]
    public float shockDuration = 3;
    public float shockDamageScale = 1;
    public float shockCharge = .4f;
}
