using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamagePopup : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshPro normalTmp;     // 普通伤害文本
    public TextMeshPro critTmp;       // 暴击文本（单独字体）

    [Header("Settings")]
    public float floatDistance = 1.0f;
    public float duration = 0.9f;
    public float popScale = 1.2f;
    public Vector2 randomOffset = new Vector2(0.2f, 0.3f);
    public float xOffset = 0f;
    public float endScale = 0.15f;
    public float fadeDelay = 0.2f;
    public float controlPointHeightMultiplier = 1.2f;

    [Header("Colors")]
    public Color colorNone = Color.white;
    public Color colorFire = new Color(1f, 0.55f, 0.2f);
    public Color colorWind = new Color(0.5f, 1f, 0.7f);
    public Color colorLightning = new Color(1f, 0.95f, 0.4f);
    public Color critColor = new Color(1f, 0.5f, 0f); // 赤橙色

    [Header("Crit Settings")]
    public float critPopScale = 1.8f;
    public float critEndScale = 0.15f;
    public float critDuration = 0.9f;

    private Transform rect;
    private Sequence seq;

    private void Awake()
    {
        rect = transform;
        if (normalTmp == null) normalTmp = GetComponentInChildren<TextMeshPro>();
        if (critTmp != null) critTmp.gameObject.SetActive(false);
    }

    public void Play(Vector3 worldPos, int damage, ElementType element, bool isCrit)
    {
        gameObject.SetActive(true);
        seq?.Kill();
        Debug.Log(isCrit + "isCrit");
        // 随机初始偏移
        Vector3 randomOffsetVec = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0f, randomOffset.y),
            0f);
        Vector3 startPos = worldPos + randomOffsetVec;
        rect.position = startPos;

        seq = DOTween.Sequence();

        if (isCrit && critTmp != null)
        {
            // 暴击逻辑
            critTmp.gameObject.SetActive(true);
            critTmp.text = damage.ToString();
            critTmp.color = critColor;

            rect.localScale = Vector3.one * critPopScale;

            // 原地快速放大
            seq.Append(rect.DOScale(Vector3.one * popScale * 2f, 0.02f).SetEase(Ease.OutBack));

            float Xoffset = Random.Range(-xOffset, xOffset);
            PlayBezierMove(startPos, floatDistance * 0.5f, Xoffset * 0.5f, critDuration);

            // fade
            seq.Join(critTmp.DOFade(0f, critDuration).SetEase(Ease.OutQuad).SetDelay(fadeDelay));
            seq.Join(rect.DOScale(Vector3.one * critEndScale, critDuration).SetEase(Ease.OutQuad));
        }
        else
        {
            // 普通伤害逻辑
            if (normalTmp != null)
            {
                normalTmp.gameObject.SetActive(true);
                normalTmp.text = damage.ToString();
                normalTmp.color = GetColorForElement(element);
                normalTmp.alpha = 1f;
            }

            rect.localScale = Vector3.one * popScale;

            seq.Append(rect.DOScale(Vector3.one * popScale * 1.2f, 0.15f).SetEase(Ease.OutBack));

            float Xoffset = Random.Range(-xOffset, xOffset);
            PlayBezierMove(startPos, floatDistance, Xoffset, duration);

            if (normalTmp != null)
            {
                seq.Join(normalTmp.DOFade(0f, duration).SetEase(Ease.OutQuad).SetDelay(fadeDelay));
            }

            seq.Join(rect.DOScale(Vector3.one * endScale, duration).SetEase(Ease.OutQuad));
        }

        seq.OnComplete(() =>
        {
            seq.Kill();
            if (normalTmp != null) normalTmp.gameObject.SetActive(false);
            if (critTmp != null) critTmp.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    private void PlayBezierMove(Vector3 startPos, float height, float xOffset, float duration)
    {
        Vector3 controlPoint = startPos + Vector3.up * height * controlPointHeightMultiplier
                               + new Vector3(xOffset * 0.5f, 0f, 0f);
        Vector3 endPos = startPos + new Vector3(xOffset, height * 0.5f, 0f);

        seq.Append(DOTween.To(() => 0f, t =>
        {
            rect.position = Mathf.Pow(1 - t, 2) * startPos
                            + 2 * (1 - t) * t * controlPoint
                            + Mathf.Pow(t, 2) * endPos;
        }, 1f, duration).SetEase(Ease.OutCubic));
    }

    private Color GetColorForElement(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return colorFire;
            case ElementType.Wind: return colorWind;
            case ElementType.Lightning: return colorLightning;
            default: return colorNone;
        }
    }
}
