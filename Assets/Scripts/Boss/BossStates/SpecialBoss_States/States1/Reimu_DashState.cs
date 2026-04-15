using UnityEngine;

public class Reimu_DashState : Reimu_AttackState
{
    public Reimu_DashState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 4f;
        weight = 10;
    }

    private int nowDir = 1;


    public override void Enter()
    {
        attackXSpeed = 20f;
        attackDuration = .4f;
        nowDir = boss.facingDir * -1;
     
        base.Enter();
    }


    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / attackDuration);

        float curveValue = boss.dashCurve.velocityCurve.Evaluate(t);
   
        float currentSpeed = curveValue * attackXSpeed * nowDir;

        boss.SetVelocity(currentSpeed, 0); // 只用这一行

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_Phase1_FightingState);
        }

    }
}
