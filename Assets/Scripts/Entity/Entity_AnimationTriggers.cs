using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;
    public bool animationOver;
    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
        animationOver = false;
    }
    public void AnimationEvent_FireWeapon()
    {
        // 获取玩家手里的枪
        Weapon currentWeapon = GetComponentInParent<Player>().currentWeaponInstance;
     
        if (currentWeapon != null)
        {
           
            // 调用枪的攻击方法
            currentWeapon.ExecuteAttack();
        }
    }
    private void CurrentStateTrigger()
    {
        entity.CurrentStatteAnimationTrigger();
    }

    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }
}
