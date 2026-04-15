using UnityEngine;

public class Boss_GroundedState : BossState
{
    public Boss_GroundedState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (boss.PlayerDetected())
        {
            stateMachine.ChangeState(boss.battleState);
        }



    }
}
