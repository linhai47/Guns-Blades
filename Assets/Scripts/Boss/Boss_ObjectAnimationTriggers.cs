using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Boss 的动画触发器：保留原有无参方法用于 AnimationEvent 调用，另外提供灵活的参数化发射方法供代码调用。
/// 包含详细的 Debug 输出以便调试。
/// </summary>
public class Boss_ObjectAnimationTriggers : Entity_AnimationTriggers
{
    public Boss boss;
    public UI_Boss uiboss;
    [Header("子弹/物体 Prefab")]
    public GameObject enemy_objects; // 子弹 prefab（应包含 BossObjects_SingleHitVfx 和 Rigidbody2D）

    [Header("跳跃特效")]
    public GameObject jump_objects;
    public Transform jump_transform;

    [Header("常用参数（供无参方法使用，方便 AnimationEvent）")]
    public Transform objectTransform;
    public float duration = 3f;

    [Tooltip("用于 AnimationEvent 的无参批量发射：发射个数")]
    public int bulletCount = 5;
    [Tooltip("用于 AnimationEvent 的无参批量发射：总体角度范围（度）")]
    public float spreadAngle = 30f;
    [Tooltip("用于 AnimationEvent 的无参批量发射：速度")]
    public float bulletSpeed = 6f;

    public float originAngle = 0;
    [Tooltip("是否在每次发射时随机偏移基准方向")]
    public bool ifRandomizeBaseDirection = false;
    [Tooltip("速度随机偏移（±），例如 5 表示速度在 speed-5 .. speed+5 之间随机")]
    public float bulletSpeedVariance = 5f;

    public float fireInterval = 0.05f;


    [Header("符卡圆环")]
    public GameObject spellCardCircle;
    public GameObject spellCardInnerCircle;
    protected override void Awake()
    {
        base.Awake();
        boss = GetComponentInParent<Boss>();
        uiboss = FindAnyObjectByType<UI_Boss>();
    }
    public void Create_JumpVfx()
    {
        GameObject jumpVfx = Instantiate(jump_objects, jump_transform.position, Quaternion.identity) ;
        Destroy(jumpVfx ,1f);


    }
    public void Create_SingleHitVfx()
    {
        Debug.Log("[Boss_ObjectAnimationTriggers] Create_SingleHitVfx called");
        if (enemy_objects == null || objectTransform == null)
        {
            Debug.LogError("[Create_SingleHitVfx] enemy_objects or objectTransform is null!");
            return;
        }

        GameObject singleHitVfx = Instantiate(enemy_objects, objectTransform.position, Quaternion.identity);
        BossObjects_SingleHitVfx casting_Objects = singleHitVfx.GetComponentInChildren<BossObjects_SingleHitVfx>();
        if (casting_Objects != null)
        {
            casting_Objects.duration = duration;
            casting_Objects.Setup(boss, false , 0);
        }
        else
        {
            Debug.LogWarning("[Create_SingleHitVfx] instantiated prefab has no BossObjects_SingleHitVfx component");
        }
        AddFuryCounter();
    }

    // 原来的无参生成（保留以便 Animator AnimationEvent 调用）
    public void Create_MultipleBullets()
    {

        float baseAngleDeg = originAngle;

        if (ifRandomizeBaseDirection)
        {
            // 完全随机基准角度（0..360）
            baseAngleDeg = Random.Range(0f, 360f);
        }

        // 将基准角度转换成方向向量
        Vector2 baseDir = new Vector2(Mathf.Cos(baseAngleDeg * Mathf.Deg2Rad), Mathf.Sin(baseAngleDeg * Mathf.Deg2Rad));

        // 使用 inspector 的速度偏移参数
        Create_Bullets(bulletCount, baseDir, spreadAngle, bulletSpeed, randomizeAnglePerBullet: true);
    }


    public void Create_Bullets(int count, Vector2 baseDirection, float totalSpreadAngleDeg, float speed, bool randomizeAnglePerBullet = false)
    {
        if (enemy_objects == null || objectTransform == null)
        {
            Debug.LogError("[Create_Bullets] enemy_objects or objectTransform is null!");
            return;
        }

        if (count <= 0)
        {
            Debug.LogWarning("[Create_Bullets] count <= 0, nothing to spawn");
            return;
        }

        // 确保 baseDirection 是单位向量，且在 2D 平面上
        if (baseDirection.sqrMagnitude <= 0.0001f)
        {
            baseDirection = Vector2.right;
           
        }
        baseDirection = baseDirection.normalized;

        // 计算起始角（左侧），然后按等间隔角度生成（如果 count=1 则角度为 base）
        float halfSpread = totalSpreadAngleDeg * 0.5f;
        for (int i = 0; i < count; ++i)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1); // 0..1
            float angleDeg = Mathf.Lerp(-halfSpread, halfSpread, t); // 在 -half..+half 之间
            if (randomizeAnglePerBullet)
            {
                float jitter = Random.Range(-totalSpreadAngleDeg / (count * 2f), totalSpreadAngleDeg / (count * 2f));
                angleDeg += jitter;
            }

            // 将 baseDirection 旋转 angleDeg
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
            float finalAngle = baseAngle + angleDeg;
            Vector2 finalDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad)).normalized;
            float finalSpeed = speed;
            if (bulletSpeedVariance > 0f)
            {
                finalSpeed = speed + Random.Range(-bulletSpeedVariance, bulletSpeedVariance);
            }
            // 实例化子弹
            GameObject bullet = Instantiate(enemy_objects, objectTransform.position, Quaternion.Euler(0, 0, finalAngle));
            BossObjects_SingleHitVfx bulletComp = bullet.GetComponentInChildren<BossObjects_SingleHitVfx>();
            if (bulletComp != null)
            {
                bulletComp.duration = duration;
                bulletComp.Setup(boss, false, finalAngle);
                bulletComp.Launch(finalDir * finalSpeed *boss.facingDir);
                Debug.DrawRay(objectTransform.position, finalDir * 1.0f, Color.red, 1.0f);
                Debug.Log($"[Create_Bullets] spawned bullet #{i} angle={finalAngle:F1} deg, dir={finalDir}, speed={finalSpeed:F2}");
            }
  
        }

    }

    public void Create_MultipleBulletsContinously()
    {
        StartCoroutine(ShootBulletsContinuously(fireInterval, bulletCount, bulletSpeed , spreadAngle));
    }

    public IEnumerator ShootBulletsContinuously(float fireInterval, int bulletCount, float speed, float angleOffset = 0f)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // 实例化子弹
            GameObject bullet = Instantiate(enemy_objects, objectTransform.position, Quaternion.identity);

            // 获取子弹脚本
            BossObjects_SingleHitVfx bulletObj = bullet.GetComponentInChildren<BossObjects_SingleHitVfx>();
            bulletObj.duration = duration;
            float  theOffset = Random.Range(0, angleOffset);

            // 计算发射方向（基础方向为 boss 朝向）
            float baseAngle = boss.facingDir == 1 ? 0f : 180f;
            Debug.Log(baseAngle +" " + theOffset);
            float fireAngle = baseAngle + theOffset * boss.facingDir; 
            



            // 转为单位方向向量
            Vector2 fireDir = new Vector2(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad));
            if(boss.transform.position.y > 0f)
            {
                fireAngle = - fireAngle;
            }
            bulletObj.Setup(boss, false , fireAngle);
            // 设置速度
            bulletObj.Launch(fireDir * speed);

         

            // 等待下一颗子弹
            yield return new WaitForSeconds(fireInterval);
        }
    }

    public void AddFuryCounter()
    {
        if (boss != null)
        {
            boss.AddFuryCounter();
            Debug.Log("[Boss_ObjectAnimationTriggers] AddFuryCounter called on boss");
        }
        else
        {
            Debug.LogWarning("[AddFuryCounter] boss reference is null");
        }
    }

    public void SpellCall()
    {
        uiboss.SpellCardSlideIn();

    }

    public void ShowSpellCardCircle()
    {
        spellCardCircle.GetComponent<SpriteRenderer>().DOFade(1f,1f);
        spellCardInnerCircle.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
    }
}
