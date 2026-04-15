using UnityEngine;

public class Reimu_JumpState : BossState
{
    public Reimu_JumpState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = 5f;
        weight = 10;
    }

    public float jumpTime = .5f;
    public float jumpDuration = .5f;
    public float ySpeed = 25f;
    public override void Enter()
    {
        base.Enter();
        jumpTime = 0f;
        stateTimer = jumpDuration;
    }


    public override void Update()
    {
        base.Update();
        jumpTime += Time.deltaTime;

        float t= Mathf.Clamp01(jumpTime/jumpDuration);

        float curveValue = boss.jumpCurve.velocityCurve.Evaluate(t);
        float currentSpeed = curveValue*ySpeed;


        boss.SetVelocity(0, currentSpeed);
        if(stateTimer < 0){
            stateMachine.ChangeState(boss.reimu_airShotState);
        }

    }



}
