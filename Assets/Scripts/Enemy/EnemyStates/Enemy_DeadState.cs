using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x+5, 15);

        enemy.GetComponent<Collider2D>().enabled = false;
        stateMachine.SwitchOffStateMachine();

    }
}
