using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _armorText;
    [SerializeField] private TextMeshProUGUI _damageText;

    public void UpdateStats(int armor, int damage)
    {
        if (_armorText != null) _armorText.text = "Броня: " + armor;
        if (_damageText != null) _damageText.text = "Урон: " + damage;
    }
}
