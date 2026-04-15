using UnityEngine;

public class Boss_SkillAnimationTriggers :Entity_AnimationTriggers
{

    public Boss boss;
    [Header("Enemy Skills")]
    public BossSkill_TitanFall enemySkill_TitanFall;

    protected override void Awake()
    {
        base.Awake();

        enemySkill_TitanFall = GetComponentInParent<BossSkill_TitanFall>();
     
        boss = GetComponentInParent<Boss>();
    }

    public void AddFuryCounter()
    {
        boss.AddFuryCounter();
    }
    //private void TitanFallTrigger()
    //{
    //    enemySkill_TitanFall.TryUseSkill();

    //}
}
