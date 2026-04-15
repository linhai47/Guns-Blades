using UnityEngine;

public class Reimu_IdleState :BossState
{
    public Reimu_IdleState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boss.idleTime;
    }


    public override void Update()
    {
        base.Update();
       

        if(stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_MoveState);
        }
    }
}
