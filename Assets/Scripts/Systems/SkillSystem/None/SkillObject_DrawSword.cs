using UnityEngine;

public class SkillObject_DrawSword :SkillObject_Base
{
    protected override void Awake()
    {
        base.Awake();
        Destroy(gameObject,.15f);
    }

    public override void Setup(SkillBase skill, bool isFlying = true)
    {
        base.Setup(skill);

        SetDirection();
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {


        // 如果不是敌人层，直接返回
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;

        // 伤害结算（父类逻辑）
        ApplyDamageToTarget(collision , 5);

        //// 命中特效
        //if (onHitVfx != null)
        //{
        //    Instantiate(onHitVfx, collision.transform.position, Quaternion.identity);
        //}


    }
}
