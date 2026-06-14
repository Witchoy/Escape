using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    private const float EquippedSlotOpacity = 1f;

    [Header("UI References")] 
    [SerializeField] private GameObject hotBarObject;

    [Header("Player References")] 
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerHandTransform;
    [SerializeField] private PlayerInteraction playerInteraction;
    
    [Header("Hotbar Input Actions")] 
    [SerializeField] private InputActionReference[] hotbarActions;

    private readonly List<Slot> _hotbarSlots = new();
    private GameObject _currentlyHeldItem;
    private Action<InputAction.CallbackContext>[] _hotbarCallbacks;

    private int _selectedHotbarIndex;

    private void Awake()
    {
        _hotbarSlots.AddRange(hotBarObject.GetComponentsInChildren<Slot>());

        playerInteraction.OnGrabPressed += HandlePickUp;
        playerInteraction.OnDropPressed += HandleDropItem;
        playerInteraction.OnUsePressed += HandleUseItem;
        playerInteraction.OnInspectPressed += HandleInspectItem;
        
        _hotbarCallbacks = new Action<InputAction.CallbackContext>[hotbarActions.Length];
        for (var i = 0; i < hotbarActions.Length; i++)
        {
            var index = i;
            _hotbarCallbacks[i] = _ => HandleHotbarKeySelection(index);
        }

        UpdateHotbarOpacity();
    }

    private void OnEnable()
    {
        for (var i = 0; i < hotbarActions.Length; i++)
            hotbarActions[i].action.performed += _hotbarCallbacks[i];
    }

    private void OnDisable()
    {
        for (var i = 0; i < hotbarActions.Length; i++)
            hotbarActions[i].action.performed -= _hotbarCallbacks[i];
    }

    private void AddItem(ItemSo itemToAdd, int amount)
    {
        var remaining = amount;

        foreach (var slot in _hotbarSlots)
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                var currentAmount = slot.GetItemAmount();
                var maxStack = itemToAdd.itemMaxStack;

                if (currentAmount < maxStack)
                {
                    var spaceLeft = maxStack - currentAmount;
                    var amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0) return;
                }
            }

        foreach (var slot in _hotbarSlots)
            if (!slot.HasItem())
            {
                var amountToPlace = Mathf.Min(itemToAdd.itemMaxStack, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0) return;
            }

        if (remaining > 0) Debug.Log("Inventory Full");
    }

    private void HandlePickUp(Item item)
    {
        if (item.TryGetComponent(out PlaySFX sfx)) sfx.Play();
        AddItem(item.item, item.amount);
        Destroy(item.gameObject);
        EquipHandItem();
        UpdateHotbarOpacity();
    }

    private void HandleDropItem()
    {
        var equippedSlot = _hotbarSlots[_selectedHotbarIndex];

        if (!equippedSlot.HasItem()) return;

        var itemSo = equippedSlot.GetItem();
        var prefab = itemSo.itemPrefab;

        if (prefab == null) return;

        var dropped = Instantiate(
            prefab,
            playerCameraTransform.position + playerCameraTransform.forward,
            Quaternion.identity
        );

        var item = dropped.GetComponent<Item>();
        if (item == null) item = dropped.AddComponent<Item>();

        item.item = itemSo;
        item.amount = equippedSlot.GetItemAmount();

        equippedSlot.ClearSlot();
        EquipHandItem();
        UpdateHotbarOpacity();
    }

    private void UpdateHotbarOpacity()
    {
        for (var i = 0; i < _hotbarSlots.Count; i++)
        {
            var iconImage = _hotbarSlots[i].GetIconImage();
            if (iconImage != null)
                iconImage.color = i == _selectedHotbarIndex
                    ? new Color(1, 1, 1, EquippedSlotOpacity)
                    : Color.white;
        }
    }

    private void HandleHotbarKeySelection(int index)
    {
        _selectedHotbarIndex = index;
        UpdateHotbarOpacity();
        EquipHandItem();
    }

    private void HandleUseItem(RaycastHit hit)
    {
        if (!_currentlyHeldItem) return;
        var usable = _currentlyHeldItem.GetComponent<IUsable>();
        if (usable == null) return;
        if (!usable.Use(hit)) return;

        var slot = _hotbarSlots[_selectedHotbarIndex];
        var newAmount = slot.GetItemAmount() - 1;
        if (newAmount <= 0)
            slot.ClearSlot();
        else
            slot.SetItem(slot.GetItem(), newAmount);

        EquipHandItem();
        UpdateHotbarOpacity();
    }

    public event Action<Transform> OnInspectItem;

    private void HandleInspectItem()
    {
        Debug.Log("Inspect Item");
        if (_currentlyHeldItem == null) return;
        OnInspectItem?.Invoke(_currentlyHeldItem.transform);
    }

    private void EquipHandItem()
    {
        if (_currentlyHeldItem != null) Destroy(_currentlyHeldItem.gameObject);

        var equippedSlot = _hotbarSlots[_selectedHotbarIndex];
        if (!equippedSlot.HasItem()) return;

        var item = equippedSlot.GetItem();
        if (item.itemHandPrefab == null) return;

        _currentlyHeldItem = Instantiate(item.itemHandPrefab, playerHandTransform);
        _currentlyHeldItem.transform.localPosition = Vector3.zero;
        _currentlyHeldItem.transform.localRotation = Quaternion.identity;
    }
}