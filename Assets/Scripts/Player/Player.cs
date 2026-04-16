using System;
using System.Collections;
using System.Globalization;
using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("武器挂载")]
    public Transform weaponHoldPoint;
    public WeaponDataSO currentWeaponData;
    public Weapon currentWeaponInstance;

   
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

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public void OnPlayerSpawned(int playerID)
    {
        int targetLayer;

        // 如果是房主（Player 1）
        if (playerID == 1)
        {
            targetLayer = LayerMask.NameToLayer("Player1");
        }
        // 如果是客户端（Player 2）
        else
        {
            targetLayer = LayerMask.NameToLayer("Player2");
        }

        // 把这个角色全身上下的皮都换成对应的 Layer
        SetLayerRecursively(gameObject, targetLayer);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        if (currentWeaponData != null) EquipWeapon(currentWeaponData);

    }
    public void EquipWeapon(WeaponDataSO newWeaponData)
    {
        currentWeaponData = newWeaponData;

        // 1. 如果手上已经有枪了，先销毁旧的
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance.gameObject);
        }

        // 2. 实例化新武器模型
        GameObject weaponObj = Instantiate(newWeaponData.weaponPrefab, weaponHoldPoint);

        // 3. 归零位置和旋转，让枪的把手正好对齐你的握枪点
        weaponObj.transform.localPosition = Vector3.zero;
        weaponObj.transform.localRotation = Quaternion.identity;

        // 4. 获取武器脚本并初始化
        currentWeaponInstance = weaponObj.GetComponent<Weapon>();
        if (currentWeaponInstance != null)
        {
            // 把数据传给枪，这样枪才知道自己该射什么子弹、有多高伤害
            currentWeaponInstance.SetupWeapon(newWeaponData, this);
        }
    }

    private void AimTowardsMouse()
    {
        if (weaponHoldPoint == null || Mouse.current == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        // 计算鼠标相对于握枪点的方向向量
        Vector3 aimDirection = mousePosition - weaponHoldPoint.position;

        // 【核心魔法】：X轴方向乘上朝向系数
        // 因为你的 Entity 翻转使用的是 Rotate(0,180,0)，当角色朝左时，
        // 父物体的局部坐标系其实已经翻转了，这里通过 facingDir 同步调整计算逻辑
        float localX = aimDirection.x * facingDir;
        float localY = aimDirection.y;

        // 算出相对角度，直接应用，不需要任何钳制！
        float angle = Mathf.Atan2(localY, localX) * Mathf.Rad2Deg;
        weaponHoldPoint.localRotation = Quaternion.Euler(0, 0, angle);
    }
    public override void HandleFlip(float xVelocity)
    {
        // 留空即可！什么都不写。
    }
    private void FlipCharacterTowardsMouse()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // 如果鼠标在右边，但角色当前朝左，则翻转
        if (mousePosition.x > transform.position.x && !facingRight)
        {
            Flip(); // 调用 Entity 基类现成的 Flip()，自动处理 Rotate(0,180,0) 和 事件派发
        }
        // 如果鼠标在左边，但角色当前朝右，则翻转
        else if (mousePosition.x < transform.position.x && facingRight)
        {
            Flip(); // 同上
        }
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
        if (!isLocalPlayer) return;
        FlipCharacterTowardsMouse();
        AimTowardsMouse();
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
