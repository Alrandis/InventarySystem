using System;
using UnityEngine;

public interface IUsableItem
{
    /// <summary>
    /// Логика использования предмета
    /// </summary>
    void Use();
    event Action<IItemInstance> OnUsed; // уведомление, что предмет был использован
}
