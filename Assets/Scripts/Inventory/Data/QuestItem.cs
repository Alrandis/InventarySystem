using UnityEngine;

[CreateAssetMenu(fileName = "QuestItem", menuName = "Scriptable Objects/QuestItem")]
public class QuestItem : ItemData
{
    // ƒополнительно можно добавить описание задани€, дл€ которого нужен предмет
    [TextArea]
    public string QuestDescription;
}
