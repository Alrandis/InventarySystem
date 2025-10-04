using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItem", menuName = "Scriptable Objects/ArmorItem")]
public class ArmorItem : ItemData
{
    public int DefaulDefense;
    public ArmorType ArmorType;

    public int DefenseMin;
    public int DefenseMax;

    public override IItemInstance CreateInstance(int count = 1)
    {
        return new ArmorInstance(this, DefaulDefense);
    }
}

public enum ArmorType
{
    Shield,
    Helmet,
    Bracer,
    Boots,
    Torso
}