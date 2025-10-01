using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// ���������� UI-���������
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;       // ������ �� ������ Inventory
    [SerializeField] private ItemSlotUI[] _itemSlots;    // ��� ����� � �����
    [SerializeField] private Button _sortByNameButton;   // ������ ���������� �� �����
    [SerializeField] private Button _sortByTypeButton;   // ������ ���������� �� ����

    private void OnEnable()
    {
        // ������������� �� ������� ���������
        _inventory.OnItemAdded += OnItemChanged;
        _inventory.OnItemRemoved += OnItemChanged;
        _inventory.OnInventorySorted += OnInventorySorted;

        // ����������� ������ ����������
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

    // ��������� ��� �����
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
