using UnityEngine;

public class Reimu_ShowUpState : BossState
{
    public Reimu_ShowUpState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }
    public float animTime = 1.8f;
    public override void Enter()
    {
        base.Enter();
        stateTimer = animTime;
        boss.canBeOnHit = true;
        
    }


    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }
    }
}
