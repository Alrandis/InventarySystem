using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private bool _isPersistent = false;
    [SerializeField] private int _stackAmount = 1; // сколько в этом объекте при выбросе
    public int StackAmount => Mathf.Max(1, _stackAmount);
    public ItemData ItemData => _itemData;
    public bool IsPersistent => _isPersistent;

    // Метод подбора — вызывается снаружи (например, из Raycast на камере)
    public void Pickup(Inventory inventory)
    {
        if (_itemData == null || inventory == null) return;

        IItemInstance instance = CreateInstance();
        inventory.AddItem(instance);

        if (!_isPersistent)
        {
            Destroy(gameObject);
        }
    }

    public void SetDroppedStack(int amount)
    {
        _isPersistent = false;
        _stackAmount = Mathf.Max(1, amount);
        Debug.Log($"Размер стека {_stackAmount}");
    }


    public void SetDropped()
    {
        _isPersistent = false;
    }

    private IItemInstance CreateInstance()
    {
        return _itemData switch
        {
            PotionItem potion => new PotionInstance(potion, _stackAmount),

            QuestItem quest => new QuestInstance(quest),

            WeaponItem weapon => new WeaponInstance(
                weapon,
                Random.Range(weapon.DamageMin, weapon.DamageMax),
                weapon.DefaultAttackSpeed
            ),

            ArmorItem armor => new ArmorInstance(
                armor,
                Random.Range(armor.DefenseMin, armor.DefenseMax)
            ),

            _ => null
        };
    }
}
