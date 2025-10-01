using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// Управление UI-инвентаря
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;       // Ссылка на скрипт Inventory
    [SerializeField] private ItemSlotUI[] _itemSlots;    // Все слоты в сетке
    [SerializeField] private Button _sortByNameButton;   // Кнопка сортировки по имени
    [SerializeField] private Button _sortByTypeButton;   // Кнопка сортировки по типу

    private void OnEnable()
    {
        // Подписываемся на события инвентаря
        _inventory.OnItemAdded += OnItemChanged;
        _inventory.OnItemRemoved += OnItemChanged;
        _inventory.OnInventorySorted += OnInventorySorted;

        // Подписываем кнопки сортировки
        _sortByNameButton.onClick.AddListener(() => _inventory.Sort(InventorySortType.ByName));
        _sortByTypeButton.onClick.AddListener(() => _inventory.Sort(InventorySortType.ByItemType));

        RefreshUI();
    }

    private void OnDisable()
    {
        _inventory.OnItemAdded -= OnItemChanged;
        _inventory.OnItemRemoved -= OnItemChanged;
        _inventory.OnInventorySorted -= OnInventorySorted;

        _sortByNameButton.onClick.RemoveAllListeners();
        _sortByTypeButton.onClick.RemoveAllListeners();
    }

    private void OnItemChanged(IItemInstance item)
    {
        RefreshUI();
    }

    private void OnInventorySorted(InventorySortType sortType)
    {
        RefreshUI();
    }

    // Обновляем все слоты
    private void RefreshUI()
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (i < _inventory.Items.Count)
            {
                _itemSlots[i].SetItem(_inventory.Items[i]);
            }
            else
            {
                _itemSlots[i].Clear();
            }
        }
    }
}
