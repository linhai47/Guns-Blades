using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    public override void ExecuteAttack()
    {
        // 1. 检查 CD 并更新时间 (调用基类)
        if (Time.time < lastAttackTime + data.baseAttackCooldown) return;
        base.ExecuteAttack();

        // 2. 发射子弹
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Projectile script = newBullet.GetComponent<Projectile>();

        if (script != null)
        {
            // 直接使用基类保存好的 ownerEntity 传给子弹
            script.SetupProjectile(ownerEntity, data, firePoint.right);
        }
    }
}