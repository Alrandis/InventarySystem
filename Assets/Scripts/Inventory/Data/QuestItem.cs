using UnityEngine;

[CreateAssetMenu(fileName = "QuestItem", menuName = "Scriptable Objects/QuestItem")]
public class QuestItem : ItemData
{
    // ������������� ����� �������� �������� �������, ��� �������� ����� �������
    [TextArea]
    public string QuestDescription;
}
