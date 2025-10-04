using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Scriptable Objects/WeaponItem")]
public class WeaponItem : ItemData
{
    public int DefaultDamage;
    public float DefaultAttackSpeed;
    public bool IsTwoHanded;

    public override IItemInstance CreateInstance(int count = 1)
    {
        return new WeaponInstance(this, DefaultDamage, DefaultAttackSpeed);
    }
}
