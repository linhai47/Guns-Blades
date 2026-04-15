using System.Collections.Generic;
using UnityEngine;

public class SkillObject_WindPulls : SkillObject_Base
{

    [SerializeField] private float damageInterval = 0.5f; 
    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    public override void Setup(SkillBase skill, bool isFlying = true)
    {
        base.Setup(skill, isFlying);
        Destroy(gameObject,1f);
    }


  
    protected void OnTriggerStay2D(Collider2D collision)
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
