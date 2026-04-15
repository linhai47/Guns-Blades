using UnityEngine;

public enum EnemyRarity { Normal, Elite, Boss }

public class Enemy_Level : MonoBehaviour
{
    [Header("敌人基础信息")]
    public int enemyLevel = 1;
    public EnemyRarity rarity = EnemyRarity.Normal;

    [Header("经验参数")]
    public float baseFactor = 10f;        // 基础倍率
    public float hpNormalization = 50f;   // HP归一化基准
    public int minXP = 1;
    public int maxXP = 0;                 // 0 = 不限制

    private Enemy_Health health;   // 引用血量脚本


    private void Awake()
    {
        health = GetComponent<Enemy_Health>();
    }

  
    public int GetExpReward(int playerLevel)
    {
        if (health == null) return 0;

        // 基础XP
        float baseXP = baseFactor * enemyLevel * (health.maxHealth / hpNormalization);

        // 等级差修正
        float levelDiff = enemyLevel - playerLevel;
        float levelDiffMultiplier = 1f + 0.10f * levelDiff;
        levelDiffMultiplier = Mathf.Clamp(levelDiffMultiplier, 0.5f, 3.0f);

        // 稀有度修正
        float rarityMult = 1f;
        switch (rarity)
        {
            case EnemyRarity.Elite: rarityMult = 2f; break;
            case EnemyRarity.Boss: rarityMult = 6f; break;
        }

        float xp = baseXP * levelDiffMultiplier * rarityMult;

        // Clamp & Round
        int finalXP = Mathf.RoundToInt(xp);
        if (finalXP < minXP) finalXP = minXP;
        if (maxXP > 0) finalXP = Mathf.Min(finalXP, maxXP);
        return finalXP;
    }


    public void GrantExp()
    {
        if (Player.instance == null) return;
        Player_Exp  playerExp = Player.instance.playerExp;
        int xp = GetExpReward(playerExp.playerLevel);
        playerExp.AddExp(xp);

        Debug.Log($"{name} 被击杀，奖励 {xp} EXP");
    }
}
