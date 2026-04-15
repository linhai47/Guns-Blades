
using UnityEngine;

public class Boss_Reimu : Boss
{

    protected override void Awake()
    {
        base.Awake();

        reimu_IdleState = new Reimu_IdleState(this, stateMachine, "idle");
        reimu_MoveState = new Reimu_MoveState(this, stateMachine, "move");
     
        reimu_AttackState = new Reimu_AttackState(this, stateMachine, "attack");
        reimu_KnockOutState = new Reimu_KnockOutState(this, stateMachine, "knockout");
        reimu_OnHitState = new Reimu_OnHitState(this, stateMachine, "onHit");
        reimu_ShowUpState = new Reimu_ShowUpState(this, stateMachine, "showUp");
        reimu_Phase1_FightingState = new Reimu_Phase1_FightingState(this, stateMachine, "fighting");
        reimu_airKickAttackState = new Reimu_AirKickAttackState(this, stateMachine, "airkickAttack");
        reimu_underAttackState = new Reimu_UnderAttackState(this, stateMachine, "underAttack");
        reimu_slideAttackState = new Reimu_SlideAttackState(this, stateMachine, "slideAttack");
        reimu_airShotState = new Reimu_AirShotState(this, stateMachine, "airShot");
        reimu_ShotState = new Reimu_ShotState(this, stateMachine, "shot");
        reimu_DashState = new Reimu_DashState(this, stateMachine, "dash");
        reimu_JumpState = new Reimu_JumpState(this, stateMachine, "jump");
        reimu_AirState = new Reimu_AirState(this, stateMachine, "air");
        reimu_RecoverState2 = new Reimu_RecoverState2(this, stateMachine, "recover2");
        reimu_Phase2_FightingState = new Reimu_Phase2_FightingState(this, stateMachine, "fighting2");
        reimu_ReadingSpellCardState = new Reimu_ReadingSpellCardState(this, stateMachine, "reading");
        reimu_EnterFantasySealState = new Reimu_EnterFantasySealState(this, stateMachine, "enter1");
        reimu_FantasySealState = new Reimu_FantasySealState(this, stateMachine, "√ŒœÎ∑‚”°");

        middleAttackState = new BossState[] {
            reimu_airKickAttackState,
            reimu_slideAttackState,
            reimu_ShotState,
            reimu_JumpState,

        };


        longAttackState = new BossState[] {
            reimu_AttackState,
            reimu_airKickAttackState,
            reimu_ShotState,
              reimu_JumpState,
        };

        shortAttackState = new BossState[] {
            reimu_underAttackState,
            reimu_slideAttackState,
            reimu_ShotState,
            reimu_DashState
        };
    }

    protected override void Start()
    {

        stateMachine.Initialize(reimu_ShowUpState);
        Flip();

    }

    protected override void Update()
    {
        base.Update();


    }
}
