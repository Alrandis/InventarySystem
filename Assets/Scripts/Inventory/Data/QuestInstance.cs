using UnityEngine;

[System.Serializable]
public class QuestInstance : IItemInstance
{
    private readonly QuestItem _data;

    public QuestInstance(QuestItem data)
    {
        _data = data;
    }

    public QuestItem Data => _data;
    public ItemData ItemData => _data;
}
