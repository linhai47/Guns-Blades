using UnityEngine;

public class Reimu_Phase2_FightingState : BossState
{
    public Reimu_Phase2_FightingState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }



    private Transform lastTarget;
    private float lastTimeWasInBattle;

    public float SkillColdDown = 1f;
    public override void Enter()
    {
        base.Enter();
        lastTime = Time.time;
        UpdateBattleTimer();
        if (player == null) player = boss.GetPlayerReference();

        stateTimer = SkillColdDown;
    }


    public override void Update()
    {
        base.Update();

        if(stateTimer < 0f)
        {
            stateMachine.ChangeState(boss.reimu_EnterFantasySealState);
        }

        if (boss.GetPlayerInRadius(boss.searchRadius))
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }

      


        LockOnPlayer();
        if (!boss.groundDetected)
        {
            stateMachine.ChangeState(boss.reimu_AirState);
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

    public void LockOnPlayer()
    {
        if (player == null) player = boss.GetPlayerReference();

        if (DirectionToPlayer() == 1 && boss.facingDir == -1)
        {
            boss.Flip();
        }
        else if (DirectionToPlayer() == -1 && boss.facingDir == 1)
        {
            boss.Flip();
        }

    }

}
