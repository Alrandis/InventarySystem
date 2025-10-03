using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    public GameObject Panel => _panel;
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

    }

    public void Hide()
    {
        _panel.SetActive(false);
    }
}
