using UnityEngine;

public class SkillObject_Windblade :SkillObject_Base
{






    //protected override void start()
    //{
    //    base.start();
    //}

    protected override void Awake()
    {
        base.Awake();
    
    }

 

    private void Update()
    {
        if(Vector2.Distance(startPosition,transform.position)> flyingDistance)
        {
            Destroy(gameObject);
        }
    }

    public override void Setup(SkillBase skill , bool isFlying = true)
    {
        base.Setup(skill);

        SetDirection();
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
      

        // 如果不是敌人层，直接返回
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;

        // 伤害结算（父类逻辑）
        ApplyDamageToTarget(collision);

        // 命中特效
        if (onHitVfx != null)
        {
            Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
        }

        // 击中后消失（要穿透就注释掉）
        Destroy(gameObject);
    }
}
