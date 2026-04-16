using UnityEngine;

// 确保挂载物体有刚体和碰撞体
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("发送者状态（自动获取）")]
    protected Entity_Stats shooterStats;
    protected Entity shooterEntity;

    [Header("基类配置")]
    [Tooltip("子弹飞向哪个层级会造成伤害")]
    [SerializeField] protected LayerMask whatIsTarget; // 把原本的 whatIsPlayer 改为更通用的名字

    [Header("视觉与物理（自动获取）")]
    protected Rigidbody2D rb;
    [SerializeField] protected GameObject onHitVfx; // 撞击特效预制体

    // 子弹自身属性（由枪注入）
    [HideInInspector] public float speed;
    [HideInInspector] public WeaponDataSO weaponData;

    protected bool targetGotHit; // 记录是否真正命中了有效目标

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 强制把刚体设为 Kinematic，防止被敌人撞飞，运动完全由代码控制
        rb.bodyType = RigidbodyType2D.Kinematic;
        // 强制把碰撞体设为 Trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    // [关键小手术] 初始化子弹，由 RangedWeapon 调用
    public virtual void SetupProjectile(Entity shooter, WeaponDataSO data, Vector2 direction)
    {
        shooterEntity = shooter;
        shooterStats = shooter.GetComponent<Entity_Stats>();
        weaponData = data;
        speed = data.bulletSpeed; // 假设你在SO里加了这个字段

        // 设置速度向量（如果是水平发射，枪口right轴通常就是方向）
        rb.linearVelocity = direction * speed;

        // [防漏网之鱼] 5秒后强制销毁，防止子弹飞出地图导致内存泄漏
        Destroy(gameObject, 5f);
    }

    // [基类通用逻辑] 通用碰撞处理
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 检查层级是否匹配
        if (((1 << collision.gameObject.layer) & whatIsTarget.value) != 0)
        {
            // 2. 尝试造成伤害
            ApplyDamageToTarget(collision);

            // 3. 撞击后逻辑：产生特效并立刻销毁子弹
            if (targetGotHit)
            {
                if (onHitVfx) Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
                Destroy(gameObject); // 子弹完成使命，退场
            }
        }
        // 4. [可选] 撞到墙壁层也销毁
        // else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) { ... }
    }

    // [保留你原本强大的伤害计算逻辑]
    protected virtual void ApplyDamageToTarget(Collider2D targetCollider, float damageScaleNumber = 1f)
    {
        IDamagable damagable = targetCollider.GetComponent<IDamagable>();
        if (damagable == null) return;

        // [复用你的 GetAttackData] 这里用 shooterStats
        if (shooterStats == null) { Debug.LogError($"{name} ShooterStats is null"); return; }

        // 注意：这里的 damageScaleData 需要你在 SO 里定义好
        AttackData attackData = shooterStats.GetAttackData(weaponData.damageScaleData);

        // 造成伤害（传入当前子弹的变换，用于计算击退方向）
        targetGotHit = damagable.TakeDamage(
            attackData.physicalDamage * damageScaleNumber,
            attackData.elementalDamage * damageScaleNumber,
            attackData.element,
            transform, // 子弹位置
            attackData.isCrit);

        if (targetGotHit)
        {
            // 挂状态
            targetCollider.GetComponent<Entity_StatusHandler>()?.ApplyStatusEffect(attackData.element, attackData.effectData, shooterEntity);
        }
    }
}