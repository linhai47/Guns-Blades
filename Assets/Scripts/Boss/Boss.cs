using UnityEngine;

public class Boss : Entity
{

    [Header("Quest Info")]
    public NPCname questTargetId;

    public Boss_Health health;


    [Header("Battle Details")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2;
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("stunned state details")]
    public float stunnedDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7, 7);
    [SerializeField] protected bool canBeStunned;

    [Header("Movement details")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;
    public float jumpForce = 12f;
    [Header("寻敌")]
    public float searchRadius= 5f;


    public Boss_IdleState idleState;
    public Boss_MoveState moveState;
    public Boss_AttackState attackState;
    public Boss_BattleState battleState;
    public Boss_DeadState deadState;
    public Boss_OnHitState onHitState;
    public Boss_1_SkillState skill_1_State;

    [Header("阶段1")]
    public Reimu_IdleState reimu_IdleState;
    public Reimu_MoveState reimu_MoveState;
    public Reimu_AttackState reimu_AttackState;
    public Reimu_KnockOutState reimu_KnockOutState;
    public Reimu_OnHitState reimu_OnHitState;
    public Reimu_ShowUpState reimu_ShowUpState;
    public Reimu_Phase1_FightingState reimu_Phase1_FightingState;
    public Reimu_AirKickAttackState reimu_airKickAttackState;
    public Reimu_SlideAttackState reimu_slideAttackState;
    public Reimu_UnderAttackState reimu_underAttackState;
    public Reimu_AirShotState reimu_airShotState;
    public Reimu_ShotState reimu_ShotState;
    public Reimu_DashState reimu_DashState;
    public Reimu_JumpState reimu_JumpState;
    public Reimu_AirState reimu_AirState;
    [Header("阶段2")]
    public Reimu_RecoverState2 reimu_RecoverState2;
    public Reimu_Phase2_FightingState reimu_Phase2_FightingState;
    public Reimu_ReadingSpellCardState reimu_ReadingSpellCardState;
    public Reimu_EnterFantasySealState reimu_EnterFantasySealState;
    public Reimu_FantasySealState reimu_FantasySealState ;

    [Range(0f, 2f)]
    public float moveAnimSpeedMultiplier = 1;
    [Header("Player detection")]
    public float playerCheckDistance = 10;
    [SerializeField] public LayerMask whatIsPlayer;
    [SerializeField] public Transform playerCheck;

    [Header("新增 狂怒条")]

    public int furyCounter = 0;

    public Transform player;
    public float activeSlowMultiplier { get; private set; } = 1;



    [Header("受伤帧")]
    public bool canBeOnHit = false;
    public float onHitDuration = .2f;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;
    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    public bool isSpecialBoss_Reimu = false;

    [Header("动画曲线")]
    public MoveCurveSO moveCurve;
    public MoveCurveSO slideCurve;
    public MoveCurveSO airkickCurveX;
    public MoveCurveSO airkickCurveY;
    public MoveCurveSO airShotCurve;
    public MoveCurveSO dashCurve;
    public MoveCurveSO jumpCurve;
    public MoveCurveSO FantasySealCurveX;
    public MoveCurveSO FantasySealCurveY;


    [Header("远 中 近 各自距离")]
    public float longAttackDistance = 15f;
    public float middleAttackDistance = 8f;
    public float shortAttackDistance = 4f;

    [Header(" 远 中 近 对应状态 ")]
    public BossState[] longAttackState;
    public BossState[] middleAttackState;
    public BossState[] shortAttackState;

    public int nowPhase = 1;
   

    protected override void Awake()
    {


        base.Awake();
        health = GetComponent<Boss_Health>();
        stats = GetComponent<Entity_Stats>();
      
    }

    protected override void Update()
    {
        base.Update();


    }

    public override void EntityKnockOut()
    {
        base.EntityKnockOut();
        if (health.canBeKnockout)
        {
            Debug.Log("Enter 进入 KnockOut");
            stateMachine.ChangeState(reimu_KnockOutState);
        }


    }

    public override void EntityDeath()
    {
        base.EntityDeath();
       
        stateMachine.ChangeState(deadState);
    }
    public void TryEnterBattleState(Transform player)
    {
        //if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
        //{
        //    return;
        //}
        if(health.currentHealth <= 0)
        {
            return;
        }
        this.player = player;
        if(isSpecialBoss_Reimu == true)
        {
            Debug.Log("Reimu On hit");
            if(canBeOnHit == true)stateMachine.ChangeState(reimu_OnHitState);

        }
        else if (canBeOnHit == false) stateMachine.ChangeState(battleState);
        else stateMachine.ChangeState(onHitState);
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatisGround);
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }
    public Collider2D GetPlayerInRadius(float radius)
    {
        // 只检测玩家图层
        Collider2D col = Physics2D.OverlapCircle(transform.position, radius, whatIsPlayer);
        return col; // null 表示没找到
    }


    public Transform GetPlayerReference()
    {
        if (player == null) player = PlayerDetected().transform;

        return player;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));
        Gizmos.color = Color.cyan; 

        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    public void AddFuryCounter()
    {
        furyCounter++;
        Debug.Log("狂怒值上升 1" + "现在的狂怒值" + furyCounter);
    }
}
