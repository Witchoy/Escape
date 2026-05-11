using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering;

    [Header("References")] [SerializeField]
    private Image iconImage;

    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Inventory")] [SerializeField]
    private bool isHotbar;

    private ItemSo _heldItem;
    private int _itemAmount;

    private void Awake()
    {
        ClearSlot();
        InventoryManager.Instance.RegisterSlot(this, isHotbar);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public ItemSo GetItem()
    {
        return _heldItem;
    }

    public int GetAmount()
    {
        return _itemAmount;
    }

    public void SetItem(ItemSo item, int amount = 1)
    {
        _heldItem = item;
        _itemAmount = amount;

        UpdateSlot();
    }

    private void UpdateSlot()
    {
        if (_heldItem != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = _heldItem.itemIcon;
            iconImage.color = Color.white;
            amountText.text = _itemAmount.ToString();
        }
        else
        {
            iconImage.enabled = false;
            amountText.text = "";
        }
    }

    public void ClearSlot()
    {
        _heldItem = null;
        _itemAmount = 0;
        iconImage.enabled = false;
        UpdateSlot();
    }

    public bool HasItem()
    {
        return _heldItem;
    }
}