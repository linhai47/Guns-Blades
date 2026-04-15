using UnityEngine;

public class Boss_OnHitState : BossState
{
    public Boss_OnHitState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boss.onHitDuration;

    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.battleState);
        }

    }

}
