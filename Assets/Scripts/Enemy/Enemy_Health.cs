using UnityEngine;

public class Enemy_Health : Entity_Health , IDamagable
{
    private Enemy enemy;
    private Enemy_Level level;
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
        enemy = GetComponent<Enemy>();
        maxHealth = enemy.stats.GetMaxHealth();
        level = GetComponent<Enemy_Level>();
        questManager= Player.instance.questManager;
    }

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit = false)
    {
        if (canTakeDamage == false)
        {
            return false;

        }
        
        bool wasHit = base.TakeDamage(damage,elementalDamage,element,  damageDealer );



        if (isDead || wasHit == false) return false;

        if (damageDealer.GetComponent<Player>() != null || damageDealer.GetComponent<SkillObject_Base>()!=null )
        { 
            enemy.TryEnterBattleState(damageDealer); 
        
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
        if(questManager == null)
        {
            Debug.Log("QuestManager is NULL");
            questManager = Player.instance.questManager;
        }
        questManager.AddProgress(enemy.questTargetId);


        level.GrantExp();

        Debug.Log(enemy.questTargetId);
    }

}
