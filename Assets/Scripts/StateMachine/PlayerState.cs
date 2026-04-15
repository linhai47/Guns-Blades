using UnityEngine;

public class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    public PlayerDirector PlayerDirector;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        stats = player.stats;
        input = player.input;
        PlayerDirector = PlayerDirector.instance;
    }

     public override void Update()
    {
        base.Update();

        if(CanDash() && input.Player.Dash.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.dashState);
        }



    }

    private bool CanDash()
    {
        if (player.wallDetected == true) return false;
        if(stateMachine.currentState == player.jumpAttackState) return false;
        if (stateMachine.currentState == player.dashState) return false;
        if (stateMachine.currentState == player.castingState) return false;

        return true;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }



}
