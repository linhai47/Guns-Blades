using TMPro;
using UnityEngine;

public class UI_StatToolTip : UI_ToolTip
{
    private Player_Stats playerStats;

    private TextMeshProUGUI statToolTipText;


    protected override void Awake()
    {
        base.Awake();

        playerStats = FindFirstObjectByType<Player_Stats>();

        statToolTipText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ShowToolTip(bool show, RectTransform targetRect, StatType statType)
    {
        base.ShowToolTip(show, targetRect);
        statToolTipText.text = GetStatTextByType(statType);
    }

    public string GetStatTextByType(StatType type)
    {
        switch (type)
        {
            // Major Attributes
            case StatType.Strength:
                return "每点力量增加 1 点物理伤害。" +
                       "\n 每点力量增加 0.5% 暴击伤害。";
            case StatType.Agility:
                return "每点敏捷增加 0.3% 暴击率。" +
                       "\n 每点敏捷增加 0.5% 闪避率。";
            case StatType.Intelligence:
                return "每点智力增加 0.5% 元素抗性。" +
                       "\n 每点智力额外增加 1 点元素伤害。" +
                       "\n 如果所有元素伤害均为 0，则不会获得额外加成。";
            case StatType.Vitality:
                return "每点体力增加 5 点最大生命值。" +
                       "\n 每点体力增加 1 点护甲值。";

            // Physical Damage
            case StatType.Damage:
                return "决定你的攻击物理伤害。";
            case StatType.CritChance:
                return "决定你的攻击产生暴击的几率。";
            case StatType.CritPower:
                return "提高暴击时造成的额外伤害。";
            case StatType.ArmorReduction:
                return "决定攻击时无视目标护甲的百分比。";
            case StatType.AttackSpeed:
                return "决定你的攻击速度。";

            // Defense
            case StatType.MaxHealth:
                return "决定你的总生命值。";
            case StatType.HealthRegen:
                return "每秒恢复的生命值。";
            case StatType.Armor:
                return "减少你受到的物理伤害。"
                    + "\n 护甲减伤上限为 85%。"
                    + "当前减伤为: " + playerStats.GetArmorMitigation(0) * 100 + "%。";
            case StatType.Evasion:
                return "完全闪避攻击的几率。" + "\n 上限为 85%。";

            // Elemental Damage
            case StatType.WindDamage:
                return "决定你的风属性攻击伤害。";
            case StatType.FireDamage:
                return "决定你的火属性攻击伤害。";
            case StatType.LightningDamage:
                return "决定你的雷属性攻击伤害。";
            //case StatType.ElementalDamage:
            //    return
            //        "元素伤害结合了三种元素。 " +
            //        "\n 最高的元素决定状态效果并造成全部伤害。" +
            //        "\n 另外两个元素会各自贡献其伤害的 50% 作为加成。";

            // Elemental Resistances
            case StatType.WindResistance:
                return "减少受到的风属性伤害。";
            case StatType.FireResistance:
                return "减少受到的火属性伤害。";
            case StatType.LightningResistance:
                return "减少受到的雷属性伤害。";

            default:
                return "该属性暂无说明。";
        }
    }

}
