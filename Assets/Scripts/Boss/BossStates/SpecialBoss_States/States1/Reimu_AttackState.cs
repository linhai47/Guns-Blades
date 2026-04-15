using Unity.VisualScripting;
using UnityEngine;

public class Reimu_AttackState :BossState
{
    public Reimu_AttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 3f;
        weight = 10;
    }
    public float elapsedTime;
    public float attackXSpeed = 8f;
    public float attackYSpeed = 10f;
    public float acc = 10;
    public float attackDuration = 2.3f;
    public override void Enter()
    {
        base.Enter();
  
        elapsedTime = 0f;

        stateTimer = attackDuration;

    }


    public override void Update()
    {

        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / attackDuration);

        float curveValue = boss.moveCurve.velocityCurve.Evaluate(t);
        float currentSpeed = curveValue * attackXSpeed * boss.facingDir;

        boss.SetVelocity(currentSpeed, boss.rb.linearVelocity.y); // 只用这一行

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }


    }
}
