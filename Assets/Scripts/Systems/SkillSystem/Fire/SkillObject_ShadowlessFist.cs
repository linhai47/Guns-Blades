using UnityEngine;

public class SkillObject_ShadowlessFist : SkillObject_Base
{

    protected override void Awake()
    {
        base.Awake();
        Destroy(gameObject , 1f);
    }

    public override void Setup(SkillBase skill, bool isFlying = true)
    {
        base.Setup(skill, isFlying);

    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsEnemy) == 0) return;

        base.OnTriggerEnter2D(collision);


    }
}
