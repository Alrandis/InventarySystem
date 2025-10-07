using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string Id; // ���������� ������������� ��� ���������� � Json
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public ItemType ItemType;
    public bool IsStackable = false;

    // ����� ��� �������� �������� ��������
    public abstract IItemInstance CreateInstance(int count = 1);
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Quest
}
