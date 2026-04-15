using UnityEngine;

public class Boss_AttackState : BossState
{
    public Boss_AttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


        SyncAttackSpeed();
    }
    public override void Update()
    {
        base.Update();
        if (boss.furyCounter >= 3)
        {
            if (boss.skill_1_State != null)
            {
                stateMachine.ChangeState(boss.skill_1_State);
                boss.furyCounter = -1;
            }
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(boss.battleState);
        }
    }
}
