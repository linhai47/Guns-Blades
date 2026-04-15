using System;
using System.Collections;
using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Player : Entity
{
    //public Player_VFX vfx { get; private set; }
    public static Player instance;
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }



    public Inventory_Player inventory { get; private set; }
    public PlayerInputSet input {  get; private set; }

    public static event Action OnPlayerDeath;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    public Player_JumpState jumpState { get; private set; }

    public Player_FallState fallState { get; private set; }

    public Player_WallJumpState wallJumpState { get; private set; }

    public Player_WallSlideState wallSlideState { get; private set; }

    public Player_DeadState deadState { get; private set; }

    public Player_DashState dashState { get; private set; }

    public Player_BasicAttackState basicAttackState { get; private set; }

    public Player_JumpAttackState jumpAttackState { get; private set; }

    public Player_CastingState castingState { get; private set; }

    public ProximitySelector selector { get; private set; }

    public Player_QuestManager questManager { get; private set; }

    public Player_Exp playerExp { get; private set; }
    public UI ui { get; private set; }
    [Header("Movement Details")]

    public float moveSpeed = 1.0f;
    public float jumpForce = 15f;
    public Vector2 wallJumpForce;
    public float dashDuration = .3f;
    public float dashSpeed = 25f;

    public Vector2 moveInput { get; private set; }
    public Vector2 mousePosition { get; private set; }
    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public Vector2 JumpAttackVelocity;

    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

   
    protected override void Awake()
    {
        base.Awake();
        instance = this;
        input = new PlayerInputSet();
        stats = GetComponent<Player_Stats>();
        //vfx = GetComponent<Player_VFX>();
        ui = FindAnyObjectByType<UI>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        health = GetComponent<Entity_Health>();
        combat = GetComponent<Player_Combat>();
        //ui.SetupControlsUI(input);
        inventory = GetComponent<Inventory_Player>();
        selector = GetComponent<ProximitySelector>();
        questManager = GetComponent<Player_QuestManager>();
        playerExp = GetComponent<Player_Exp>();


        idleState = new Player_IdleState(this,stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpfall");
        fallState = new Player_FallState(this, stateMachine, "jumpfall");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpfall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        dashState = new Player_DashState(this, stateMachine, "dash");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        castingState = new Player_CastingState(this, stateMachine, "casting");
    }



    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    
    protected override IEnumerator SpeedUpEntityCo(float duration, float accMultiplier)
    {
        Debug.Log("Speed up!" + accMultiplier);
        
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttack = JumpAttackVelocity;
        //Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 + accMultiplier;

        moveSpeed = speedMultiplier * moveSpeed;
        jumpForce = speedMultiplier * jumpForce;
        anim.speed = speedMultiplier * anim.speed;
        wallJumpForce = speedMultiplier * wallJumpForce;
        JumpAttackVelocity = speedMultiplier * JumpAttackVelocity;
        //for (int i = 0; i < attackVelocity.Length; i++)
        //{
        //    attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        //}

        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        JumpAttackVelocity = originalJumpAttack;
        //for (int i = 0; i < attackVelocity.Length; i++)
        //{
        //    attackVelocity[i] = originalAttackVelocity[i];
        //}
        SpeedUpCo = null;

    }
    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    public void EnterAttackStateWithDelay()
    {

        if(queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    public IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }
    private void TryInteract()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;
            }
        }

        if (closest == null)
            return;

        closest.GetComponent<IInteractable>().Interact();

    }


    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Interact.performed += ctx => TryInteract();

        input.Player.QuickItemSlot_1.performed += ctx => inventory.TryUseQuickItemInSlot(1);
        input.Player.QuickItemSlot_2.performed += ctx => inventory.TryUseQuickItemInSlot(2);
    }
    public void TeleportPlayer(Vector3 position) => transform.position = position;
    protected override void Update()
    {
        base.Update();
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    ui.isOpen = !ui.isOpen;
        //    ui.OpenMerchantUI(ui.isOpen);
        //}
    }


    private void OnDisable()
    {
        input?.Disable();   
    }
    //public SkillBase GetSkillByIndex(int index)
    //{



    //}
 


    //private void TryCastSkill(int skillIndex)
    //{
    //    SkillBase skill = GetSkillByIndex(skillIndex); // 你自己实现的技能列表
    //    if (skill == null) return;

    //    // 设置 CastingState 的触发技能
    //    castingState.SetTriggerName(skill.triggerName);

    //    // 切换到 Casting 状态
    //    stateMachine.ChangeState(castingState);
    //}
}
