using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private bool _isPersistent = false;

    public ItemData ItemData => _itemData;
    public bool IsPersistent => _isPersistent;

    // ћетод подбора Ч вызываетс€ снаружи (например, из Raycast на камере)
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

    private IItemInstance CreateInstance()
    {
        return _itemData switch
        {
            PotionItem potion => new PotionInstance(potion),
            WeaponItem weapon => new WeaponInstance(weapon, weapon.DefaultDamage, weapon.DefaultAttackSpeed),
            ArmorItem armor => new ArmorInstance(armor, armor.DefaulDefense),
            _ => null
        };
    }
}
