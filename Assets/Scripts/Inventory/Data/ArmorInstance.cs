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
            { "������ ", Defense.ToString() },
            { "��� ����� ", GetType(ArmorItem.ArmorType) }
        };
        return stats;
    }

    private string GetType(ArmorType type)
    {
        switch (type)
        {
            case ArmorType.Shield:
                return "���";
            case ArmorType.Helmet:
                return "����";
            case ArmorType.Bracer:
                return "��������";
            case ArmorType.Boots:
                return "������";
            case ArmorType.Torso:
                return "���������";
        }

        return "��� ����� �� ������";
    }
}
