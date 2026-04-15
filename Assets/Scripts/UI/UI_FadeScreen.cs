using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeScreen : MonoBehaviour
{
    public static UI_FadeScreen instance; // 单例

    public Image fadeImage;
    public Coroutine fadeEffectCo;

    private void Awake()
    {
        // 单例检查
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
      

        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("UI_FadeScreen requires an Image component.");
        }
    }

  
    #region Public Methods
    public void DoFadeIn(float duration = 1f) => StartCoroutine(FadeInCoroutine(duration));
    public void DoFadeOut(float duration = 1f) => StartCoroutine(FadeOutCoroutine(duration));

    public IEnumerator FadeInCoroutine(float duration = 1f)
    {
        Debug.Log("Start Fade In");
        if (fadeImage == null)
        {
            Debug.LogError("[Fade] fadeImage IS NULL -> abort");
            yield break;
        }
        else
        {
            Debug.Log($"[Fade] fadeImage ok, gameObject.activeInHierarchy={fadeImage.gameObject.activeInHierarchy}, this.enabled={this.enabled}");
        }
        fadeImage.color = new Color(0f, 0f, 0f, 1f); // 从黑色开始

        //if (fadeEffectCo != null) { StopCoroutine(fadeEffectCo); fadeEffectCo = null; }
        //fadeEffectCo = StartCoroutine(FadeEffectCo(0f, Mathf.Max(0.0001f, duration)));
        //yield return fadeEffectCo;
        //fadeEffectCo = null;

        fadeEffectCo = StartCoroutine(FadeEffectCo(0f, Mathf.Max(0.0001f, duration)));
        yield return StartCoroutine(FadeEffectCo_Wrapper()); // wrapper 负责等待并保证清理

        //Debug.Log("[Fade] Fade In coroutine finished");
    }
    private IEnumerator FadeEffectCo_Wrapper()
    {
        // 等待实际正在运行的 FadeEffectCo 结束 (通过读取 fadeEffectCo 是否为 null)
        // 注意：我们会在 FadeEffectCo 的末尾将 fadeEffectCo 置 null
        while (fadeEffectCo != null)
            yield return null;
    }

    public IEnumerator FadeOutCoroutine(float duration = 1f)
    {
        if (fadeImage == null) yield break;

        fadeImage.color = new Color(0f, 0f, 0f, 0f); // 从透明开始
        if (fadeEffectCo != null) { StopCoroutine(fadeEffectCo); fadeEffectCo = null; }
        fadeEffectCo = StartCoroutine(FadeEffectCo(1f, Mathf.Max(0.0001f, duration)));
        yield return fadeEffectCo;
        fadeEffectCo = null;
    }
    #endregion

    #region Internal Coroutine
    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;
        Debug.Log(startAlpha + "StartAlpha" + targetAlpha + "TargetAlpha");
        while (time < duration)
        {
            //Debug.Log(time + "now duration : " + duration);
            time += Time.deltaTime;
            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            //Debug.Log($"FadeIn alpha: {color.a}");
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);

        fadeEffectCo = null;
        Debug.Log("[Fade] FadeEffectCo complete and cleared");
    }
    #endregion
}
