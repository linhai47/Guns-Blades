using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BossState : EntityState
{
    public Transform player;
    public Boss boss;
    public float cooldown;
    public float lastTime;
    public int weight;
    public BossState(Boss boss, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.boss = boss;
        anim = boss.anim;
        rb = boss.rb;
        stats = boss.stats;
    }

    public override void Enter()
    {
        base.Enter();

        lastTime = Time.time;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        float battleAnimSpeedMultiplier = boss.battleMoveSpeed / boss.moveSpeed;

        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("moveAnimSpeedMultiplier", boss.moveAnimSpeedMultiplier);
    }

    public float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;

        return Mathf.Abs(player.position.x - boss.transform.position.x);
    }

    public int DirectionToPlayer()
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
