using UnityEngine;

public class Boss_DeadState : BossState
{
    public Boss_DeadState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + 5, 15);

        boss.GetComponent<Collider2D>().enabled = false;
        stateMachine.SwitchOffStateMachine();

    }
}
