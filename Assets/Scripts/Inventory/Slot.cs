using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    private ItemSo _heldItem;

    private int _itemAmount;

    public ItemSo GetItem()
    {
        return _heldItem;
    }

    public void SetItem(ItemSo heldItem, int amount)
    {
        _heldItem = heldItem;
        _itemAmount = amount;

        UpdateSlot();
    }

    public int GetItemAmount()
    {
        return _itemAmount;
    }

    private void UpdateSlot()
    {
        if (_heldItem != null)
        {
            iconImage.sprite = _heldItem.itemSprite;
            amountText.text = _itemAmount.ToString();
        }
        else
        {
            iconImage.sprite = null;
            amountText.text = "";
        }
    }

    public void ClearSlot()
    {
        _heldItem = null;
        _itemAmount = 0;

        UpdateSlot();
    }

    public bool HasItem()
    {
        return _heldItem != null;
    }

    public Image GetIconImage()
    {
        return iconImage;
    }
}