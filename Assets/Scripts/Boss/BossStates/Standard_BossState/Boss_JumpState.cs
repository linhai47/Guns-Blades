using UnityEngine;

public class Boss_JumpState :BossState
{
    public Boss_JumpState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boss.SetVelocity(boss.rb.linearVelocity.x, boss.jumpForce);
    }

}
