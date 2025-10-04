using UnityEngine;

[RequireComponent(typeof(ItemTooltip))]
public class ItemTooltipPositioner : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(10f, -10f);

    private RectTransform panelRect;
    private Canvas canvas;

    private void Awake()
    {
        ItemTooltip tooltip = GetComponent<ItemTooltip>();
        panelRect = tooltip.Panel.gameObject.GetComponent<RectTransform>();
        canvas = tooltip.Panel.GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Обновляет позицию тултипа по экранным координатам мыши
    /// и смещает его, если выходит за границы.
    /// </summary>
    public void UpdatePosition(Vector2 screenPosition)
    {
        if (panelRect == null || canvas == null) return;

        // Применяем смещение
        Vector2 pos = screenPosition + offset;
        // Получаем границы Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Размер тултипа
        Vector2 tooltipSize = panelRect.sizeDelta;

        // Коррекция по правой и верхней границе
        if (pos.x + tooltipSize.x > canvasSize.x)
        {
            pos.x = canvasSize.x - tooltipSize.x;
        }
        if (pos.y - tooltipSize.y < 0)
        {
            pos.y = tooltipSize.y;
        }

        // Устанавливаем позицию
        panelRect.position = pos;
    }
}
