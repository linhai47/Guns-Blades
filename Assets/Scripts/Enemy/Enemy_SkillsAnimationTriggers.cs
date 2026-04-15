using UnityEngine;

public class Enemy_SkillsAnimationTriggers :Entity_AnimationTriggers
{
    public Enemy enemy;

    [Header("Enemy Skills")]
    public BossSkill_TitanFall enemySkill_TitanFall;

    protected override void Awake()
    {
        base.Awake();

        enemySkill_TitanFall = GetComponentInParent<BossSkill_TitanFall>();
        enemy = GetComponentInParent<Enemy>();

    }

 
    //private void TitanFallTrigger()
    //{
    //    enemySkill_TitanFall.TryUseSkill();

    //}
}
