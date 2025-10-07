using UnityEngine;

[CreateAssetMenu(fileName = "QuestItem", menuName = "Scriptable Objects/QuestItem")]
public class QuestItem : ItemData
{
    // ������������� ����� �������� �������� �������, ��� �������� ����� �������
    [TextArea]
    public string QuestDescription;

    public override IItemInstance CreateInstance(int count = 1)
    {
        return new QuestInstance(this);
    }
}
