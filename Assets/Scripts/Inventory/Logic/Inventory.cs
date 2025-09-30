using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Храним интерфейс, чтобы можно было положить и оружие, и броню, и расходники
    private List<IItemInstance> _items = new();
    // Указываем критерий сортировки по алфавиту или по типу
    private InventorySortType _currentSort = InventorySortType.None;
    // Ограничиваем доступ из вне, чтобы в случае чего ни кто не мог изменить инвентарь, кроме него самого
    public IReadOnlyList<IItemInstance> Items => _items;

    public void AddItem(IItemInstance itemToAdd)
    {
        _items.Add(itemToAdd);
        Debug.Log($"Добавлен предмет: {itemToAdd.ItemData.Name}");
    }

    public void RemoveItem(IItemInstance itemToRemove)
    {
        _items.Remove(itemToRemove);
        Debug.Log($"Удален предмет: {itemToRemove.ItemData.Name}");
    }

    public bool Contains(IItemInstance item)
    {
        return _items.Contains(item);
    }

    public void Sort(InventorySortType sortType)
    {
        _currentSort = sortType;

        switch (sortType)
        {
            case InventorySortType.ByName:
                _items.Sort((a, b) => string.Compare(a.ItemData.Name, b.ItemData.Name));
                break;

            case InventorySortType.ByItemType:
                _items.Sort((a, b) => a.ItemData.ItemType.CompareTo(b.ItemData.ItemType));
                break;

            case InventorySortType.None:
            default:
                break; 
        }
    }
}

public enum InventorySortType
{
    None,
    ByName,
    ByItemType
}
