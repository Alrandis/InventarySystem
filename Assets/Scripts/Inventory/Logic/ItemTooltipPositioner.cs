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
    /// ��������� ������� ������� �� �������� ����������� ����
    /// � ������� ���, ���� ������� �� �������.
    /// </summary>
    public void UpdatePosition(Vector2 screenPosition)
    {
        if (panelRect == null || canvas == null) return;

        // ��������� ��������
        Vector2 pos = screenPosition + offset;
        // �������� ������� Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        // ������ �������
        Vector2 tooltipSize = panelRect.sizeDelta;

        // ��������� �� ������ � ������� �������
        if (pos.x + tooltipSize.x > canvasSize.x)
        {
            pos.x = canvasSize.x - tooltipSize.x;
        }
        if (pos.y - tooltipSize.y < 0)
        {
            pos.y = tooltipSize.y;
        }

        // ������������� �������
        panelRect.position = pos;
    }
}
