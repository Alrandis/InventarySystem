using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestInstance : IItemInstance
{
    private readonly QuestItem _data;

    public QuestInstance(QuestItem data)
    {
        _data = data;
    }

    public Dictionary<string, string> GetStats()
    {
        var stats = new Dictionary<string, string>
        {
            { "Квестовая информация \n", _data.QuestDescription.ToString() },
        };
        return stats;
    }

    public QuestItem Data => _data;
    public ItemData ItemData => _data;
}
