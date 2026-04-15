using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.VFX;

public class AfterImageEffect : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;

    [SerializeField] private float spawnInterval = .1f;

    [SerializeField] private float fadeDuration = .5f;

    private Coroutine afterImageCo;


    private SpriteRenderer entityRender;


    private void Awake()
    {
      
        entityRender = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplyAfterImage(float duration)
    {
        if (afterImageCo != null)
        {
            StopCoroutine(afterImageCo);
        }
        
        StartCoroutine(AfterImageCo(duration));

    }

    private IEnumerator AfterImageCo( float duration)
    {

        float totaltime = 0;

        while(totaltime < duration)
        {
            SpawnAfterImage();
            yield return new WaitForSeconds (spawnInterval);
            totaltime += spawnInterval;

        }
        afterImageCo = null;
    }

    private void SpawnAfterImage()
    {
        GameObject afterImage = Instantiate(afterImagePrefab ,transform.position ,transform.rotation );

        SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
        sr.sprite = entityRender.sprite;

        sr.flipX = entityRender.flipX;
        sr.color = new Color(0.8f, 1f, 0.9f, 0.6f); // 带点风属性青绿色

        Destroy(afterImage, fadeDuration);
        afterImage.AddComponent<FadeOut>().StartFade(fadeDuration);
    }




}

public class FadeOut : MonoBehaviour
{
    private Coroutine fadeOutCo;
    private SpriteRenderer sr;
    public void StartFade(float duration)
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Fade(duration));
    }

    private IEnumerator Fade(float duration)
    {
        float elapsed = 0f;
        Color c = sr.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(c.a, 0, elapsed / duration);
            sr.color = new Color(c.r, c.g, c.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}