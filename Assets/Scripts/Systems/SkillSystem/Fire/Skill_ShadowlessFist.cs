using System.Collections;
using UnityEngine;

public class Skill_ShadowlessFist : SkillComplex
{
    public GameObject shadowlessFistPrefab;

    private SkillObject_ShadowlessFist skillObject_ShadowlessFist;
    [SerializeField] protected float checkRadius = 1;
    [SerializeField] protected Vector3 Offset;
    [SerializeField] private bool isNumerous=false;
    private float gravity;
    private Collider2D[] enemys;
    [SerializeField] private float attackInterval = 0.3f;

    [SerializeField] private GameObject attackParticlePrefab;
    [SerializeField] private GameObject attackTrailPrefab;
    [SerializeField] private float trailMoveSpeed = 30f;
    [SerializeField] private float curveHeight = 1.5f;

    [SerializeField] private float LineyScale = 1f;
    [SerializeField] private float LinezScale = 1f;

    [SerializeField] private float TotalAttackTime = .5f;

    [SerializeField] private float particleInterval = 0.001f; // 每隔多少秒生成一次火花
    [SerializeField] private float particleLifetime = 0.1f;  // 粒子对象生存时间

    [SerializeField] private GameObject LockOnPrefab;
    [SerializeField] private float LockOnTime = .2f;

    [SerializeField] private GameObject FlameRingPrefab; // 改名为Prefab


    [SerializeField] private float rangeNumber = 1f;
    private Circle_Base ringCircle;



    protected override void Awake()
    {
        base.Awake();
        anim = player.anim;
        if (FlameRingPrefab == null)
        {
            Debug.LogError(" FlameRingPrefab 没有赋值！");
            return;
        }

        // 在玩家身上生成一个实例
        
        playerStats = player.GetComponent<Entity_Stats>();

    }

    public override void TryUseSkill()
    {
        base.TryUseSkill();

        //AttackTarget();

    }

    public void LockOnTarget()
    {
      
        enemys = SearchCollisionInCircle(player.transform, checkRadius);
        if (ringCircle == null && FlameRingPrefab != null)
        {
     
            // 实例化并挂到玩家身上
            GameObject ringObj = Instantiate(FlameRingPrefab, player.transform.position, Quaternion.identity);
            ringObj.transform.SetParent(player.transform);
            ringObj.transform.localPosition = Vector3.zero;

            ringCircle = ringObj.GetComponent<Circle_Base>();
            if (ringCircle != null)
            {
                ringCircle.radius = checkRadius /rangeNumber;
                ringCircle.DrawCircle(player.transform.position);
                ringCircle.Highlight(1f); // 高亮0.1秒
                Destroy(ringObj, 1f); //  圆环显示后自动销毁
            }

         
        }
        if (enemys.Length <= 0) return;


        foreach (Collider2D target in enemys)
        {
            CreateLockOn(target.transform);

        }

    }

    public void AttackTarget()
    {
        
        enemys = SearchCollisionInCircle(player.transform, checkRadius);

        if (enemys.Length > 0)
        {
            attackInterval = TotalAttackTime / enemys.Length;
            StartCoroutine(AttackEnemiesSequentially());
        }
    }

    public Collider2D[] SearchCollisionInCircle(Transform t, float radius)
    {

        return Physics2D.OverlapCircleAll(t.position + Offset, radius, whatIsEnemy);

    }

    private IEnumerator AttackEnemiesSequentially()
    {
        Transform previousTarget = transform;

        // 隐藏玩家渲染器，保证技能视觉干净
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
        if (player.input != null)
            player.input.Disable();

        int enemyCount = enemys.Length;
        if (enemyCount == 0)
            yield break;

        // 每条攻击间隔 = 总攻击时间 / 敌人数
        float currentInterval = TotalAttackTime / enemyCount;

        foreach (Collider2D target in enemys)
        {
            if (target == null) continue; // 防止敌人消失

            // 创建轨迹，传入 duration
            if (previousTarget != null)
                CreateAttackLine(previousTarget.position, target.transform.position, currentInterval);

            // 生成技能对象
            CreateShadowlessFist(target.transform);

            // 伤害逻辑
            Single_Damage(target.transform, 1f);

            previousTarget = target.transform;

            // 等待与轨迹时间一致
            yield return new WaitForSeconds(currentInterval);
        }

        // 恢复玩家渲染器
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
            renderer.enabled = true;
        if (player.input != null)
            player.input.Enable();

    }

    // 改造后的 CreateAttackLine，传入 duration
    private void CreateAttackLine(Vector3 startPos, Vector3 endPos, float duration)
    {
        if (attackTrailPrefab == null) return;

        GameObject trailObj = Instantiate(attackTrailPrefab, startPos, Quaternion.identity);
        StartCoroutine(MoveTrailLinearWithBezierSpeed(trailObj, startPos, endPos, duration));
    }


    private void CreateParticle(Vector3 pos , Vector3 startPos , Vector3 endPos)
    {
        if (attackParticlePrefab == null) return;
        GameObject particle = Instantiate(attackParticlePrefab, pos, Quaternion.identity);
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();

        Vector3 moveDir = (endPos - startPos).normalized;
        Vector3 particleDir = -moveDir; // 取反方向
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = particleDir * 5f; // 可调速度
        ps.Emit(emitParams, 1); // 发射一颗粒子
        Destroy(particle,.1f);
    }

    
    private void CreateLockOn(Transform enemyTransform)
    {
        GameObject lockOnObj = Instantiate(LockOnPrefab, enemyTransform.position, Quaternion.identity);
        lockOnObj.transform.SetParent(enemyTransform); // 直接跟随敌人移动
        lockOnObj.transform.localPosition = Vector3.zero; // 可选偏移
        Destroy(lockOnObj, LockOnTime);
    }

    // 改造后的 Coroutine，duration 精确控制

    private IEnumerator MoveTrailLinearWithBezierSpeed(GameObject trailObj, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        duration /= 3;
        int cnt = 0;
        float particleTimer = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float linearT = Mathf.Clamp01(elapsed / duration);

            float tBezier = Mathf.Pow(1 - linearT, 3) * 0f +
                            3 * Mathf.Pow(1 - linearT, 2) * linearT * 0.2f +
                            3 * (1 - linearT) * Mathf.Pow(linearT, 2) * 0.8f +
                            Mathf.Pow(linearT, 3) * 1f;

            Vector3 pos = Vector3.Lerp(startPos, endPos, tBezier);
            trailObj.transform.position = pos;

            particleTimer += Time.deltaTime;

            if (isNumerous)
            {
                if (particleTimer >= particleInterval)
                {
                    CreateParticle(pos ,startPos,endPos);
                    particleTimer = 0f;
                }
            }
            else
            {
                cnt++;
              if(cnt  % 1== 0)   CreateParticle(pos, startPos, endPos);
            }

                yield return null;
        }

        trailObj.transform.position = endPos;
        Destroy(trailObj);
    }



    public void CreateShadowlessFist(Transform enemyTransform)
    {
        GameObject shadowlessFist = Instantiate(shadowlessFistPrefab, enemyTransform.position, Quaternion.identity);

        skillObject_ShadowlessFist = shadowlessFist.GetComponent<SkillObject_ShadowlessFist>();

        skillObject_ShadowlessFist.Setup(this);
    }


}
