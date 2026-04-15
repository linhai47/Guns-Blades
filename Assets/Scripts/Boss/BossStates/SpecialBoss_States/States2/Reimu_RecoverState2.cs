using UnityEngine;

public class Reimu_RecoverState2 : BossState
{
    public Reimu_RecoverState2(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {

    }

    public float recoverTime = .533f;
    public override void Enter()
    {
        base.Enter();
        stateTimer = recoverTime;
        boss.health.currentHealth = boss.stats.GetMaxHealth();
    }

    public override void Update()
    {
        base.Update();


        if(stateTimer < 0)
        {
           stateMachine.ChangeState(boss.reimu_ReadingSpellCardState);
        }
    }
}
