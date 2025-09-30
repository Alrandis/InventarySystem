using UnityEngine;
[System.Serializable]
public class ArmorInstance
{
    public ArmorItem ArmorType;

    public int Defence;

    public ArmorInstance(ArmorItem armor, int defaulDefense)
    {
        ArmorType = armor;
        Defence = defaulDefense;
    }
}
