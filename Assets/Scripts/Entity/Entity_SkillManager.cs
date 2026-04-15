using UnityEngine;

public class Entity_SkillManager : MonoBehaviour
{
    public SkillBase[] allSkills { get; private set; }

    public Skill_Windblade windBlade { get; private set; }

    public Skill_TornadoSword tornadoSword { get; private set; }

    public Skill_EyeofCyclone eyeofCyclone { get; private set; }

    public Skill_WindPulls windPulls { get; private set; }

    public Skill_StormRools stormRools { get; private set; }

    public Skill_DrawSword drawSword { get; private set; }

    public Skill_ShadowlessFist shadowlessFist { get; private set; }

    public SkillType[] playerSkillSlots;
    public float globalCastingTime = 0f;
    private void Awake()
    {

        allSkills = GetComponentsInChildren<SkillBase>();

        windBlade = GetComponentInChildren<Skill_Windblade>();

        tornadoSword = GetComponentInChildren<Skill_TornadoSword>();

        eyeofCyclone = GetComponentInChildren<Skill_EyeofCyclone>();

        windPulls = GetComponentInChildren<Skill_WindPulls>();

        stormRools = GetComponentInChildren<Skill_StormRools>();

        drawSword = GetComponentInChildren<Skill_DrawSword>();

        shadowlessFist = GetComponentInChildren <Skill_ShadowlessFist>();
        
    }
    private void Update()
    {
        if (globalCastingTime > 0f)
            globalCastingTime -= Time.deltaTime;

    }
    public void ReduceAllSkillCooldownBy(float amount)
    {
        foreach (var skill in allSkills)
        {
            skill.ReduceCooldownBy(amount);
        }
    }


    public SkillBase GetSKillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.WindBlade: return windBlade;

            case SkillType.TornadoSword: return tornadoSword;

            case SkillType.EyeOfCyclone: return eyeofCyclone;

            case SkillType.WindPulls: return windPulls;

            case SkillType.StormRools: return stormRools;

            case SkillType.DrawSword: return drawSword;

            case SkillType.ShadowlessFist: return shadowlessFist;

            default:
                Debug.Log($"Skill type {type} is not implemented yet");
                return null;
        }

    }


}
