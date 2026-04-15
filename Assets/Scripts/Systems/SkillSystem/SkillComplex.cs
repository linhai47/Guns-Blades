using UnityEngine;

public class SkillComplex : SkillBase
{

    [SerializeField] protected LayerMask whatIsEnemy;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Entity_Stats playerStats;
    protected ElementType usedElement;
    protected bool targetGotHit;
    protected Transform lastTarget;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected GameObject onHitVfx;


    protected void Single_Damage(Transform t, float radius)
    {
        ApplyDamageToTarget(GetSingleEnemy(t, radius));


    }
    protected Collider2D GetSingleEnemy(Transform t, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);

        Collider2D closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(t.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = hit;
            }
        }

        return closest;



    }
    protected void ApplyDamageToTarget(Collider2D target, float damageScaleNumber = 1f)
    {
        IDamagable damagable = target.GetComponent<IDamagable>();

        AttackData attackData = playerStats.GetAttackData(damageScaleData);

        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();


        float physicsDamage = attackData.physicalDamage * damageScaleNumber;
        float elementalDamage = attackData.elementalDamage * damageScaleNumber;
        ElementType element = attackData.element;
        Entity attackEntity = attackData.attackEntity;
        bool isCrit = attackData.isCrit;
        targetGotHit = damagable.TakeDamage(physicsDamage, elementalDamage, element, transform ,isCrit);


        if (element != ElementType.None)
        {
            statusHandler.ApplyStatusEffect(element, attackData.effectData, attackEntity);
        }
        if (targetGotHit)
        {
            lastTarget = target.transform;
            Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
        }
        usedElement = element;
    }

}
