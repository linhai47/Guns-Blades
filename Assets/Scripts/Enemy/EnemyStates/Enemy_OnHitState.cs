using UnityEngine;

public class Enemy_OnHitState : EnemyState
{

    public Enemy_OnHitState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.onHitDuration;
       
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

    }
   

  

}
