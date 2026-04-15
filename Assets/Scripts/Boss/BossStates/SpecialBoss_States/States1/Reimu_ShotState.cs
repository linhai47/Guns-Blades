using PixelCrushers.DialogueSystem.UnityGUI;
using UnityEngine;

public class Reimu_ShotState : Reimu_AttackState
{
    public Reimu_ShotState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown =5f;
        weight = 20;
       
    }



    public override void Enter()
    {
        attackDuration = 1.5f;
      
        base.Enter();
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }
    }


}
