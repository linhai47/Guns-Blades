using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class ReimuSkill_FantasySeal : MonoBehaviour
{
    public Entity entity;

    [Header("Prefab 设置")]
    public GameObject[] bulletPrefabs; // 三种子弹预制体

    [Header("控制参数")]
    public Transform center;
    public int bulletCount = 12;
    public float radius = 3f;
    public float expandDuration = 1f;     // 扩散时间
    public float rotateDuration = 12f;    // 一圈旋转时间

    [Header("目标玩家")]
    public Transform player;

    private List<Reimu_FantasySealBullet> bullets = new List<Reimu_FantasySealBullet>();
    private List<TrackingBullet> tracking = new List<TrackingBullet>();
    private List<BezierBullet> beziers = new List<BezierBullet>();
    private GameObject ring; // 环形容器

    [Header("发射设置")]
    public float ShotInterval = 1.2f;
    public float finalLaunchDelay = 6f; // 扩散+旋转后多久收束
    public float curveHeightRange = 3f;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    public void FantasySeal()
    {
        Debug.Log("=== [FantasySeal] 技能启动 ===");
        CancelInvoke();

        // 清理上一次
        if (ring != null)
        {
            DOTween.Kill(ring.transform);
            Destroy(ring);
            ring = null;
        }
        bullets.Clear();
        tracking.Clear();
        beziers.Clear();
        // 创建新环
        ring = new GameObject("BulletRing");
        ring.transform.SetParent(center);
        ring.transform.localPosition = Vector3.zero;

        // 生成子弹
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject prefab = bulletPrefabs[i % bulletPrefabs.Length];
            GameObject bulletObj = Instantiate(prefab, ring.transform);
            bulletObj.GetComponent<TrackingBullet>().SetEntity(entity);
            bulletObj.transform.localPosition = Vector3.zero;
            bulletObj.GetComponent<BezierBullet>().SetEntity(entity);
            float angle = i * 360f / bulletCount;
            Vector3 localTarget = Quaternion.Euler(0, 0, angle) * Vector3.right * radius;

            // 扩散动画
            bulletObj.transform
                .DOLocalMove(localTarget, expandDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(i * 0.01f);

            var bullet = bulletObj.GetComponent<Reimu_FantasySealBullet>();
            bullets.Add(bullet);
            var trackbullet = bulletObj.GetComponent<TrackingBullet>();
            tracking.Add(trackbullet);
            var bezier = bulletObj.GetComponent<BezierBullet>();
            beziers.Add(bezier);
        }

        // 环旋转
        ring.transform.DORotate(
            new Vector3(0, 0, 360),   // 旋转到目标角度
            rotateDuration,           // 旋转耗时
            RotateMode.FastBeyond360  // 旋转模式，允许超过360°
        ).SetEase(Ease.Linear);      // 匀速旋转


        // 延迟调用发射
        Invoke(nameof(FireSequence), expandDuration);
        Invoke(nameof(LaunchAllBulletsToPlayer), expandDuration + finalLaunchDelay);

        Debug.Log("=== [FantasySeal] 技能初始化完毕 ===");
    }

    private void FireSequence()
    {
        if (bullets.Count < 12) return;

        for (int i = 0; i < 6; i++)
        {
            var b1 = bullets[i];
            var b2 = bullets[i + 6];
            float delay = i * ShotInterval;

            DOVirtual.DelayedCall(delay, () =>
            {
                b1.ScaleAndShot(center.position);
                b2.ScaleAndShot(center.position);
            });

            var b3 = beziers[i];
            var b4 = beziers[i + 6];
            b3.SetCurveHeight(curveHeightRange);
            b4.SetCurveHeight(-curveHeightRange);
        }
    }

    private void LaunchAllBulletsToPlayer()
    {
        foreach (var b in bullets)
        {
            b.ScaleAndFire(center.position);
            b.transform.SetParent(null);
        }

        foreach (var b in beziers)
        {
            b.Fire();
     
        }

        //foreach (var b in tracking)
        //{
        //    b.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        //    if (b != null)
        //        b.isMoving = true;
        //    var distance = b.transform.position - center.position;


        //    float angle = Mathf.Atan2(distance.x, distance.y);

        //    b.SetRotation(0);
        //}
    }
}
