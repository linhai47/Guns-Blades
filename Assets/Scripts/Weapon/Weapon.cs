using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponDataSO data;

    // [新增] 武器自己记住它的持有者
    protected Entity ownerEntity;
    protected Entity_Stats ownerStats;
    protected float lastAttackTime;

    // 只有在装备这把武器的那一刻，调用一次
    public virtual void SetupWeapon(WeaponDataSO weaponData, Entity owner)
    {
        this.data = weaponData;
        this.ownerEntity = owner;
        this.ownerStats = owner.GetComponent<Entity_Stats>();
    }

    // 现在的 ExecuteAttack 不需要任何参数了！动画事件可以直接无脑调用它
    public virtual void ExecuteAttack()
    {
        if (Time.time < lastAttackTime + data.baseAttackCooldown) return;
        lastAttackTime = Time.time;

        // 播放武器本身的开火动画（如果有的话）
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null){ 
            anim.SetBool("Attack",true);

            Debug.Log("Attack");
        }
    }
}