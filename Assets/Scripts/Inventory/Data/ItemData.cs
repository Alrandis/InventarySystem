using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public ItemType ItemType;
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Quest
}
