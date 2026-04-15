using System.Collections;
using UnityEngine;

public class BezierBullet : Projectile
{
    [Header("贝塞尔参数")]
    public float controlOffset = 3f;        // 控制点偏移
    public float raycastRadius = 10f;       // 检测玩家半径
    public bool isMoving = false;

    private Vector3 startPos;
    private Vector3 midPos;
    private Vector3 lastPos;

    public float trackTime = .4f;
    private float percentSpeed;
    private float percent = 0;
    private float bezierRatio = 1f;
    private float searchRatio = .5f;
    private float Range = 100f;
    private float lifeTimeCountDown;
    private float curveHeight = 3f;

    private Vector3 linearDir;

    private bool isTrackingPhase = true; //  新增：是否还在跟踪阶段
    public Collider2D col;
    public float midRate = .2f;
    public float extendRate = 2f;
    private Vector3 finalTarget;
    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }
    protected override void Update()
    {
        base.Update();
        if (!isMoving) return;

        if (isTrackingPhase)
        {
            if (trackTime > 0)
            {
                if (targetTransform)
                {
                    percent += percentSpeed * Time.deltaTime;
      
                    Vector3 currentPos = transform.position;
                    transform.position = Bezier(percent, startPos, midPos, targetTransform.position);
                    lastPos = currentPos;
                }
                else
                {
                    FindTarget();
                }
                trackTime -= Time.deltaTime;
            }
            else
            {
                //  跟踪结束，开始第二阶段
                StartPostBezier();
            }
        }
        else // 第二阶段：后续贝塞尔飞行
        {
            // 第二阶段：延续曲线方向继续飞行
            percent += percentSpeed * Time.deltaTime;
            if (percent >extendRate)
            {
                Destroy(gameObject);
                return;
            }

          
            Vector3 currentPos = transform.position;
            transform.position = Bezier(percent, startPos, midPos,finalTarget);
            lastPos = currentPos;
        }
        Debug.Log(percent);
    }

    private void StartPostBezier()
    {
        Debug.Log("Enter PostBezier");
        isTrackingPhase = false;

        finalTarget = targetTransform.position;
  
        

    }

    private void FindTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 direction = (currentPos - lastPos).normalized;
        transform.Translate(speed * Time.deltaTime * direction);
        lastPos = currentPos;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, raycastRadius, whatIsPlayer);
        if (hit == null) return;

        targetTransform = hit.transform;
        startPos = transform.position;
        midPos = GetMiddlePosition(startPos, direction, speed);
        percent = 0;
        percentSpeed = speed / ((Vector3)targetTransform.position - startPos).magnitude;
    }

    public void Fire()
    {
        col.enabled = true;
        startPos = transform.position;
        isMoving = true;
        lastPos = startPos - new Vector3(1, 0, 0);
        lifeTimeCountDown = Range / speed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsPlayer) == 0) return;

        ApplyDamageToTarget(collision);

        col.enabled = false;
        StartCoroutine(DelayStartPostBezier());
    }
    private IEnumerator DelayStartPostBezier()
    {
        yield return new WaitForSeconds(0.05f); // 让子弹先飞出一点再开始新曲线
        trackTime = 0f;
    }

    Vector3 GetMiddlePosition(Vector3 Pos, Vector3 dir, float v)
    {
        Vector3 basePos = Pos + bezierRatio * dir * v;
        Vector3 base2Pos =( Pos + targetTransform.position) / 2; 
        float distance = (targetTransform.position- Pos).magnitude;
        float sDistance = (targetTransform.position - basePos).magnitude;
        float ratio = sDistance / distance;

        Vector3 finalPos = Pos + Mathf.Min(midRate,ratio) * dir;
        float yOffset = Mathf.Sin(.15f * Mathf.PI) * curveHeight;
        return finalPos + new Vector3(0, yOffset, 0);
    }

    public Vector3 Bezier(float t, Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = Vector3.LerpUnclamped(a, b, t);
        var bc = Vector3.LerpUnclamped(b, c, t);
        return Vector3.LerpUnclamped(ab, bc, t);
    }

    public void SetCurveHeight(float curveHeight)
    {
        this.curveHeight = curveHeight;
    }
}
