using UnityEngine;

public class Boss_BattleState : BossState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    public Boss_BattleState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UpdateBattleTimer();
        if (player == null) player = boss.GetPlayerReference();

        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(boss.retreatVelocity.x * boss.activeSlowMultiplier * -DirectionToPlayer(), boss.retreatVelocity.y);

            boss.HandleFlip(DirectionToPlayer());


        }
    }


    public override void Update()
    {
        base.Update();
        if (boss.PlayerDetected() == true)
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }
        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(boss.idleState);
        }

        if (WithinAttackRange() && boss.PlayerDetected())
        {
            stateMachine.ChangeState(boss.attackState);

        }
        else
        {
            boss.SetVelocity(boss.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);

        }

    }

    private void UpdateTargetIfNeeded()
    {
        if (boss.PlayerDetected() == false) return;

        Transform newTarget = boss.PlayerDetected().transform;
        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    private void UpdateBattleTimer()
    {
        lastTimeWasInBattle = Time.time;
    }

    private bool BattleTimeIsOver() => Time.time > lastTimeWasInBattle + boss.battleTimeDuration;


    public bool WithinAttackRange() => DistanceToPlayer() < boss.attackDistance;

    public bool ShouldRetreat() => DistanceToPlayer() < boss.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;

        return Mathf.Abs(player.position.x - boss.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null) { return 0; }

        return player.position.x > boss.transform.position.x ? 1 : -1;

    }
}
