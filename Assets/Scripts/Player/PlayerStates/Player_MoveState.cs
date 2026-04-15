using PixelCrushers.DialogueSystem.UnityGUI;
using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();


    }

    public override void Update()
    {
        base.Update();
        //if (PlayerDirector.nowState == playerDirectorState.move) return;


        if (player.moveInput.x == 0 || player.wallDetected ) stateMachine.ChangeState(player.idleState);


        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);

    }


}
