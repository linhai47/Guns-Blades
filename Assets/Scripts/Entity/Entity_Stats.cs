using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public StatSetupSO defaultStatSetup;

    public Entity attackEntity;

    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;
    public Stat_ResourceGroup resources;
    public ElementType nowElement = ElementType.None;


    protected virtual void Awake()
    {
        // 给nowElement 赋值 可能应该得在start上做？
        attackEntity = GetComponent<Entity>();

    }

    protected virtual void Start()
    {


    }


    public AttackData GetAttackData(DamageScaleData scaleData)
    {


        return new AttackData(this, scaleData, nowElement, attackEntity);

    }


    public float GetElementalDamage(ElementType element, float scaleFactor = 1)
    {

        float fireDamage = offense.fireDamage.GetValue();
        float windDamage = offense.windDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue();

        float elementDamage = fireDamage;
        if(element == ElementType.Wind) elementDamage = windDamage;

        else if(element == ElementType.Lightning) elementDamage = lightningDamage;

        else if(element == ElementType.None) elementDamage = bonusElementalDamage;

            float finalDamage = elementDamage + bonusElementalDamage;

        return finalDamage * scaleFactor;
         

    }
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f; // Bonus resistance from intelligence: +0.5% per INT

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireRes.GetValue();
                break;
            case ElementType.Wind:
                baseResistance = defense.windRes.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningRes.GetValue();
                break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f; // Resistance will be capped at 75%;
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100; // Convert value into 0 to 1 multiplier

        return finalResistance;
    }
    public float GetPhyiscalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower() / 100; // Total crit power as multiplier ( e.g 150 / 100 = 1.5f - multiplier)

        isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalDamage * scaleFactor;
    }
    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        //Debug.Log(finalMaxHealth);
        return finalMaxHealth;
    }


    public float GetAttackSpeed()
    {
        float basicAttackSpeed = offense.attackSpeed.GetValue();

        float bonusAttackSpeed = major.agility.GetValue() * 0.03f;
        return basicAttackSpeed + bonusAttackSpeed;
    }

    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue();

    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * .3f);
   
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * .5f);
    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMutliplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMutliplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = .85f; // Max mitigation will be capped at 85%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }
    // Bonus armor from Vitality: +1 per VIT 
    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue();

    public float GetArmorReduction()
    {
        // Total armor reduction as multiplier ( e.g 30 / 100 = 0.3f - multiplier)
        float finalReduction = offense.armorReduction.GetValue() / 100;

        return finalReduction;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f; // Bonus evasion from Agility: +0.5% per AGI 

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f; // Max evasion will be capped at 85%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }
    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.WindDamage: return offense.windDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            case StatType.ElementalDamage: return null;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.WindResistance: return defense.windRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

        offense.windDamage.SetBaseValue(defaultStatSetup.windDamage);
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.windRes.SetBaseValue(defaultStatSetup.windResistance);
        defense.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
    }

}
