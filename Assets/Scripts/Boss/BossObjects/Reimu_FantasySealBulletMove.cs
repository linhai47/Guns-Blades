using UnityEngine;

public class FantasySealBulletMove : MonoBehaviour
{
    [Header("飞行参数")]
    public float speed = 8f;
    public bool enableTracking = true;
    public float trackTime = 1.2f;      // 前几秒跟踪玩家
    public float flyTime = 2.0f;        // 总飞行时间
    public float maxLifeTime = 5f;

    private Transform target;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 controlPos;
    private float elapsed = 0f;
    private bool isMoving = false;

    // 启动子弹移动
    public void Launch(Transform player)
    {
        target = player;
        startPos = transform.position;
        endPos = target.position;

        Vector3 mid = (startPos + endPos) / 2f;
        controlPos = mid + new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), 0);

        elapsed = 0f;
        isMoving = true;  // 开启 Update 中的移动逻辑
    }

    private void Update()
    {
        if (!isMoving) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / flyTime);

        if (enableTracking && elapsed <= trackTime && target != null)
        {
            // 仅前 trackTime 秒跟踪
            Vector3 mid = (startPos + target.position) / 2f;
            controlPos = mid + new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 2f), 0);
            endPos = target.position;
        }

        // 二阶贝塞尔曲线
        Vector3 pos = Mathf.Pow(1 - t, 2) * startPos +
                      2 * (1 - t) * t * controlPos +
                      Mathf.Pow(t, 2) * endPos;
        transform.position = pos;

        // 朝向移动方向
        Vector3 dir = (pos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

        if (elapsed >= flyTime || elapsed >= maxLifeTime)
        {
            isMoving = false;
            Destroy(gameObject, 0.1f);
        }
    }
}
