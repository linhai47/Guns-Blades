using UnityEngine;

public class EnemyObjects : MonoBehaviour
{
    public Enemy enemy;

    [SerializeField] protected GameObject onHitVfx;

    [Header("Collision Detection")]
    [SerializeField] protected float checkRadius = 1;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform targetCheck;

    [Header("Flying object ")]
    [SerializeField] protected float flyingSpeed = 15f;
    [SerializeField] protected float flyingDistance = 20f;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Entity_Stats enemyStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;
    protected bool targetGotHit;
    protected Transform lastTarget;

    protected int facingDir;
    protected Vector2 moveDirection;
    protected Vector2 startPosition;

    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void start()
    {


    }

    // EnemyObjects.cs
    public virtual void Setup(Enemy owner, bool isFlying = true)
    {
        if (owner == null)
        {
            Debug.LogError("[EnemyObjects.Setup] owner 为 null，调用方必须传入 Enemy 实例！");
            return;
        }

        enemy = owner;

        // 优先使用 enemy 已有的 stats 引用，否则尝试自动查找
        if (enemy.stats != null)
        {
            enemyStats = enemy.stats;
        }
        else
        {
            // 尝试在 enemy gameObject 上查找（根/子/父）
            enemyStats = enemy.GetComponent<Entity_Stats>()
                       ?? enemy.GetComponentInChildren<Entity_Stats>()
                       ?? enemy.GetComponentInParent<Entity_Stats>();
            if (enemyStats != null)
                Debug.LogWarning($"[EnemyObjects.Setup] 自动找到并赋值 ObjectsStats -> {enemyStats.name}");
        }

        if (enemyStats == null)
        {
            Debug.LogError($"[EnemyObjects.Setup] 无法为 {enemy.name} 找到 Entity_Stats（ObjectsStats 为空）！请检查组件挂载或赋值顺序。");
            // 视情况 return 或继续（建议 return）
            return;
        }

        // 其余原有初始化
        damageScaleData = enemy.GetComponent<Entity_Combat>()?.basicAttackScale;
        if (damageScaleData == null) Debug.LogWarning("[EnemyObjects.Setup] damageScaleData 为 null");

        facingDir = enemy.facingDir;

        if (isFlying) SetDirection();
    }


    protected void Circle_AOE_Damage(float radius, Transform t)
    {

        foreach (var target in GetEnemiesInCircle(t, radius))
        {
            ApplyDamageToTarget(target);

        }



    }

    protected void Single_Damage(Transform t, float radius)
    {
        ApplyDamageToTarget(GetSingleEnemy(t, radius));


    }

    protected void ApplyDamageToTarget(Collider2D target, float damageScaleNumber = 1f)
    {

        IDamagable damagable = target.GetComponent<IDamagable>();

        if(enemyStats == null)
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
            lastTarget = target.transform;
            Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
        }
        usedElement = element;
    }

    protected Collider2D[] GetEnemiesInCircle(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsPlayer);


    }

    protected Collider2D GetSingleEnemy(Transform t, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(t.position, radius, whatIsPlayer);

        Collider2D closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(t.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = hit;
            }
        }

        return closest;



    }



    protected Transform FindClosestTarget(Transform t, float radius)
    {
        Transform target = null;

        float closestDistance = Mathf.Infinity;

        foreach (var enemy in GetEnemiesInCircle(t, radius))
        {
            float distance = Vector2.Distance(t.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }

        return target;

    }

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
        {
            targetCheck = transform;
        }
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }


    protected void SetDirection()
    {

        startPosition = transform.position;

        moveDirection = new Vector2(facingDir, 0);

        if (facingDir == -1)
        {

            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        rb.linearVelocity = moveDirection * flyingSpeed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
