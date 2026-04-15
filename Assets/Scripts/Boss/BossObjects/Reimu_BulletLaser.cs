using UnityEngine;

public class Reimu_BulletLaser :BossObjects
{
  
    protected override void Awake()
    {
        base.Awake();

     
    }
   public void SetDirectionAndVelocity(Vector3 Dir , float angle)
    {
        Setup(boss,true,angle,Dir);


    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
      
        // 如果不是敌人层，直接返回
        if (((1 << collision.gameObject.layer) & whatIsPlayer) == 0) return;

        // 伤害结算（父类逻辑）
        ApplyDamageToTarget(collision);


    }
}
