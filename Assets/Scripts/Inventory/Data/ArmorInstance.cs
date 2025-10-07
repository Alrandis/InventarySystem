using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class ArmorInstance : IItemInstance
{
    public ArmorItem ArmorItem;

    public int Defense;

    public ItemData ItemData => ArmorItem;

    public ArmorInstance(ArmorItem armor, int defaulDefense)
    {
        ArmorItem = armor;
        Defense = defaulDefense;
    }

    public Dictionary<string, string> GetStats()
    {
        var stats = new Dictionary<string, string>
        {
            { "Защита ", Defense.ToString() },
            { "Тип брони ", GetType(ArmorItem.ArmorType) }
        };
        return stats;
    }

    private string GetType(ArmorType type)
    {
        switch (type)
        {
            case ArmorType.Shield:
                return "щит";
            case ArmorType.Helmet:
                return "шлем";
            case ArmorType.Bracer:
                return "перчатки";
            case ArmorType.Boots:
                return "сапоги";
            case ArmorType.Torso:
                return "нагрудник";
        }

        return "Тип брони не указан";
    }
}
