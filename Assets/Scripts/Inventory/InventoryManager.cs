// Attach to: InventoryManager GameObject
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    private readonly List<Slot> _allSlots = new();
    private readonly List<Slot> _hotBarSlots = new();
    private readonly List<Slot> _inventorySlots = new();

    public IReadOnlyList<Slot> AllSlots => _allSlots;
    public IReadOnlyList<Slot> HotBarSlots => _hotBarSlots;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterSlot(Slot slot, bool isHotbar)
    {
        _allSlots.Add(slot);
        if (isHotbar)
        {
            _hotBarSlots.Add(slot);
            _hotBarSlots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        }
        else
        {
            _inventorySlots.Add(slot);
            _inventorySlots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        }
    }

    public void AddItem(ItemSo item, int amount)
    {
        var remaining = amount;
        var priorityOrder = new[] { _hotBarSlots, _inventorySlots };

        foreach (var list in priorityOrder)
        {
            foreach (var slot in list)
            {
                if (!slot.HasItem() || slot.GetItem() != item) continue;
                var current = slot.GetAmount();
                var space = item.maxStack - current;
                if (space <= 0) continue;

                var toAdd = Mathf.Min(space, remaining);
                slot.SetItem(item, current + toAdd);
                remaining -= toAdd;
                if (remaining <= 0) { OnInventoryChanged?.Invoke(); return; }
            }
        }

        foreach (var list in priorityOrder)
        {
            foreach (var slot in list)
            {
                if (slot.HasItem()) continue;
                var toPlace = Mathf.Min(item.maxStack, remaining);
                slot.SetItem(item, toPlace);
                remaining -= toPlace;
                if (remaining <= 0) { OnInventoryChanged?.Invoke(); return; }
            }
        }

        if (remaining > 0)
            Debug.Log($"Inventory full — could not add {remaining} of {item.itemName}");

        OnInventoryChanged?.Invoke();
    }

    public void ConsumeItem(Slot slot, int amount = 1)
    {
        if (!slot.HasItem()) return;

        var newAmount = slot.GetAmount() - amount;
        if (newAmount <= 0)
            slot.ClearSlot();
        else
            slot.SetItem(slot.GetItem(), newAmount);

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(Slot slot)
    {
        if (!slot.HasItem()) return;
        slot.ClearSlot();
        OnInventoryChanged?.Invoke();
    }

    public void MoveItem(Slot from, Slot to)
    {
        if (from == to) return;

        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            var space = to.GetItem().maxStack - to.GetAmount();
            if (space <= 0) return;

            var move = Mathf.Min(space, from.GetAmount());
            to.SetItem(to.GetItem(), to.GetAmount() + move);
            from.SetItem(from.GetItem(), from.GetAmount() - move);
            if (from.GetAmount() <= 0) from.ClearSlot();
        }
        else if (to.HasItem())
        {
            var tempItem = to.GetItem();
            var tempAmount = to.GetAmount();
            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
        }
        else
        {
            to.SetItem(from.GetItem(), from.GetAmount());
            from.ClearSlot();
        }

        OnInventoryChanged?.Invoke();
    }
}
