using UnityEngine;

public class Enemy_Skeleton :Enemy
{

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this,stateMachine,"idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        

    }


}
