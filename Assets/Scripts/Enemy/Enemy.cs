using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Quest Info")]
    public NPCname questTargetId;

    public Enemy_Health health { get; private set; }

 
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


    [Range(0f, 2f)]
    public float moveAnimSpeedMultiplier = 1;
    [Header("Player detection")]
    public float playerCheckDistance = 10;
    [SerializeField] public LayerMask whatIsPlayer;
    [SerializeField] public Transform playerCheck;

 

    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1;

    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_OnHitState onHitState;
    public Enemy_1_SkillState skill_1_State;

    [Header("ÊÜÉËÖ¡")]
    public bool canBeOnHit = false;
    public float onHitDuration = .2f;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;
    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    protected override void Awake()
    {

       
        base.Awake();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();

    }
    protected override void Start()
    {
        base.Start();
        
    }


    protected override void Update()
    {
        base.Update();


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

        this.player = player;
        if(canBeOnHit == false)stateMachine.ChangeState(battleState);
        else stateMachine.ChangeState(onHitState);
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatisGround);
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
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

    }

   
}
