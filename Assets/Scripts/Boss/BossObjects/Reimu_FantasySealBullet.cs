using DG.Tweening;
using UnityEngine;

public class Reimu_FantasySealBullet : BossObjects
{
    [Header("光线Prefab")]
    public GameObject beamPrefab;
    public float beamSpeed;

    [Header("动画设置")]
    public float scaleUp = 1.6f;
    public float scaleTime = 0.3f;
    public float shrinkTime = 0.3f;
    public float shotTime = 1f;
    public float beamLength = 6f;
    public float beamDuration = 0.4f;
    public float SkillDuration = 0.5f;
    [Header("光效aura")]
    public SpriteRenderer sr;
    public float auraSize = 5f;
    public float expandAuraTime = .1f;
    public float fadeTime = .5f;

    [Header("跟踪参数")]
    public bool enableTracking = true;
    public float trackDuration = 1.2f;   // 前几秒跟踪玩家
    public float flyDuration = 2f;       // 总飞行时间

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 controlPos;
    private float elapsed;
    private bool isMoving = false;
    private Vector3 originalScale;
    public TrailRenderer trail;

    private void Start()
    {
        originalScale = transform.localScale;
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = true;
        trail.Clear();   //  让它生成 mesh buffer
        trail.enabled = false;
    }
    public void ScaleAndFire(Vector3 center)
    {
        Vector3 targetScale = originalScale * scaleUp;

       

        trail.enabled = true;
        // DOTween 动画
        var seq = DG.Tweening.DOTween.Sequence();
        seq.Append(transform.DOScale(targetScale, scaleTime).SetEase(DG.Tweening.Ease.OutBack));
        var distance = transform.position - center;
        seq.Join(transform.DOLocalMove(center +  1.2f*distance,SkillDuration).SetEase(Ease.OutBack));
        seq.AppendCallback(() => Fire(center));
        seq.AppendInterval(shotTime);
        seq.Append(transform.DOScale(originalScale*2, shrinkTime).SetEase(DG.Tweening.Ease.InBack));

    }
    public void ScaleAndShot(Vector3 center)
    {
        Vector3 targetScale = originalScale * scaleUp;



        // DOTween 动画
        var seq = DG.Tweening.DOTween.Sequence();
        seq.Append(transform.DOScale(targetScale, scaleTime).SetEase(DG.Tweening.Ease.OutBack));
        seq.AppendCallback(() => Fire(center));
        seq.AppendInterval(shotTime);
        seq.Append(transform.DOScale(originalScale, shrinkTime).SetEase(DG.Tweening.Ease.InBack));
    }

    private void Fire(Vector3 center)
    {
        Vector3 dir = (transform.position - center).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 光线
        GameObject beam = Instantiate(beamPrefab, transform.position, Quaternion.identity);
        beam.GetComponent<TrackingBullet>().SetEntity(boss);
        beam.transform.rotation = Quaternion.Euler(dir);

        var laser = beam.GetComponent<Reimu_BulletLaser>();
        
        if (laser != null)
        {
            laser.SetDirectionAndVelocity(dir, angle);
        }

        var seq = DG.Tweening.DOTween.Sequence();
        seq.Append(sr.transform.DOScale(auraSize, expandAuraTime).SetEase(DG.Tweening.Ease.OutQuad));
        seq.Join(sr.DOFade(1, expandAuraTime).SetEase(DG.Tweening.Ease.OutQuad));
        seq.Append(sr.DOFade(0f, fadeTime).SetEase(DG.Tweening.Ease.InQuad));
        seq.OnComplete(() => Destroy(beam));

   
    }

    

   

}
