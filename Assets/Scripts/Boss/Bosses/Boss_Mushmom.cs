using UnityEngine;

public class Boss_Mushmom : Boss
{
    
    protected override void Awake()
    {
        base.Awake();
        idleState = new Boss_IdleState(this, stateMachine, "idle");
        moveState = new Boss_MoveState(this, stateMachine, "move");
        battleState = new Boss_BattleState(this, stateMachine, "battle");
        attackState = new Boss_AttackState(this, stateMachine, "attack");
        deadState = new Boss_DeadState(this, stateMachine, "die");
        onHitState = new Boss_OnHitState(this, stateMachine, "onHit");
        skill_1_State = new Boss_1_SkillState(this, stateMachine, "skill1");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        
    }

    protected override void Update()
    {
        base.Update();


    }


    
}
