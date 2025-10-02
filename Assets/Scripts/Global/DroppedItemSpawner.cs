using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
    static int amountToRemove = 0;
    // Выбрасываем предмет (включая весь стек, если стековый)
    public static void DropItem(Inventory inventory, int slotIndex)
    {
        if (inventory == null || slotIndex < 0 || slotIndex >= inventory.Size) return;

        var item = inventory.Items[slotIndex];
        if (item == null) return;

        // Удаляем из инвентаря
        amountToRemove = (item is IStackable stack) ? stack.CurrentStack : 1;
        Debug.Log($"Размер удаляемого стека {amountToRemove}");
        inventory.RemoveItemAt(slotIndex, amountToRemove);

        // Спавним в мире
        SpawnDroppedItem(item, GetSpawnPosition(), Quaternion.identity);
    }

    private static Vector3 GetSpawnPosition()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        return player.position + player.forward * 1f; // чуть вперед
    }

    // Спавн предмета под игроком или немного перед ним
    private static void SpawnDroppedItem(IItemInstance itemInstance, Vector3 position, Quaternion rotation)
    {
        if (itemInstance == null || itemInstance.ItemData == null) return;

        GameObject prefab = itemInstance.ItemData.Prefab;
        if (prefab == null) return;

        GameObject dropped = Instantiate(prefab, position, rotation);

        // Получаем PickupItem и помечаем, что он не персистентный
        PickupItem pickup = dropped.GetComponent<PickupItem>();
        if (pickup != null)
        {
            if (itemInstance is IStackable)
            {
                // сохраняем количество в префабе
                pickup.SetDroppedStack(amountToRemove);
            }
            else
            {
                // для нестековых — просто пометим как выброшенное
                pickup.SetDropped();
            }
        }
    }
}
