using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamagable
{
    private Entity_Stats entityStats;
    private Entity_DropManager dropManager;
    private Slider healthBar;
    [Header("on Damage Knockback")]
    [SerializeField]
    private float knockBackDuration = .2f;
    [SerializeField] private Vector2 onDamageKnockback = new Vector2(1.5f, 2f);

    [Header("on Heavy Damage Knockback")]
    [SerializeField]
    private float heavyKnockBackDuration = .5f;
    [SerializeField] private float heavyKnockBackThreshold = .5f;
    [SerializeField] private Vector2 onHeavyDamageKnockback = new Vector2(7, 7);

    [Header("Health Regen")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    private Entity entity;
    private Entity_VFX entityVfx;
    

    public float currentHealth;

    public float lastDamageTaken { get; private set; }
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    public bool canBeKnockout = false;
    public float GetcurrentHealth() => currentHealth;
    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        dropManager = GetComponent<Entity_DropManager>();
        SetupHealth();
    }

    protected virtual void Start()
    {


    }

    private void SetupHealth()
    {
        if (entityStats != null)
        {

            currentHealth = entityStats.GetMaxHealth();
            OnHealthUpdate += UpdateHealthBar;

            UpdateHealthBar();
            InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
        }
    }


    public void EnableHealthBar(bool enable) => healthBar?.transform.parent.gameObject.SetActive(enable);
    private void UpdateHealthBar()
    {
        if (healthBar == null && healthBar.transform.parent.gameObject.activeSelf == false) return;
        healthBar.value = GetHealthPercent();
    }
    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();
    private void RegenerateHealth()
    {

        if (canRegenerateHealth == false) return;
        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }
    public void IncreaseHealth(float healAmount)
    {
        if (isDead) return;


        float newHealth = currentHealth + healAmount;


        currentHealth = Mathf.Min(newHealth, entityStats.GetMaxHealth());
        OnHealthUpdate?.Invoke();

    }

    public void ReduceHealth(float damage)
    {

        currentHealth -= damage;
        OnHealthUpdate?.Invoke();
        entityVfx?.PlayOnDamageVfx();
        if (currentHealth <= 0) Die();

    }
    protected virtual void Die()
    {
        if (canBeKnockout)
        {
            entity?.EntityKnockOut();
            return;
        }
        isDead = true;
        canTakeDamage = false;
    
        entity?.EntityDeath();
        dropManager?.DropItems();

    }

    public virtual bool TakeDamage(float damage,float ElementalDamage,ElementType element,  Transform damageDealer, bool isCrit = false)
    {
        if (isDead || !canTakeDamage) return false;
        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }
        HitStopController.Instance.StopTime(0.02f, 0.0f);
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;

        float physicalDamageTaken = damage*(1 - mitigation);

        float elementalDamageTaken = ElementalDamage *( 1 - resistance);
        
        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;
        OnTakingDamage?.Invoke();
        return true;
    }
    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);
        entity?.ReceiveKnockback(knockback, duration);
    }
    private Vector2 CalculateKnockback(float damage,Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;
        knockback.x = knockback.x * direction;
        return knockback;


    }
    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();



    }
    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;
    private float CalculateDuration(float damage)
    {
        return IsHeavyDamage(damage) ? heavyKnockBackDuration : knockBackDuration;


    }


    private bool IsHeavyDamage( float damage)
    {
        
        return damage/ entityStats.GetMaxHealth() > heavyKnockBackThreshold;

    }
}
