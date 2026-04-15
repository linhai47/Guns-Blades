using UnityEngine;

public class Boss_MoveState :Boss_GroundedState
{
    public Boss_MoveState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
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

            stateMachine.ChangeState(boss.idleState);
        }   
    }
}
