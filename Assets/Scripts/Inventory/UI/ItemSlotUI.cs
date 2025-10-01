using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text stackText;
    [SerializeField] private GameObject selectionHighlight;

    private IItemInstance currentItem;

    public void SetItem(IItemInstance item)
    {
        currentItem = item;

        if (item == null)
        {
            itemIcon.gameObject.SetActive(false);
            stackText.text = "";
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = item.ItemData.Icon;

            if (item is IStackable stackable)
            {
                stackText.text = stackable.CurrentStack.ToString();
            }
            else
            {
                stackText.text = "";
            }
        }
    }

    public void Clear()
    {
        SetItem(null);
    }

    public void Select()
    {
        selectionHighlight.SetActive(true);
    }

    public void Deselect()
    {
        selectionHighlight.SetActive(false);
    }

}
