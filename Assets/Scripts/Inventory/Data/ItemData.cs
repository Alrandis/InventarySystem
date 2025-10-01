using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public GameObject PreviewPrefab;
    public ItemType ItemType;
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Quest
}
