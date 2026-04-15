using System.Collections;
using UnityEngine;

public class Circle_Base : MonoBehaviour
{
    [Header("圆环设置")]
    public int segments = 100;      // 圆的分段数
    public float radius = 3f;       // 半径
    public float lineWidth = 0.1f;

    [Header("火焰粒子设置")]
    [SerializeField] private ParticleSystem flameParticlePrefab; // 火焰粒子预制体
    [SerializeField] private int particleCount = 4;              // 粒子数量（自动均匀分布）

    [Header("圆环旋转控制")]
    public float circleDuration = 2f; // 粒子沿圆环一圈的时间

    [Header("减速控制")]
    [SerializeField] private float slowRate = 1f; // 每秒增加的时长（越大减速越快）

    [Header("其它")]
    [SerializeField] private LineRenderer line;
   
    private Color originalColor;
    private GameObject particleParent;

    private void Awake()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.loop = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        if (line.material != null)
            originalColor = line.material.color;
    
        // 父物体管理粒子
        particleParent = new GameObject("FlameParticles");
        particleParent.transform.SetParent(transform);
        particleParent.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 绘制圆环并生成均匀分布的粒子
    /// </summary>
    public void DrawCircle(Vector3 center)
    {
        // 绘制圆环
 
        Vector3[] points = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2;
            points[i] = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }
        line.positionCount = segments;
        line.SetPositions(points);

        // 清空旧粒子
        foreach (Transform child in particleParent.transform)
            Destroy(child.gameObject);

        // 生成粒子（均匀分布）
        ParticleSystem[] particles = new ParticleSystem[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            float angle = (float)i / particleCount * Mathf.PI * 2;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            particles[i] = Instantiate(flameParticlePrefab, pos, Quaternion.identity, particleParent.transform);
            particles[i].Play();
        }

        // 启动粒子移动协程
        StartCoroutine(MoveParticlesAlongCircle(center, particles));
    }

    /// <summary>
    /// 粒子沿圆环移动（去掉贝塞尔曲线，线性匀速）
    /// </summary>
    private IEnumerator MoveParticlesAlongCircle(Vector3 center, ParticleSystem[] particles)
    {
        float elapsed = 0f;
        float currentDuration = circleDuration; // 当前一圈时间
        float maxDuration = 9999f;              // 近似停止时的 duration

        while (particles.Length > 0)
        {
            elapsed += Time.deltaTime;

            // 转速逐渐降低
            currentDuration += slowRate * Time.deltaTime;
            currentDuration = Mathf.Min(currentDuration, maxDuration);

            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] == null) continue;

                // t = 0~1 循环
                float t = ((float)i / particles.Length + elapsed / currentDuration) % 1f;

                // 直接线性速度（去掉贝塞尔）
                float angle = t * Mathf.PI * 2;

                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                particles[i].transform.position = pos;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 高亮圆环
    /// </summary>
    public void Highlight(float duration = 0.1f)
    {
        StartCoroutine(DoHighlight(duration));
    }

    private IEnumerator DoHighlight(float duration)
    {
        if (line.material == null) yield break;

        line.material.color = Color.yellow;
        yield return new WaitForSeconds(duration);
        line.material.color = originalColor;
    }
}
