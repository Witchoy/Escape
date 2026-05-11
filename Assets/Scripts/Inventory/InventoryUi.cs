// Attach to: Inventory Canvas / InventoryPanel GameObject
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    private const float DragIconOpacity = 0.5f;

    public event Action<Slot> OnHoveredSlotChanged;

    [SerializeField] public Image dragIcon;
    [SerializeField] private InputActionReference mouseAction;

    private Slot _draggedSlot;
    private Slot _lastHoveredSlot;
    private bool _isDragging;

    private void OnEnable()
    {
        mouseAction.action.performed += StartDrag;
        mouseAction.action.canceled += EndDrag;
        InventoryManager.Instance.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnDisable()
    {
        mouseAction.action.performed -= StartDrag;
        mouseAction.action.canceled -= EndDrag;
        InventoryManager.Instance.OnInventoryChanged -= OnInventoryChanged;
    }

    private void Update()
    {
        UpdateDragIconPosition();
        CheckHoveredSlotChanged();
    }

    private void CheckHoveredSlotChanged()
    {
        var hovered = GetHoveredSlot();
        if (hovered == _lastHoveredSlot) return;
        _lastHoveredSlot = hovered;
        OnHoveredSlotChanged?.Invoke(hovered);
    }

    private void StartDrag(InputAction.CallbackContext ctx)
    {
        Debug.Log("StartDrag");
        var hovered = GetHoveredSlot();
        if (hovered == null || !hovered.HasItem()) return;

        _draggedSlot = hovered;
        _isDragging = true;
        dragIcon.sprite = hovered.GetItem().itemIcon;
        dragIcon.color = new Color(1, 1, 1, DragIconOpacity);
        dragIcon.enabled = true;
    }

    private void EndDrag(InputAction.CallbackContext ctx)
    {
        Debug.Log("EndDrag");
        if (!_isDragging) return;

        var hovered = GetHoveredSlot();
        if (hovered != null)
            InventoryManager.Instance.MoveItem(_draggedSlot, hovered);

        dragIcon.enabled = false;
        _isDragging = false;
    }

    private void UpdateDragIconPosition()
    {
        if (!_isDragging) return;
        dragIcon.transform.position = Mouse.current.position.ReadValue();
    }

    private Slot GetHoveredSlot()
    {
        return InventoryManager.Instance.AllSlots.FirstOrDefault(s => s.hovering);
    }

    private void OnInventoryChanged()
    {
        // Cancel any in-flight drag if the source slot was cleared externally.
        if (_isDragging && _draggedSlot != null && !_draggedSlot.HasItem())
        {
            dragIcon.enabled = false;
            _isDragging = false;
        }
    }
}
