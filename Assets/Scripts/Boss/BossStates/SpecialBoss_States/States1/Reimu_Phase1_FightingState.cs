using UnityEngine;

public class Reimu_Phase1_FightingState : BossState
{
    public Reimu_Phase1_FightingState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
        cooldown = .5f;
        
    }


    private Transform lastTarget;
    private float lastTimeWasInBattle;


    public override void Enter()
    {
        base.Enter();
        lastTime = Time.time;
        UpdateBattleTimer();
        if (player == null) player = boss.GetPlayerReference();


    }


    public override void Update()
    {
        base.Update();



        if (boss.GetPlayerInRadius(boss.searchRadius))
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }
        
        if (Input.GetKeyUp(KeyCode.F))
        {
            stateMachine.ChangeState(boss.reimu_airKickAttackState);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            stateMachine.ChangeState(boss.reimu_slideAttackState);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            stateMachine.ChangeState(boss.reimu_underAttackState);
        }


        LockOnPlayer();
        if (!boss.groundDetected)
        {
            stateMachine.ChangeState(boss.reimu_AirState);
        }

        if(Time.time - lastTime < cooldown && DistanceToPlayer() > 3f)
        {
            boss.SetVelocity(boss.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }
        else
        {
            boss.SetVelocity(0,rb.linearVelocity.y);
        }
        if (WithinShortAttackRange() && boss.PlayerDetected())
        {
            GetCoolDownShortState();
        }
        else if (WithinMiddleAttackRange() && boss.PlayerDetected())
        {
            GetCoolDownMiddleState();
        }
        else if (WithinLongAttackRange() && boss.PlayerDetected())
        {
            GetCoolDownLongState();

        }
        else
        {
            boss.SetVelocity(boss.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }

    }
    private void GetCoolDownShortState()
    {

        int totalWeight = 0;
        foreach (var state in boss.shortAttackState)
        {
            totalWeight += state.weight;
        }
        Debug.Log(totalWeight);
        int t = Random.Range(0, totalWeight);
        int rate = totalWeight - t;// 比如 权重为 5， 总权重 为 15 ， roll出来 8 ， 那么有三分之一的概率进入State应该 ， 小于等于5才可以进入 
        foreach (var state in boss.shortAttackState)
        {
            if (Time.time - state.lastTime >= state.cooldown && rate >= state.weight)
            {
                stateMachine.ChangeState(state);
            }


        }
    }
    private void GetCoolDownMiddleState()
    {
        int totalWeight = 0;
        foreach (var state in boss.middleAttackState)
        {
            totalWeight += state.weight;
        }
        Debug.Log(totalWeight);
        int t = Random.Range(0, totalWeight);
        int rate = totalWeight - t;// 比如 权重为 5， 总权重 为 15 ， roll出来 8 ， 那么有三分之一的概率进入State应该 ， 小于等于5才可以进入 
        foreach (var state in boss.middleAttackState)
        {
            if (Time.time - state.lastTime >= state.cooldown && rate >= state.weight)
            {
                stateMachine.ChangeState(state);
            }


        }
    }
    private void GetCoolDownLongState()
    {
        int totalWeight = 0;
        foreach (var state in boss.longAttackState)
        {
            totalWeight += state.weight;
        }
        Debug.Log(totalWeight);
        int t = Random.Range(0, totalWeight);
        int rate = totalWeight - t;// 比如 权重为 5， 总权重 为 15 ， roll出来 8 ， 那么有三分之一的概率进入State应该 ， 小于等于5才可以进入 
        foreach (var state in boss.longAttackState)
        {
            if (Time.time - state.lastTime >= state.cooldown && rate >= state.weight)
            {
                stateMachine.ChangeState(state);
            }


        }
    }



    private void UpdateTargetIfNeeded()
    {
        if (boss.GetPlayerInRadius(boss.searchRadius)) return;

        Transform newTarget = boss.GetPlayerInRadius(boss.searchRadius).transform;
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


    public bool WithinLongAttackRange() => DistanceToPlayer() < boss.longAttackDistance;
    public bool WithinMiddleAttackRange() => DistanceToPlayer() < boss.middleAttackDistance;

    public bool WithinShortAttackRange() => DistanceToPlayer() < boss.shortAttackDistance;


  
}
