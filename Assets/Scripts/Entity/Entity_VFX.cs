using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    private Entity entity;
    private AfterImageEffect afterImageEffect;
    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;


    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Element Colors")]
    [SerializeField] private Color accVfx = Color.green;


    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;

    [Header ("Special effect vfx")]
    [SerializeField] private GameObject windVfxPrefab;
    private Color originalHitVfxColor;


  

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        entity = GetComponent<Entity>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
        afterImageEffect = GetComponentInChildren<AfterImageEffect>();
    }

    public void CreateOnHitVFX (Transform target ,bool isCrit ,ElementType element)
    {
        GameObject hitPrefab= isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position , Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element);
        if (entity.facingDir == -1) vfx.transform.Rotate(0, 180, 0);
    }
    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Wind:
                return accVfx;
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Lightning:
                return shockVfx;

            default:
                return Color.white;
        }
    }
    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
        }


        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }


    public IEnumerator OnDamageVfxCo()
    {

        sr.material = onDamageMaterial;
     
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
      
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }
    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Wind)
        {
            StartCoroutine(PlayAccStatusVfxCo(duration));
            afterImageEffect.ApplyAfterImage(duration);
        }
        if (element == ElementType.Fire)
        {
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
        }

        if (element == ElementType.Lightning)
        {
            StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
        }


    }
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;

        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * .8f;
        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;

        }
        sr.color = Color.white;
    }
    private IEnumerator PlayAccStatusVfxCo(float duration)
    {
        GameObject vfx = Instantiate(windVfxPrefab, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(duration);
        Destroy(vfx);
    }
}
