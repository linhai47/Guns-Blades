using UnityEngine;

public class Reimu_SlideAttackState : Reimu_AttackState
{
    public Reimu_SlideAttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 5f;
        weight = 10;
    }


    public override void Enter()
    {
        attackXSpeed =20f;
        attackDuration = .4f;
        base.Enter();
    }


    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / attackDuration);

        float curveValue = boss.slideCurve.velocityCurve.Evaluate(t);
        float currentSpeed = curveValue * attackXSpeed * boss.facingDir;

        boss.SetVelocity(currentSpeed, boss.rb.linearVelocity.y); // 只用这一行

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }

    }
}
