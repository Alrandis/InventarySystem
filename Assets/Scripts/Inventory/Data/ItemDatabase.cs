using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] AllItems;

    public ItemData GetItemById(string id)
    {
        foreach (var item in AllItems)
        {
            if (item.Id == id)
                return item;
        }

        Debug.LogWarning($"[ItemDatabase] Item with id '{id}' not found!");
        return null;
    }
}
