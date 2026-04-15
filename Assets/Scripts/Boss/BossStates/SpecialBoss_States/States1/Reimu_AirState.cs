using UnityEngine;

public class Reimu_AirState : BossState
{
    public Reimu_AirState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if(boss.groundDetected)
        {
            if(boss.nowPhase == 1)
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);


            if (boss.nowPhase == 2)
            {
                stateMachine.ChangeState(boss.reimu_EnterFantasySealState);
                boss.canBeOnHit = false;
            }
        }

    }

}
