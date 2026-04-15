using Unity.VisualScripting;
using UnityEngine;

public class BossObjects : MonoBehaviour
{
    public Boss boss;

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


    public bool isBullet = false;
  
    [Header("MoveCurve 控制（如果为空，则速度恒定）")]
    public AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Tooltip("如果为 null 或空，子弹按 flyingSpeed 恒定移动。推荐曲线范围在 0..1 或更高（倍率）。")]
    public float minSpeedMultiplier = 0f; // 可选：当曲线在 0 时，也让速度最小为这个值（防止完全静止）
    public float bulletDuration = 3f;
    protected bool curveActive = false;
    private float timer = 0f;

    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyStats = boss.GetComponent<Entity_Stats>();
    }

    protected virtual void start()
    {


    }

    // EnemyObjects.cs
    public virtual void Setup(Boss owner, bool isFlying = true, float angle = 0f, Vector3? Dir = null)
    {
        if (Dir == null)
            Dir = new Vector3(1, 0, 0); // 在运行时赋默认值

        Vector3 actualDir = Dir.Value;
        if (owner == null)
        {
            Debug.LogError("[EnemyObjects.Setup] owner 为 null，调用方必须传入 Enemy 实例！");
            return;
        }

        boss = owner;

        // 优先使用 enemy 已有的 stats 引用，否则尝试自动查找
        if (boss.stats != null)
        {
            enemyStats = boss.stats;
        }
        else
        {
            // 尝试在 enemy gameObject 上查找（根/子/父）
            enemyStats = boss.GetComponent<Entity_Stats>()
                       ?? boss.GetComponentInChildren<Entity_Stats>()
                       ?? boss.GetComponentInParent<Entity_Stats>();
            if (enemyStats != null)
                Debug.LogWarning($"[EnemyObjects.Setup] 自动找到并赋值 ObjectsStats -> {enemyStats.name}");
        }

        if (enemyStats == null)
        {
            Debug.LogError($"[EnemyObjects.Setup] 无法为 {boss.name} 找到 Entity_Stats（ObjectsStats 为空）！请检查组件挂载或赋值顺序。");
            // 视情况 return 或继续（建议 return）
            return;
        }

        // 其余原有初始化
        damageScaleData = boss.GetComponent<Entity_Combat>()?.basicAttackScale;
        if (damageScaleData == null) Debug.LogWarning("[EnemyObjects.Setup] damageScaleData 为 null");

        facingDir = boss.facingDir;

        if (isFlying) SetDirection(actualDir);
        if(isBullet)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
     
        curveActive = isBullet && moveCurve != null && flyingDistance > 0f;
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
            lastTarget = target.transform;
            Debug.Log("Hit On");
            Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
            if (isBullet)
            {
                Destroy(gameObject);
            }
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


    protected void SetDirection(Vector3 Dir)
    {

        startPosition = transform.position;

        moveDirection = Dir;

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

    protected virtual void FixedUpdate()
    {
        if (isBullet && rb != null && curveActive)
        {
            UpdateBulletMovementByCurve();
        }
    }

    /// <summary>
    /// 使用已飞行距离 / flyingDistance 得到 t，然后用 moveCurve.Evaluate(t) 作为速度倍率，
    /// 最终速度 = moveDirection.normalized * flyingSpeed * multiplier
    /// </summary>
    protected void UpdateBulletMovementByCurve()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / bulletDuration);

        float curveValue = moveCurve != null ? moveCurve.Evaluate(t) : 1f;
        float currentSpeed = curveValue * flyingSpeed;


        Vector2 dir = transform.right.normalized;
      
        rb.linearVelocity = dir * currentSpeed;
       
    }


}
