using UnityEngine;

public class PotionInstance : IItemInstance, IStackable, IUsableItem
{
    private readonly PotionItem _data;
    private int _currentStack;

    public PotionInstance(PotionItem data, int amount = 1)
    {
        _data = data;
        _currentStack = Mathf.Clamp(amount, 1, data.MaxStack);
    }

    // Реализация IItemInstance
    public ItemData ItemData => _data;

    public PotionItem Data => _data;
    public int HealAmount => _data.HealAmount;
    // Реализация IStackable
    public int MaxStack => _data.MaxStack;
    public int CurrentStack => _currentStack;

    public bool CanStackWith(IStackable other)
    {
        return other is PotionInstance potion && potion.Data == _data;
    }

    public void AddToStack(int amount)
    {
        _currentStack = Mathf.Min(_currentStack + amount, MaxStack);
    }

    public void Use()
    {
        if (_currentStack <= 0) return;

        // Применяем эффект зелья
        Debug.Log($"Выпито зелье {_data.Name}, +{HealAmount} HP");

        _currentStack--;

        // Если стек пуст, можно уведомить инвентарь удалить предмет
        // Для UI лучше через событие
    }
}

