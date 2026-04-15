using UnityEngine;
public class Player_JumpAttackState : PlayerState
{
    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
        if (player.moveInput.x != 0)
        {
            player.SetVelocity(player.moveSpeed * player.moveInput.x * player.inAirMoveMultiplier, rb.linearVelocity.y);

        }

        if(stateInfo.normalizedTime > 1f)
        {
            if (rb.linearVelocity.y > 0f) player.SetVelocity(rb.linearVelocity.x, 0);


            stateMachine.ChangeState(player.fallState);

        }

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
       
       
    }


}
