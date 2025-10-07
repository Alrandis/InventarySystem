using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EquipSaveSystem : MonoBehaviour
{
    [SerializeField] private EquipManager _equipManager;
    [SerializeField] private ItemDatabase _database;

    private string _savePath;

    [Serializable]
    private class SavedEquipItem
    {
        public EquipSlotType SlotType;
        public string ItemId;
        public int CustomDamage = -1;
        public int CustomDefense = -1;
    }

    [Serializable]
    private class EquipData
    {
        public List<SavedEquipItem> Items = new();
    }

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "equip.json");
    }

    private void Start()
    {
        if (_equipManager != null)
            _equipManager.OnEquipmentChanged += HandleEquipmentChanged;
    }

    private void OnDestroy()
    {
        if (_equipManager != null)
            _equipManager.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged()
    {
        Save();
    }

    public void Save()
    {
        var data = new EquipData();

        foreach (var slot in _equipManager.Slots)
        {
            if (slot.CurrentItem == null || slot.IsDuplicate)
                continue;

            var saved = new SavedEquipItem
            {
                SlotType = slot.SlotType,
                ItemId = slot.CurrentItem.ItemData.Id,
                CustomDamage = -1,
                CustomDefense = -1
            };

            if (slot.CurrentItem is WeaponInstance weapon)
                saved.CustomDamage = weapon.Damage;
            else if (slot.CurrentItem is ArmorInstance armor)
                saved.CustomDefense = armor.Defense;

            data.Items.Add(saved);
        }

        File.WriteAllText(_savePath, JsonUtility.ToJson(data, true));
        Debug.Log($"[EquipSaveSystem] Equipment saved to {_savePath}");
    }

    public void Load()
    {
        if (!File.Exists(_savePath))
        {
            Debug.Log("[EquipSaveSystem] No saved equipment found.");
            return;
        }

        string json = File.ReadAllText(_savePath);
        var data = JsonUtility.FromJson<EquipData>(json);
        if (data == null || data.Items == null)
        {
            Debug.LogWarning("[EquipSaveSystem] Invalid or empty equipment data.");
            return;
        }

        foreach (var saved in data.Items)
        {
            var itemData = _database.GetItemById(saved.ItemId);
            if (itemData == null)
            {
                Debug.LogWarning($"[EquipSaveSystem] Unknown item id: {saved.ItemId}");
                continue;
            }

            IItemInstance instance;

            if (itemData is WeaponItem weaponData)
            {
                int dmg = saved.CustomDamage != -1 ? saved.CustomDamage : weaponData.DefaultDamage;
                instance = new WeaponInstance(weaponData, dmg, weaponData.DefaultAttackSpeed);
            }
            else if (itemData is ArmorItem armorData)
            {
                int def = saved.CustomDefense != -1 ? saved.CustomDefense : armorData.DefaulDefense;
                instance = new ArmorInstance(armorData, def);
            }
            else
            {
                instance = itemData.CreateInstance();
            }

            _equipManager.ForceEquip(saved.SlotType, instance);
        }
        //_equipManager.OnEquipmentChanged?.Invoke();

        Debug.Log("[EquipSaveSystem] Equipment loaded successfully.");
    }
}
