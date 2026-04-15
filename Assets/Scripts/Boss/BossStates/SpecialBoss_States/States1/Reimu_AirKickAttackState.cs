using UnityEngine;

public class Reimu_AirKickAttackState : Reimu_AttackState
{
    public Reimu_AirKickAttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 5f;
        weight = 5;
    }
    public float attackYSpeed = 8f;
   

    public override void Enter()
    {
        attackXSpeed = 20f;
        attackDuration = 1f;
        base.Enter();
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / attackDuration);

        float curveValueX = boss.airkickCurveX.velocityCurve.Evaluate(t);
        float curveValueY = boss.airkickCurveY.velocityCurve.Evaluate(t);
        float currentSpeedX = curveValueX * attackXSpeed * boss.facingDir;
        float currentSpeedY = curveValueY * attackYSpeed;
        boss.SetVelocity(currentSpeedX,currentSpeedY); // 只用这一行

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }
    }
}
