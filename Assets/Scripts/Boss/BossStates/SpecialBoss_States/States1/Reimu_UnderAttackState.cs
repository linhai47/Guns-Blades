using UnityEngine;

public class Reimu_UnderAttackState :Reimu_AttackState
{
    public Reimu_UnderAttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 5f;

        weight = 10;
    }

    public override void Enter()
    {

        attackDuration =.9f;
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
