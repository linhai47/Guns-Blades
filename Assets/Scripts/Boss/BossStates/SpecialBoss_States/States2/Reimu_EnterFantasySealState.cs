using UnityEngine;

public class Reimu_EnterFantasySealState :BossState
{
    public Reimu_EnterFantasySealState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }
    public float duration = 1.53f;
    public override void Enter()
    {
        base.Enter();
        stateTimer = duration;

    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_FantasySealState);
        }

    }
}
