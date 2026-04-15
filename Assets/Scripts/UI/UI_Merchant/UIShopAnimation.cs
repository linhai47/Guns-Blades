using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UIShopAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform panel;  // 商店面板
    [SerializeField] private float duration = 0.3f; // 动画时长
    [SerializeField] private Vector2 hiddenPos = new Vector2(0, 800); // 初始隐藏位置
    [SerializeField] private Vector2 shownPos = new Vector2(0, 0);    // 显示位置

    private Coroutine currentAnim;

    void Awake()
    {
        panel.anchoredPosition = hiddenPos;
    }

    public void Toggle(bool show)
    {
        panel.DOAnchorPos(show ? shownPos : hiddenPos, duration)
            .SetEase(Ease.InOutSine); // 缓入缓出，可以换成 Ease.InOutSine 等
    }

    private IEnumerator MovePanel(Vector2 target)
    {
        Vector2 start = panel.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            panel.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        panel.anchoredPosition = target;
    }
    public void ResetToHidden()
    {
        // 停止可能正在运行的动画
        panel.DOKill();

        // 直接设置到初始隐藏位置
        panel.anchoredPosition = hiddenPos;
    }

}

