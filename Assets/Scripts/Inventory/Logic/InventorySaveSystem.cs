using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Система сохранения и загрузки инвентаря
/// </summary>
public class InventorySaveSystem : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemDatabase _database;

    private string _savePath;

    [Serializable]
    private class SavedItem
    {
        public string Id;
        public int StackCount;
    }

    [Serializable]
    private class InventoryData
    {
        public SavedItem[] Items;
    }

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
    }

    /// <summary>
    /// Сохранение текущего инвентаря в JSON
    /// </summary>
    public void Save()
    {
        if (_inventory == null)
        {
            Debug.LogError("[InventorySaveSystem] Inventory reference is missing!");
            return;
        }

        var data = new InventoryData();
        data.Items = new SavedItem[_inventory.Size];

        for (int i = 0; i < _inventory.Size; i++)
        {
            var item = _inventory.Items[i];
            if (item == null)
            {
                data.Items[i] = new SavedItem { Id = null, StackCount = 0 };
                continue;
            }

            int count = 1;
            if (item is IStackable stackable)
                count = stackable.CurrentStack;

            data.Items[i] = new SavedItem
            {
                Id = item.ItemData.Id,
                StackCount = count
            };
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"[InventorySaveSystem] Inventory saved to {_savePath}");
    }

    /// <summary>
    /// Загрузка инвентаря из JSON
    /// </summary>
    public void Load()
    {
        if (!File.Exists(_savePath))
        {
            Debug.Log("[InventorySaveSystem] No save file found — creating empty inventory.");
            return;
        }

        string json = File.ReadAllText(_savePath);
        var data = JsonUtility.FromJson<InventoryData>(json);

        if (data == null || data.Items == null)
        {
            Debug.LogWarning("[InventorySaveSystem] Invalid or empty save data.");
            return;
        }

        for (int i = 0; i < data.Items.Length && i < _inventory.Size; i++)
        {
            var saved = data.Items[i];

            // Если слот пустой
            if (string.IsNullOrEmpty(saved.Id))
            {
                _inventory.RemoveItemAt(i, int.MaxValue);
                continue;
            }

            var itemData = _database.GetItemById(saved.Id);
            if (itemData == null)
            {
                Debug.LogWarning($"[InventorySaveSystem] Unknown item ID: {saved.Id}");
                continue;
            }

            IItemInstance instance;

            // Проверяем, стекуемый ли предмет
            if (itemData.IsStackable)
            {
                // Используем корректный тип стека
                instance = itemData.CreateInstance(saved.StackCount);                
            }
            else
            {
                // Не-стекуемый предмет
                instance = itemData.CreateInstance();
            }

            //// Удаляем старый предмет из слота
            //_inventory.RemoveItemAt(i, int.MaxValue);

            //// Добавляем предмет напрямую в слот
            //_inventory.AddItem(instance);

            _inventory.SetItemAt(i, instance);
        }

        Debug.Log("[InventorySaveSystem] Inventory loaded successfully.");
    }
}
