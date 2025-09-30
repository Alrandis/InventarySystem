using UnityEngine;

[System.Serializable]
public class WeaponInstance 
{
    public WeaponItem WeaponType;

    public int Damage;
    public float AttackSpeed;

    public WeaponInstance(WeaponItem weapon, int startingDamage, float startingAttackSpeed) 
    {
        WeaponType = weapon;
        Damage = startingDamage;
        AttackSpeed = startingAttackSpeed;
    }
}
