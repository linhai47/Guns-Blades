using System;
using UnityEngine;
[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]
public class SkillDataSO : ScriptableObject
{

    [Header("Skill Description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock & Upgrade")]
    public int cost;
    public BasicSkillType basicSkilltype;
    public bool unlockedByDefault;
    public SkillData skillData;

}
[Serializable]

public class SkillData
{
    public SkillType skillType;
    public float cooldown;

    public DamageScaleData damageScaleData;
}