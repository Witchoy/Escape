using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private const float EquippedSlotOpacity = 1f;
    private const float PickupRange = 3f;

    [Header("UI References")] 
    [SerializeField] private GameObject hotBarObject;

    [Header("Visual Feedback")] 
    [SerializeField] private Material itemHighlightMaterial;

    [Header("Player References")] 
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerHandTransform;

    [Header("Input Actions")] 
    [SerializeField] private InputActionReference pickUpItemAction;
    [SerializeField] private InputActionReference dropItemAction;
    
    [Header("Hotbar Input Actions")] 
    [SerializeField] private InputActionReference[] hotbarActions;

    private readonly List<Slot> _hotbarSlots = new();
    private GameObject _currentlyHeldItem;
    private Material _highlightedItemOriginalMaterial;
    private Renderer _highlightedItemRenderer;
    private Action<InputAction.CallbackContext>[] _hotbarCallbacks;

    private int _selectedHotbarIndex;

    private void Awake()
    {
        _hotbarSlots.AddRange(hotBarObject.GetComponentsInChildren<Slot>());

        _hotbarCallbacks = new Action<InputAction.CallbackContext>[hotbarActions.Length];
        for (var i = 0; i < hotbarActions.Length; i++)
        {
            var index = i;
            _hotbarCallbacks[i] = _ => HandleHotbarKeySelection(index);
        }

        UpdateHotbarOpacity();
    }

    private void Update()
    {
        DetectLookedAtItem();
    }

    private void OnEnable()
    {
        pickUpItemAction.action.performed += Pickup;

        dropItemAction.action.performed += Drop;

        for (var i = 0; i < hotbarActions.Length; i++)
            hotbarActions[i].action.performed += _hotbarCallbacks[i];
    }

    private void OnDisable()
    {
        pickUpItemAction.action.performed -= Pickup;

        dropItemAction.action.performed -= Drop;

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

    private void Pickup(InputAction.CallbackContext ctx)
    {
        HandlePickUp();
    }

    private void Drop(InputAction.CallbackContext ctx)
    {
        HandleDropItem();
    }

    private void HandlePickUp()
    {
        if (_highlightedItemRenderer != null)
        {
            var item = _highlightedItemRenderer.GetComponent<Item>();
            if (item != null)
            {
                AddItem(item.item, item.amount);
                Destroy(item.gameObject);
                EquipHandItem();
            }
        }

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

    private void DetectLookedAtItem()
    {
        if (_highlightedItemRenderer != null)
        {
            _highlightedItemRenderer.material = _highlightedItemOriginalMaterial;
            _highlightedItemRenderer = null;
            _highlightedItemOriginalMaterial = null;
        }

        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, PickupRange))
        {
            var item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                var rend = item.GetComponent<Renderer>();
                if (rend != null)
                {
                    _highlightedItemOriginalMaterial = rend.material;
                    rend.material = itemHighlightMaterial;
                    _highlightedItemRenderer = rend;
                }
            }
        }
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