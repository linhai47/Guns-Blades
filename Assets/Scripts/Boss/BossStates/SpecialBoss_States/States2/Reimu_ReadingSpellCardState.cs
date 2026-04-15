using UnityEngine;

public class Reimu_ReadingSpellCardState :BossState
{
    public Reimu_ReadingSpellCardState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public float duration =1.66f;
    public override void Enter()
    {
        base.Enter();
        stateTimer = duration;
        boss.nowPhase = 2;
    }


    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {

            stateMachine.ChangeState(boss.reimu_EnterFantasySealState);
        }
    }
}
