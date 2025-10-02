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
    private ItemSlotUI _selectedSlot;

    private void Awake()
    {
        // �������������� ������� ������ (����� ������� ������� � �����)
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].Init(i, this, _inventory);
            //_itemSlots[i].OnDropClicked += HandleDropClicked;
        }
    }


    private void OnEnable()
    {
        if (_inventory != null)
        {
            _inventory.OnItemChanged += OnItemChanged;
            _inventory.OnInventorySorted += OnInventorySorted;
        }

        if (_sortByNameButton != null) 
            _sortByNameButton.onClick.AddListener(() => { _inventory.Sort(InventorySortType.ByName); });
        if (_sortByTypeButton != null) 
            _sortByTypeButton.onClick.AddListener(() => { _inventory.Sort(InventorySortType.ByItemType); });

        RefreshUI();
    }

    private void OnDisable()
    {
        if (_inventory != null)
        {
            _inventory.OnItemChanged -= OnItemChanged;
            _inventory.OnInventorySorted -= OnInventorySorted;
        }

        if (_sortByNameButton != null) 
            _sortByNameButton.onClick.RemoveAllListeners();
        if (_sortByTypeButton != null) 
            _sortByTypeButton.onClick.RemoveAllListeners();
    }

    private void OnItemChanged(int index, IItemInstance item)
    {
        if (index < 0 || index >= _itemSlots.Length) return;

        if (item != null)
            _itemSlots[index].SetItem(item);
        else
        {
            _itemSlots[index].Clear();


            // ���� ���� ��� ������� � ����� ���������
            if (_selectedSlot == _itemSlots[index])
            {
                _selectedSlot.SetSelected(false);
                _selectedSlot = null;
            }
        }
    }

    public void SelectSlot(ItemSlotUI slot)
    {
        if (_selectedSlot != null)
            _selectedSlot.SetSelected(false);

        _selectedSlot = slot;

        if (_selectedSlot != null)
            _selectedSlot.SetSelected(true);
    }

    public void ClearSelectionIfSlot(int index)
    {
        if (_selectedSlot != null && _selectedSlot.Index == index)
        {
            _selectedSlot.SetSelected(false);
            _selectedSlot = null;
        }
    }

    private void OnInventorySorted(InventorySortType sortType)
    {
        // ��� ���������� ����� ������������ ��
        RefreshUI();
    }

    private void RefreshUI()
    {
        int slots = _itemSlots.Length;
        for (int i = 0; i < slots; i++)
        {
            IItemInstance item = (i < _inventory.Size) ? _inventory.Items[i] : null;

            if (item != null) 
                _itemSlots[i].SetItem(item);
            else 
                _itemSlots[i].Clear();
        }
    }

    // Drag
    public void StartDrag(ItemSlotUI slot)
    {
        _draggedFromSlot = slot;
    }

    // SwapSlots: ������ ����������� ����� / swap / move
    public void SwapSlots(ItemSlotUI fromSlot, ItemSlotUI toSlot)
    {
        if (fromSlot == null || toSlot == null) return;
        if (fromSlot == toSlot) return;

        var fromItem = fromSlot.GetItem();
        var toItem = toSlot.GetItem();

        if (fromItem == null) return;

        int fromIndex = fromSlot.Index;
        int toIndex = toSlot.Index;

        // 1) ���� ����� � ����� ���������� � ���������� MergeStacks
        if (fromItem is IStackable && toItem is IStackable)
        {
            var fromStack = fromItem as IStackable;
            var toStack = toItem as IStackable;

            if (fromStack.CanStackWith(toStack))
            {
                _inventory.MergeStacks(fromIndex, toIndex);
                return;
            }
        }

        // 2) ���� ������� ������ - move
        if (toItem == null)
        {
            _inventory.MoveItem(fromIndex, toIndex);
            return;
        }

        // 3) ������� swap
        _inventory.SwapItems(fromIndex, toIndex);
    }

}
