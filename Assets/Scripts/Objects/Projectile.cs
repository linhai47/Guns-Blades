using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("发送者")]
    public Entity entity;

    [Header("目标")]
    public Transform targetTransform;

    [SerializeField] protected LayerMask whatIsPlayer;
    [Header("速度")]
    public float speed;
    [Header("伤害逻辑")]
    public DamageScaleData damageScaleData;
    protected Entity_Stats enemyStats;
    protected bool targetGotHit;
    [SerializeField] protected GameObject onHitVfx;
    protected Rigidbody2D rb;
    protected Collider2D co;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        co = GetComponent<Collider2D>();
 
 
    }
    protected virtual void Update()
    {
           
    }
    public void SetEntity(Entity entity)
    {
        this.entity = entity;
        enemyStats = entity.GetComponent<Entity_Stats>();

    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       
    }

    protected virtual void ApplyDamageToTarget(Collider2D target, float damageScaleNumber = 1f)
    {

        IDamagable damagable = target.GetComponent<IDamagable>();

        if (enemyStats == null)
        {
            Debug.Log("EnemyStats is null");
        }
        AttackData attackData = enemyStats.GetAttackData(damageScaleData);

        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();


        float physicsDamage = attackData.physicalDamage * damageScaleNumber;
        float elementalDamage = attackData.elementalDamage * damageScaleNumber;
        ElementType element = attackData.element;
        Entity attackEntity = attackData.attackEntity;
        bool isCrit = attackData.isCrit;
        targetGotHit = damagable.TakeDamage(physicsDamage, elementalDamage, element, transform, isCrit);


        if (element != ElementType.None)
        {
            statusHandler.ApplyStatusEffect(element, attackData.effectData, attackEntity);
        }
        if (targetGotHit)
        {

            Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
  
        }
      
    }
}
