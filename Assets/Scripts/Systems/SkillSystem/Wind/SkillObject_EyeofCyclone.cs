using System.Collections.Generic;
using UnityEngine;

public class SkillObject_EyeofCyclone : SkillObject_Base
{
    [SerializeField] private float damageInterval = 0.5f; // 每0.5秒造成一次伤害
    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    protected override void Awake()
    {
        base.Awake();
        Destroy(gameObject,1.25f);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        // 如果不是敌人层，直接返回
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;

        // 伤害结算（父类逻辑）
        ApplyDamageToTarget(collision);

        // 命中特效
        if (onHitVfx != null)
        {
            Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
        }
    }

    protected  void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;

        if (!lastHitTime.ContainsKey(collision.gameObject) ||
            Time.time - lastHitTime[collision.gameObject] >= damageInterval)
        {
            ApplyDamageToTarget(collision);
            lastHitTime[collision.gameObject] = Time.time;

            if (onHitVfx != null)
            {
                Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
            }
        }
    }
}
