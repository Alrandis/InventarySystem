using UnityEngine;

[CreateAssetMenu(fileName = "PotionItem", menuName = "Scriptable Objects/PotionItem")]
public class PotionItem : ItemData
{
    public int HealAmount;
    public int MaxStack;

    public override IItemInstance CreateInstance(int count = 1)
    {
        return new PotionInstance(this, count);
    }
}
