using UnityEngine;

public class Enemy_JumpState : EnemyState
{
    public Enemy_JumpState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(enemy.rb.linearVelocity.x, enemy.jumpForce);
    }

}
