using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossObjects_SingleHitVfx :BossObjects
{
    public float duration = .5f;

    protected override void Awake()
    {
        base.Awake();
 
        Destroy(gameObject, duration);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Player Enter Into");
        // 如果不是敌人层，直接返回
        if (((1 << collision.gameObject.layer) & whatIsPlayer) == 0) return;

        // 伤害结算（父类逻辑）

        ApplyDamageToTarget(collision);
        // 命中特效
        if (onHitVfx != null)
        {
            Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
        }



    }
    public void Launch(Vector2 velocity)
    {

            rb.linearVelocity = velocity;
        
  
       
    }

  

    void SelfDestruct()
    {
        Debug.Log("[BossObjects_SingleHitVfx] SelfDestruct called");
        Destroy(gameObject);
    }
}
