using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private ElementType currentEffect = ElementType.None;
    private Entity entity;
    private Entity_VFX entity_VFX;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    [SerializeField] private Player player;
    private bool isSpeedUp=false;
    //private Entity attackerEntity;
    [Header("Shock effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maximumCharge = 1;

    [Header("accelerate")]
    [SerializeField] private float accMultiplier = 1;
    private Coroutine shockCo;
    private Coroutine accCo;
    private void Awake()
    {
        entity = GetComponent<Entity>();
        entity_VFX = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }
    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData ,Entity attackerEntity)
    {


        if (element == ElementType.Wind && CanBeApplied(ElementType.Wind))
        {
            ApplyAccEffect(effectData.accDuration, accMultiplier , attackerEntity);   
        }
        if (element == ElementType.Fire && CanBeApplied(ElementType.Fire))
        {

            ApplyBurnEffect(effectData.burnDuration, effectData.totalBurnDamage);

        }
        if (element == ElementType.Lightning && CanBeApplied(ElementType.Lightning))
        {
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
        }

    }


    private void ApplyShockEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);

        currentCharge = currentCharge + finalCharge;
        if (currentCharge >= maximumCharge)
        {
            DoLightningStrike(damage);
            StopShockEffect();
            return;
        }
        if (shockCo != null)
        {
            StopCoroutine(shockCo);
        }
        shockCo = StartCoroutine(ShockEffectCo(duration));
    }
    private void StopShockEffect()
    {
        currentCharge = 0;
        currentEffect = ElementType.None;
        entity_VFX.StopAllVfx();
    }
    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }
    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entity_VFX.PlayOnStatusVfx(duration, ElementType.Lightning);

        yield return new WaitForSeconds(duration);
        StopShockEffect();
    }



    private void ApplyBurnEffect(float duration , float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage *(1- fireResistance);

        StartCoroutine(BurnEffectCo(duration , finalDamage));

    }
    private IEnumerator BurnEffectCo(float duration , float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entity_VFX.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;

    }


    private void ApplyAccEffect(float duration, float accMultiplier , Entity attackerEntity)
    {
        float windResistance = entityStats.GetElementalResistance(ElementType.Wind);
        float finalDuration = duration * (1 - windResistance);

        if (accCo != null)
        {
            StopCoroutine(accCo);
            
        }
        accCo =  StartCoroutine(AccEffectCo(finalDuration, accMultiplier,attackerEntity));
        

    }
    private IEnumerator AccEffectCo(float duration, float accMultiplier, Entity attackerEntity)
    {

        attackerEntity.SpeedUpEntity(accMultiplier ,duration);

        currentEffect = ElementType.Wind;
     

        attackerEntity.vfx.PlayOnStatusVfx(duration, ElementType.Wind);
        
        yield return new WaitForSeconds(duration);

        
        currentEffect = ElementType.None;
    }

    public void RemoveAllNegativeEffects()
    {
        StopAllCoroutines();
        currentEffect = ElementType.None;
        entity_VFX.StopAllVfx();
    }
    public bool CanBeApplied(ElementType element)
    {

        
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;

        return currentEffect == ElementType.None;
    }

}
