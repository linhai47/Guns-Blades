using UnityEngine;

public class Reimu_FantasySealState : Reimu_AttackState
{
    public Reimu_FantasySealState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        attackDuration = 10f;
    }


    public override void Enter()
    {
        base.Enter();
        elapsedTime = 0f;
        attackXSpeed = 4f;
        attackYSpeed = 3f;
        stateTimer = attackDuration;
        LockOnPlayer();
    }

    public override void Update()
    {
        if (stateTimer < 0)
        {
            //LockOnPlayer();
            boss.Flip(); 
            //stateMachine.ChangeState(boss.reimu_Phase2_FightingState);
            stateMachine.ChangeState(boss.reimu_EnterFantasySealState);
        }
      
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / attackDuration);

        float curveValueX = boss.FantasySealCurveX.velocityCurve.Evaluate(t);
        float curveValueY = boss.FantasySealCurveY.velocityCurve.Evaluate(t);
        float XSpeed = curveValueX * attackXSpeed * boss.facingDir ;
        float YSpeed = curveValueY * attackYSpeed;


        boss.SetVelocity(XSpeed, YSpeed); // 只用这一行




    

    }


}   
