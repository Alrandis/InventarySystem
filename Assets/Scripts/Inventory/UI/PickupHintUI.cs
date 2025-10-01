using TMPro;
using UnityEngine;

public class PickupHintUI : MonoBehaviour
{
    public static PickupHintUI Instance;

    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Hide();
    }

    public void Show(string message)
    {
        _panel.SetActive(true);
        _text.text = message;
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }
}
