// Attach to: ItemDescription panel (child of Inventory Canvas)
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionUi : MonoBehaviour
{
    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private GameObject itemDescriptionParent;
    [SerializeField] private Image itemDescriptionImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    private void OnEnable()
    {
        inventoryUi.OnHoveredSlotChanged += OnHoveredSlotChanged;
    }

    private void OnDisable()
    {
        inventoryUi.OnHoveredSlotChanged -= OnHoveredSlotChanged;
    }

    private void OnHoveredSlotChanged(Slot slot)
    {
        if (slot == null || !slot.HasItem())
        {
            itemDescriptionParent.SetActive(false);
            return;
        }

        var item = slot.GetItem();
        itemDescriptionParent.SetActive(true);
        itemDescriptionImage.sprite = item.itemIcon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;
    }
}
