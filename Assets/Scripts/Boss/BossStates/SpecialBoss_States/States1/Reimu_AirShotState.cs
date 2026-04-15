using UnityEngine;

public class Reimu_AirShotState : Reimu_AttackState
{
    public Reimu_AirShotState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 5f;
        weight = 10;
    }



    public override void Enter()
    {
        attackDuration = 1.3f;
        attackYSpeed = 20f;
     
        base.Enter();
    }

    public override void Update()
    {


        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        //float t = Mathf.Clamp01(elapsedTime / attackDuration);

        //float curveValue = boss.airShotCurve.velocityCurve.Evaluate(t);
        //float currentSpeed = curveValue * attackYSpeed ;
        
        boss.SetVelocity(0, 0); // 只用这一行
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }
    }

}
