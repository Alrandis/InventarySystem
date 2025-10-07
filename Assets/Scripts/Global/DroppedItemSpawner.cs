using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
    static int amountToRemove = 0;
    // ����������� ������� (������� ���� ����, ���� ��������)
    public static void DropItem(Inventory inventory, int slotIndex)
    {
        if (inventory == null || slotIndex < 0 || slotIndex >= inventory.Size) return;

        var item = inventory.Items[slotIndex];
        if (item == null) return;

        // ������� �� ���������
        amountToRemove = (item is IStackable stack) ? stack.CurrentStack : 1;
        Debug.Log($"������ ���������� ����� {amountToRemove}");
        inventory.RemoveItemAt(slotIndex, amountToRemove);

        // ������� � ����
        SpawnDroppedItem(item, GetSpawnPosition(), Quaternion.identity);
    }

    private static Vector3 GetSpawnPosition()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        return player.position + player.forward * 1f; // ���� ������
    }

    // ����� �������� ��� ������� ��� ������� ����� ���
    private static void SpawnDroppedItem(IItemInstance itemInstance, Vector3 position, Quaternion rotation)
    {
        if (itemInstance == null || itemInstance.ItemData == null) return;

        GameObject prefab = itemInstance.ItemData.Prefab;
        if (prefab == null) return;

        GameObject dropped = Instantiate(prefab, position, rotation);

        // �������� PickupItem � ��������, ��� �� �� �������������
        PickupItem pickup = dropped.GetComponent<PickupItem>();
        if (pickup != null)
        {
            if (itemInstance is IStackable)
            {
                // ��������� ���������� � �������
                pickup.SetDroppedStack(amountToRemove);
            }
            else
            {
                // ��� ���������� � ������ ������� ��� �����������
                pickup.SetDropped();
            }
        }
    }
}
