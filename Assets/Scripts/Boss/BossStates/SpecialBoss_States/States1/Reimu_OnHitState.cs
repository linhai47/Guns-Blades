using UnityEngine;

public class Reimu_OnHitState :BossState

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Reimu_OnHitState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        stateTimer = boss.onHitDuration;

    }

    public override void Update()
    {
      base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }

    }


}
