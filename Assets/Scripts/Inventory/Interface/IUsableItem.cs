using System;
using UnityEngine;

public interface IUsableItem
{
    /// <summary>
    /// ������ ������������� ��������
    /// </summary>
    void Use();
    event Action<IItemInstance> OnUsed; // �����������, ��� ������� ��� �����������
}
