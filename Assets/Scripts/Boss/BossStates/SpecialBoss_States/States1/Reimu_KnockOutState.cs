using System.Collections;
using UnityEngine;

public class Reimu_KnockOutState : BossState
{
  
    public Reimu_KnockOutState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName) { }
    public override void Enter()
    {
        base.Enter();
        stateTimer = 2f;

      

        boss.anim.CrossFade(animBoolName, 0,0, 0f);


        boss.anim.Update(0f);
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
        {
            stateMachine.ChangeState(boss.reimu_RecoverState2);
        }


    }

}
