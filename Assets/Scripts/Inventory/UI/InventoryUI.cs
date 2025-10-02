using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemSlotUI[] _itemSlots;
    [SerializeField] private Button _sortByNameButton;
    [SerializeField] private Button _sortByTypeButton;

    private ItemSlotUI _draggedFromSlot;

    private void Awake()
    {
        // инициализируем индексы слотов (слоты сделаны вручную в сцене)
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Init(i);
        }
    }

    private void OnEnable()
    {
        if (_inventory != null)
        {
            _inventory.OnItemChanged += Inventory_OnItemChanged;
            _inventory.OnInventorySorted += Inventory_OnInventorySorted;
        }

        if (_sortByNameButton != null) _sortByNameButton.onClick.AddListener(() => { _inventory.Sort(InventorySortType.ByName); });
        if (_sortByTypeButton != null) _sortByTypeButton.onClick.AddListener(() => { _inventory.Sort(InventorySortType.ByItemType); });

        RefreshUI();
    }

    private void OnDisable()
    {
        if (_inventory != null)
        {
            _inventory.OnItemChanged -= Inventory_OnItemChanged;
            _inventory.OnInventorySorted -= Inventory_OnInventorySorted;
        }

        if (_sortByNameButton != null) _sortByNameButton.onClick.RemoveAllListeners();
        if (_sortByTypeButton != null) _sortByTypeButton.onClick.RemoveAllListeners();
    }

    private void Inventory_OnItemChanged(int index, IItemInstance item)
    {
        // обновл€ем только этот слот (если индекс валидный)
        if (index >= 0 && index < _itemSlots.Length)
        {
            if (item != null) _itemSlots[index].SetItem(item);
            else _itemSlots[index].Clear();
        }
    }

    private void Inventory_OnInventorySorted(InventorySortType sortType)
    {
        // при сортировке проще перерисовать всЄ
        RefreshUI();
    }

    private void RefreshUI()
    {
        int slots = _itemSlots.Length;
        for (int i = 0; i < slots; i++)
        {
            IItemInstance item = null;
            if (_inventory != null && i < _inventory.Size)
                item = _inventory.Items[i];

            if (item != null) _itemSlots[i].SetItem(item);
            else _itemSlots[i].Clear();
        }
    }

    // Drag
    public void StartDrag(ItemSlotUI slot)
    {
        _draggedFromSlot = slot;
    }

    // SwapSlots: логика объединени€ стека / swap / move
    public void SwapSlots(ItemSlotUI fromSlot, ItemSlotUI toSlot)
    {
        if (fromSlot == null || toSlot == null) return;
        if (fromSlot == toSlot) return;

        var fromItem = fromSlot.GetItem();
        var toItem = toSlot.GetItem();

        if (fromItem == null) return;

        int fromIndex = fromSlot.Index;
        int toIndex = toSlot.Index;

        // 1) ≈сли стеки и можно объединить:
        if (fromItem is IStackable fs && toItem is IStackable ts && fs.CanStackWith(ts))
        {
            ts.AddToStack(fs.CurrentStack);      // добавл€ем в существующий стак
            // удал€ем исходный стак (он полностью переместилс€ в целевой стак)
            _inventory.RemoveItemAt(fromIndex);
            return;
        }

        // 2) ≈сли целевой пустой - move
        if (toItem == null)
        {
            _inventory.MoveItem(fromIndex, toIndex);
            return;
        }

        // 3) ќбычный swap
        _inventory.SwapItems(fromIndex, toIndex);
    }
}
