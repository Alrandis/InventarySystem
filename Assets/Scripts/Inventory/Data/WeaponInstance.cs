using System.Collections.Generic;
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

    public Dictionary<string, string> GetStats()
    {
        var stats = new Dictionary<string, string>
        {
            { "Урон", Damage.ToString() },
            { "Скорость атаки", AttackSpeed.ToString() }
        };
        return stats;
    }
}
