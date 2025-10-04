using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string Id; // уникальный идентификатор для сохранения в Json
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public ItemType ItemType;
    public bool IsStackable = false;

    // Метод для создания инстанса предмета
    public abstract IItemInstance CreateInstance(int count = 1);
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Quest
}
