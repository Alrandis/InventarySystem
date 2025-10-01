using UnityEngine;

[System.Serializable]
public class WeaponInstance : IItemInstance
{
    public WeaponItem WeaponType;

    public int Damage;
    public float AttackSpeed;

    public ItemData ItemData => WeaponType;

    public WeaponInstance(WeaponItem weapon, int startingDamage, float startingAttackSpeed) 
    {
        WeaponType = weapon;
        Damage = startingDamage;
        AttackSpeed = startingAttackSpeed;
    }
}
