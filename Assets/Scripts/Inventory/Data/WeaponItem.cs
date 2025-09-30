using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Scriptable Objects/WeaponItem")]
public class WeaponItem : ItemData
{
    public int DefaultDamage;
    public float DefaultAttackSpeed;
    public bool IsTwoHanded;
}
