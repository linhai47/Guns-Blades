using UnityEngine;

public class Boss_Health :Entity_Health,IDamagable
{
    private Boss boss;
    private Boss_Level level;
    private Player_QuestManager questManager;
    public GameObject damagePopupPrefab;
    public float maxHealth;

    protected override void Awake()
    {
        base.Awake();


    }
    protected override void Start()
    {
        base.Start();
        boss = GetComponent<Boss>();
        maxHealth = boss.stats.GetMaxHealth();
        level = GetComponent<Boss_Level>();
        questManager = Player.instance.questManager;
    }

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit = false)
    {
        if (canTakeDamage == false)
        {
            return false;

        }

        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);



        if (isDead || wasHit == false) return false;



        if (damageDealer.GetComponent<Player>() != null || damageDealer.GetComponent<SkillObject_Base>() != null )
        {
             boss.TryEnterBattleState(damageDealer);

        }
        else
        {
            Debug.Log(damageDealer);
        }
        if (damagePopupPrefab != null)
        {
            Vector3 popupPos = transform.position + Vector3.up * 1.2f; // Õ∑∂•¬‘…œ∑Ω
            int shownDamage = Mathf.RoundToInt(damage + elementalDamage);
            DamagePopupPool.Instance.Spawn(popupPos, shownDamage, element, isCrit);
        }
        return true;
    }
    protected override void Die()
    {
        base.Die();
        if (questManager == null)
        {
            Debug.Log("QuestManager is NULL");
            questManager = Player.instance.questManager;
        }
        questManager.AddProgress(boss.questTargetId);


        level.GrantExp();

        Debug.Log(boss.questTargetId);
    }
}
