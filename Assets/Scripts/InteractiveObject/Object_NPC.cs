using System;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
public enum NPCState
{
    Idle,
    Patrol,
    Talk,
    Taskover,
    //Win_Before,
}
public enum NPCname
{

    None,米拉,阿克斯塔尔,雷格斯,希尔薇雅,月,


    风之圣地,


    骷髅,
}
public enum EnemyName
{
    None,骷髅
}
public class Object_NPC : MonoBehaviour, IInteractable
{
    protected Transform player;
    protected UI ui;
    public NPCState currentState;
    protected Animator anim;
    public Rigidbody2D rb;
    [SerializeField] private Transform bubble;
    protected Player_QuestManager questManager;


    [SerializeField] private Transform npc;
    [SerializeField] protected Vector2 MoveSpeed;
    public RewardType rewardNpc;
    protected bool facingRight = false;
    protected int facingDir = -1;
    protected bool hasSeenPlayer = false;

    [Header("Patrol 设置")]
    public float patrolRange = 3f;   // 徘徊范围（中心点左右）
    protected float leftLimit;
    protected float rightLimit;
    protected Vector2 startPos;
    public float idleDuration = 2f;  // Idle 停顿时间
    protected float idleTimer;
    public bool isOpen = false;
    [SerializeField] public bool canPatrol = true;
    private bool isPatroling;
    public Inventory_Player inventory;
    public Inventory_Merchant merchant;
    public NPCname Npcname;
    protected virtual void Start()
    {
        questManager = Player.instance.questManager;
    }

    protected virtual void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        ui =FindAnyObjectByType<UI>();
        anim= GetComponentInChildren<Animator>();

        SetState(NPCState.Idle);
        startPos = transform.position;
        leftLimit = startPos.x - patrolRange;
        rightLimit = startPos.x + patrolRange;
        isPatroling = canPatrol;
    }
    protected void Flip()
    {
        facingDir *= -1;  // 反转方向（1 -> -1 或 -1 -> 1）
        facingRight = !facingRight;
        SpriteRenderer sr = npc.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.flipX = !sr.flipX;


    }

    public virtual void Interact()
    {
        Debug.Log(Npcname.ToString());


        if (questManager == null)
        {
            questManager = Player.instance.questManager;

            questManager.questNPC = this;
        }
        questManager.questNPC = this;

    }
    public virtual void MeetNPC()
    {

        if (!Enum.TryParse<EnemyName>(Npcname.ToString(), out var enemy))
        {
            questManager.AddProgress(Npcname);
        }
        else
        {
            Debug.Log($"{Npcname} 是敌人，不增加任务进度");
        }

    }
    protected virtual void Enter()
    {
        

    }
    
   

    // 改 currentState就行
    protected virtual void Update()
    {
        HandleNpcFlip();
        
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    ChangeTalk();
        //}
        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeTaskOver();
        }
        
        switch (currentState)
        {
            case NPCState.Idle:
                idleTimer += Time.deltaTime;
                ChangeIdle();
                break;
            case NPCState.Patrol:
                ChangePatrol();
                break;
            case NPCState.Talk:
                ChangeTalk();
                break;
            case NPCState.Taskover:
                ChangeTaskOver();
                break;
            //case NPCState.Win_Before:
            //   ChangeWinBefore();
            //    break;
        }



    }
    public void SetState(NPCState newState)
    {
        anim.SetBool("idle", false);
        anim.SetBool("patrol", false);
        anim.SetBool("talk", false);
        anim.SetBool("taskover", false);
        //anim.SetBool("winbefore", false);
        currentState = newState;
        switch (newState)
        {
            case NPCState.Idle:
                anim.SetBool("idle", true);
         
                rb.linearVelocity = Vector2.zero;
                idleTimer = 0f;
                break;
            case NPCState.Patrol:
                anim.SetBool("patrol", true);
                break;
            case NPCState.Talk:
                anim.SetBool("talk", true);
                break;
            case NPCState.Taskover:
                anim.SetBool("taskover", true);
                break;
            //case NPCState.Win_Before:
            //    anim.SetBool("winbefore", true);
            //    break;
        }
    }

    private void HandleNpcFlip()
    {
        if (player == null || npc == null) return;

        //if (currentState != NPCState.Idle) return;
        if(currentState != NPCState.Idle)
        {
            if (facingDir > 0 && transform.position.x > rightLimit)
            {
                Flip();
                SetState(NPCState.Idle);
                return;
            }
            else if (facingDir < 0 && transform.position.x < leftLimit)
            {
                Flip();
                SetState(NPCState.Idle);
                return;
            }

        }

        if (hasSeenPlayer == false) return;
        
        if (npc.position.x > player.position.x && facingRight)
        {
            //npc.transform.Rotate(0, 180, 0);
            SpriteRenderer sr = npc.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.flipX = !sr.flipX;
            facingRight = false;
            facingDir *= -1;
        }
        else if (npc.position.x < player.position.x && !facingRight)
        {
            //npc.transform.Rotate(0, 180, 0);
            SpriteRenderer sr = npc.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.flipX = !sr.flipX;
            facingRight = true;
            facingDir *= -1;
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        inventory = player.GetComponent<Inventory_Player>();
        if(merchant != null) merchant.SetInventory(inventory);
        hasSeenPlayer = true;

       

        // 重置 Idle 计时器

        // NPC 切换到 Idle 状态
        //SetState(NPCState.Idle);
        if (collision.CompareTag("Player"))
        {


            isPatroling = false;
            SetState(NPCState.Idle);
            

            //Debug.Log($"{name}: Player entered NPC range!");

        }

    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        ui.HideAllTooltips();
        if (collision.CompareTag("Player"))
        {
         
            if (canPatrol)
            {
               isPatroling = true;
        
            }

    
        }
    }
 
    protected virtual void ChangeIdle()
    {
        //SetState(NPCState.Idle);
        
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            if (isPatroling) ChangePatrol();
        }
    }
    protected virtual void ChangePatrol()
    {

        SetState(NPCState.Patrol);
        hasSeenPlayer = false;
        rb.linearVelocity = new Vector2(MoveSpeed.x * facingDir, 0);

        
    }
    protected virtual void ChangeTalk()
    {
        SetState(NPCState.Talk);

    }
    protected virtual void ChangeTaskOver()
    {
        SetState(NPCState.Taskover);

    }

    protected virtual void ChangeWinBefore()
    {
        //SetState(NPCState.Win_Before);
    }
}
