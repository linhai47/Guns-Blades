using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Entity_Combat : MonoBehaviour 
{
    public event Action<float> OnDoingPhysicalDamage;
    Entity_VFX vfx;
    Entity_Stats entityStats;
    Entity_SFX sfx;
    Entity attackerEntity;
    [Header("Target detection")]
    public float targetCheckRadius = 1;
    [SerializeField] private Transform targetCheck;
    [SerializeField] private LayerMask whatIsTarget;
    public DamageScaleData basicAttackScale;

    [Header("Active Window Settings")]
    [Tooltip("默认的攻击生效时长（秒），如果用动画事件来控制，可以不使用此默认值）")]
    public float defaultActiveDuration = 0.55f;
    [Tooltip("可选的预备时间（startup）在 StartAttackWindow 时生效")]
    public float startup = 0f;

    private bool isAttackActive = false;                 // 当前是否处于攻击的 active 窗口
    private HashSet<GameObject> alreadyHit;              // 本次攻击周期已命中的对象集合
    private Coroutine attackCoroutine = null;            // 如果用协程自动结束 Active 窗口时保存的协程

    //[Header("Visual -> Parent Sync (动画最后烘焙)")]
    //[Tooltip("指向Visual子物体（Animator 所在），若不设会尝试自动查找第一个子物体")]
    //public Transform visual;

    //[Tooltip("Visual 子物体上的 Animator（可选，若为空会尝试自动获取）")]
    //public Animator visualAnimator;

    //[Tooltip("是否在烘焙时同步 Y 轴（如果由物理 gravity 控制 Y，设为 false）")]
    //public bool bakeSyncY = true;
    //private Rigidbody2D parentRb;

    protected virtual void Awake()
    {
        vfx = GetComponentInParent<Entity_VFX>();
        sfx = GetComponentInParent<Entity_SFX>();
        entityStats = GetComponentInParent<Entity_Stats>();
        attackerEntity = GetComponentInParent<Entity>();

        alreadyHit = new HashSet<GameObject>();

        //if (visual == null)
        //{
        //    // 尝试找到名为 "Visual" 的子物体，否则第一个子物体
        //    Transform vf = transform.Find("Animator");
        //    if (vf == null && transform.childCount > 0) vf = transform.GetChild(0);
        //    visual = vf;
        //}
        //if (visualAnimator == null && visual != null)
        //    visualAnimator = visual.GetComponent<Animator>();

        //// parentRb 尝试获取父层的刚体（包含在自身或父物体）
        //parentRb = GetComponentInParent<Rigidbody2D>();
        //if (parentRb == null)
        //{

        //    Debug.LogWarning($"{name}: Parent Rigidbody2D not found; ApplyVisualFinalPositionToParent will do nothing.");
        //}
    }


    public void PerformAttack()
    {
        bool targetGotHit = false;
        AttackData attackData = entityStats.GetAttackData(basicAttackScale);
        float physicalDamage = attackData.physicalDamage;
        float elementalDamage = attackData.elementalDamage;
        ElementType element = attackData.element;
        Entity attackEntity = attackData.attackEntity;
        bool isCrit = attackData.isCrit;
       
        foreach (var target in getDetectedColliders ())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
            if (damagable == null) continue;
            targetGotHit = damagable.TakeDamage(physicalDamage, elementalDamage, element, transform, isCrit);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            if (element!= ElementType.None)
            {
         
                statusHandler?.ApplyStatusEffect(element, attackData.effectData , attackerEntity);
            }


            if (targetGotHit)
            {
                OnDoingPhysicalDamage?.Invoke(physicalDamage);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element);
                sfx?.PlayAttackHit();
            }
            
        }

        if (targetGotHit == false) sfx?.PlayAttackMiss();

    }


    private Collider2D[] getDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position,targetCheckRadius, whatIsTarget);


    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);



    }

    private bool ProcessHit(Collider2D targetCollider, AttackData attackData)
    {
        IDamagable damagable = targetCollider.GetComponent<IDamagable>();
        if (damagable == null) return false;

        float physicalDamage = attackData.physicalDamage;
        float elementalDamage = attackData.elementalDamage;
        ElementType element = attackData.element;
        bool isCrit = attackData.isCrit;
        Entity attackEntity = attackData.attackEntity;

        // 把判伤调用集中到这里
        bool targetGotHit = damagable.TakeDamage(physicalDamage, elementalDamage, element, transform, isCrit);

        Entity_StatusHandler statusHandler = targetCollider.GetComponent<Entity_StatusHandler>();
        if (element != ElementType.None)
        {
            statusHandler?.ApplyStatusEffect(element, attackData.effectData, attackerEntity);
        }

        if (targetGotHit)
        {
            OnDoingPhysicalDamage?.Invoke(physicalDamage);
            vfx.CreateOnHitVFX(targetCollider.transform, attackData.isCrit, element);
            sfx?.PlayAttackHit();
        }

        return targetGotHit;
    }

    private void FixedUpdate()
    {
        if (isAttackActive)
        {
            DoActiveHitChecks();
        }
    }
    private void DoActiveHitChecks()
    {
        // 获取一次攻击数据（假设一个攻击周期内数据不变）
        AttackData attackData = entityStats.GetAttackData(basicAttackScale);

        Collider2D[] hits = getDetectedColliders();
        bool anyEffectiveHitThisFrame = false;

        foreach (var col in hits)
        {
            var go = col.gameObject;
            if (alreadyHit.Contains(go)) continue; // 本攻击周期已命中过，跳过

            // 先标记，避免短时间内重复多次尝试（也可以在成功后再标记，按需求调整）
            alreadyHit.Add(go);
            if (col == null)
            {
                Debug.Log("col is null");
            }
            if (attackData == null)
            {
                Debug.Log("Attack Data is null");
            }
            bool effective = ProcessHit(col, attackData);
            if (effective) anyEffectiveHitThisFrame = true;
        }

        // 不在这里播放 Miss，Miss 的判断在 FinishAttackWindow 统一处理
    }
    public void AttackStartEvent()
    {
        StartAttackWindow(defaultActiveDuration);
    }

    public void AttackEndEvent()
    {
        EndAttackWindow();
    }
    public void StartAttackWindow(float autoDuration = -1f)
    {
        // 取消旧协程
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        attackCoroutine = StartCoroutine(AttackWindowCoroutine(autoDuration));
    }

    // 手动结束 active 窗口（通常由动画事件调用）
    public void EndAttackWindow()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        FinishAttackWindow();
    }

    private IEnumerator AttackWindowCoroutine(float autoDuration)
    {
        // 可选 startup（如果不需要 startup，可设置为 0）
        if (startup > 0f)
            yield return new WaitForSeconds(startup);

        isAttackActive = true;
        alreadyHit.Clear(); // 清空本次攻击周期的命中记录

        if (autoDuration > 0f)
        {
            yield return new WaitForSeconds(autoDuration);
            FinishAttackWindow();
        }
        else
        {
            // 如果没有自动时长，则等待外部显式调用 EndAttackWindow()
            yield break;
        }
    }
    private void FinishAttackWindow()
    {
        bool anyHit = alreadyHit.Count > 0;
        //if (!anyHit)
        //    sfx?.PlayAttackMiss();

        isAttackActive = false;
        alreadyHit.Clear();
    }
    //public void ApplyVisualFinalPositionToParent()
    //{
    //    if (visual == null)
    //    {
    //        Debug.LogWarning($"{name}: ApplyVisualFinalPositionToParent called but visual is null.");
    //        return;
    //    }

    //    if (parentRb == null)
    //    {
    //        // 尝试再次获取
    //        parentRb = GetComponentInParent<Rigidbody2D>();
    //        if (parentRb == null)
    //        {
    //            Debug.LogWarning($"{name}: No parent Rigidbody2D found. Can't bake visual position to parent.");
    //            return;
    //        }
    //    }

    //    // 计算 visual 的世界位置（最终位置）
    //    Vector3 visualWorld = visual.position;


    //    Vector2 targetPos = parentRb.position;
    //    targetPos.x = visualWorld.x;
    //    if (bakeSyncY)
    //        targetPos.y = visualWorld.y;


    //    parentRb.MovePosition(targetPos);


    //    visual.localPosition = Vector3.zero;


    //}
}
