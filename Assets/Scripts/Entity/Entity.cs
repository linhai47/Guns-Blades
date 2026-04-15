using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;
    public Entity_VFX vfx { get; private set; }

    public Entity_Combat entity_Combat { get; private set; }

    public SpriteRenderer sr;
    public Rigidbody2D rb { get; private set; }


    public Animator anim;

    private bool facingRight = true;
    public int facingDir = 1;
    private bool isKnocked;
    private Coroutine knockbackCo;
    protected Coroutine SpeedUpCo;
    public StateMachine stateMachine;
    [Header("Collision detection")]
    public LayerMask whatisGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    public static event Action<Entity> onEnemySpawnedStatic;
    public event Action<Entity> onEnemyDied;

    public bool useTopAnimator = false;

    public Entity_Stats stats;
    protected virtual void Start()
    {
        onEnemySpawnedStatic?.Invoke(this);

    }

    protected virtual void Awake()
    {



        anim = GetComponentInChildren<Animator>();
        if (anim != null && anim.gameObject == gameObject)
        {
            // 找到的是自己，说明本体也有 Animator
            // 那就再手动找子物体
            Animator[] all = GetComponentsInChildren<Animator>(true);
            foreach (var a in all)
            {
                if (a.gameObject != gameObject)
                {
                    anim = a;
                    break;
                }
            }
        }
        stats = GetComponent<Entity_Stats>();

        rb = GetComponent<Rigidbody2D>();
        vfx = GetComponent<Entity_VFX>();
        sr = GetComponentInChildren<SpriteRenderer>();
        entity_Combat = GetComponentInChildren<Entity_Combat>();
        stateMachine = new StateMachine();
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();


    }

    public void SpeedUpEntity(float accMultiplier, float duration, bool canBeOverride = false)
    {
        if (SpeedUpCo != null)
        {
            if (canBeOverride)
                StopCoroutine(SpeedUpCo);
            else
                return;
        }

        SpeedUpCo = StartCoroutine(SpeedUpEntityCo(duration, accMultiplier));


    }
    protected virtual IEnumerator SpeedUpEntityCo(float duration, float accMultiplier)
    {

        yield return null;
    }
    public void CurrentStatteAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();

    }
    public virtual void EntityDeath()
    {
        onEnemyDied?.Invoke(this);
    }

    public virtual void EntityKnockOut()
    {

    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    public void SetVelocityWithAcceleration(float targetX, float targetY, float acceleration)
    {
        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 targetVelocity = new Vector2(targetX, targetY);

        // 计算需要变化的速度向量
        Vector2 velocityChange = targetVelocity - currentVelocity;

        // 限制每帧最大速度变化量
        Vector2 clampedChange = Vector2.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = currentVelocity + clampedChange;

        HandleFlip(targetX);
    }
    public void HandleFlip(float xVelocity)
    {
        if (facingRight == true && xVelocity < 0)
        {
            Flip();
        }
        else if (facingRight == false && xVelocity > 0)
        {
            Flip();
        }

    }
    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null) StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }
    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    public void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);

        facingRight = !facingRight;

        OnFlipped?.Invoke();
    }

    public void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);

        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, facingDir * Vector2.right, wallCheckDistance, whatisGround)
                && Physics2D.Raycast(secondaryWallCheck.position, facingDir * Vector2.right, wallCheckDistance, whatisGround);


        }
        else wallDetected = Physics2D.Raycast(primaryWallCheck.position, facingDir * Vector2.right, wallCheckDistance, whatisGround);


    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null) Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));


    }
}
