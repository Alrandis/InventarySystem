using UnityEngine;
[System.Serializable]
public class ArmorInstance : IItemInstance
{
    public ArmorItem ArmorType;

    public int Defense;

    public ItemData ItemData => ArmorType;

    public ArmorInstance(ArmorItem armor, int defaulDefense)
    {
        ArmorType = armor;
        Defense = defaulDefense;
    }
}
