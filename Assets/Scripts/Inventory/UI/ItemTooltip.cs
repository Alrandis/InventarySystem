using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;

    private void Awake()
    {
        _panel.SetActive(false);
    }

    public void Show(string itemTitle, string itemDescription, Vector3 position)
    {
        _panel.SetActive(true);
        _title.text = itemTitle;
        _description.text = itemDescription;

        // ставим у мышки (или чуть смещаем)
        _panel.transform.position = position + new Vector3(10f, -10f, 0f);
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }
}
