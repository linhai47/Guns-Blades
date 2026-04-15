using UnityEngine;

public class Reimu_MoveState :BossState
{
    public Reimu_MoveState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        if (boss.wallDetected || boss.groundDetected == false)
        {

            boss.Flip();
        }
    }

    public override void Update()
    {
        base.Update();

        boss.SetVelocity(boss.GetMoveSpeed() * boss.facingDir, rb.linearVelocity.y);


        if (boss.wallDetected || boss.groundDetected == false)
        {

            stateMachine.ChangeState(boss.reimu_IdleState);
        }
    }

}
