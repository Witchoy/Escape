// Attach to: Player GameObject
using System;
using UnityEngine;

public class HotBarController : MonoBehaviour
{
    public static HotBarController Instance { get; private set; }

    public event Action OnEquippedChanged;

    public int EquippedIndex { get; private set; }

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform cameraTransform;

    private HandEquipper _handEquipper;

    public bool IsInputBlocked => playerController != null &&
        playerController.State is PlayerState.ININVENTORY or PlayerState.INSPECTING;

    private void Awake()
    {
        Instance = this;
        _handEquipper = GetComponent<HandEquipper>();
    }

    public void Select(int index)
    {
        EquippedIndex = index;
        OnEquippedChanged?.Invoke();
    }

    public void DropEquipped()
    {
        if (IsInputBlocked) return;

        var slots = InventoryManager.Instance.HotBarSlots;
        var equippedSlot = slots[EquippedIndex];
        if (!equippedSlot.HasItem()) return;

        var itemSo = equippedSlot.GetItem();
        if (!itemSo.itemPrefab) return;

        var dropped = Instantiate(
            itemSo.itemPrefab,
            cameraTransform.position + cameraTransform.forward,
            cameraTransform.rotation
        );

        var item = dropped.GetComponent<Item>();
        item.item = itemSo;
        item.amount = equippedSlot.GetAmount();

        InventoryManager.Instance.RemoveItem(equippedSlot);
        OnEquippedChanged?.Invoke();
    }

    public void ConsumeEquippedItem(int amount = 1)
    {
        var slot = InventoryManager.Instance.HotBarSlots[EquippedIndex];
        InventoryManager.Instance.ConsumeItem(slot, amount);
        OnEquippedChanged?.Invoke();
    }

    public IUsable GetEquippedUsable() => _handEquipper.GetEquippedUsable();
    public Transform GetEquippedItemTransform() => _handEquipper.GetEquippedItemTransform();
}
