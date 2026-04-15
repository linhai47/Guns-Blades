using UnityEngine;

public class Boss_1_SkillState : BossState
{
    public Boss_1_SkillState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


    }
    public override void Update()
    {
        base.Update();
        if (boss.furyCounter == 0)
        {
            stateMachine.ChangeState(boss.battleState);
        }
    }
}
