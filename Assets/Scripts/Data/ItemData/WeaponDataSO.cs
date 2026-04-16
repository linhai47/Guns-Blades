using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Weapon item", fileName = "Weapon data - ")]
public class WeaponDataSO : EquipmentDataSO
{
    [Header("Weapon Specific")]
    public GameObject weaponPrefab;
    public float baseDamage;
    public float baseAttackCooldown;

    // --- 补上下面这两个字段 ---
    [Header("Bullet Settings")]
    public float bulletSpeed;         // 子弹飞行速度
    public DamageScaleData damageScaleData; // 伤害缩放数据（用于你的 GetAttackData 方法）

    [Header("Detection")]
    public float hitboxRadius;
    public ElementType weaponElement;
}