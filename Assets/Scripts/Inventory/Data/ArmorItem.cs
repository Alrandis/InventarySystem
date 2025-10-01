using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItem", menuName = "Scriptable Objects/ArmorItem")]
public class ArmorItem : ItemData
{
    public int DefaulDefense;
    public ArmorType ArmorType;
}

public enum ArmorType
{
    Shield,
    Helmet,
    Bracer,
    Boots,
    Torso
}