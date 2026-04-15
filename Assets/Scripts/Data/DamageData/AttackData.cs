using UnityEngine;
using System;

[Serializable]
public class AttackData
{
    public float physicalDamage;
    public float elementalDamage;
    public bool isCrit;
    public ElementType element;
    public Entity attackEntity;
    public ElementalEffectData effectData;
    public AttackData(Entity_Stats entityStats, DamageScaleData ScaleData, ElementType element ,Entity attackEntity)
    {
        this.attackEntity = attackEntity;
        this.element =element;
        physicalDamage = entityStats.GetPhyiscalDamage(out isCrit, ScaleData.physical);
        elementalDamage = entityStats.GetElementalDamage(this.element, ScaleData.elemental);
        effectData = new ElementalEffectData(entityStats, ScaleData);


    }



}
