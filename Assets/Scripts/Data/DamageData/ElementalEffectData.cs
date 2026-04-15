using UnityEngine;
using System;

[Serializable]
public class ElementalEffectData
{
    public float accDuration;
    public float accMultiplier;

    public float burnDuration;
    public float totalBurnDamage;

    public float shockDuration;
    public float shockDamage;
    public float shockCharge;


    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)
    {
        accDuration = damageScale.accDuration;
        accMultiplier = damageScale.accMultiplier;

        burnDuration = damageScale.burnDuration;
        totalBurnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.shockDamageScale;
        shockCharge = damageScale.shockCharge;

        //burnDamage = entityStats.offense.fireDamage.GetValue() * scaleFactor.burnDamageScale;
        // elemental shard does 200% elemental damage

    }
}


//public class ScaleFactor
//{
//    public float burnDamageScale = .5f;

//}