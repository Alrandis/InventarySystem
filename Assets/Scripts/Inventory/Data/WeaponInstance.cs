using UnityEngine;

[System.Serializable]
public class WeaponInstance 
{
    public WeaponItem WeaponType;

    public int Damage;
    public float AttackSpeed;

    public WeaponInstance(WeaponItem weapon, int StartingDamage, float StartingAttackSpeed) 
    {
        WeaponType = weapon;
        Damage = StartingDamage;
        AttackSpeed = StartingAttackSpeed;
    }
}
