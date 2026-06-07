using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const float PickupRange = 3f;
    
    [Header("References")] 
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private InputActionReference useAction;
    [SerializeField] private InputActionReference grabAction;
    [SerializeField] private InputActionReference dropAction;

    private IHighlightable _highlightedItem;
    private bool _hasHit;
    private RaycastHit _currentHit;

    public event Action<Item> OnGrabPressed;
    public event Action OnDropPressed;

    private void OnEnable()
    {
        useAction.action.performed += Use;
        grabAction.action.performed += Grab;
        dropAction.action.performed += Drop;
    }

    private void OnDisable()
    {
        useAction.action.performed -= Use;
        grabAction.action.performed -= Grab;
        dropAction.action.performed -= Drop;
    }
    
    private void Update()
    {
        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        _hasHit = Physics.Raycast(ray, out _currentHit, PickupRange);
        HandleHighlight();
    }
    
    private void Use(InputAction.CallbackContext context)
    {
        if (!_hasHit) return;
        _currentHit.collider.GetComponent<IUsable>()?.Use();
    }
    
    private void Grab(InputAction.CallbackContext context)
    {
        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, PickupRange))
        {
            var item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                ClearHighlight();
                OnGrabPressed?.Invoke(item);
            }
        }
    }

    private void Drop(InputAction.CallbackContext context)
    {
        // TODO: Check if Item can be dropped
        OnDropPressed?.Invoke();
    }
    
    private void ClearHighlight()
    {
        if (_highlightedItem != null)
        {
            _highlightedItem.Unhighlight();
            _highlightedItem = null;
        }
    }

    private void HandleHighlight()
    {
        ClearHighlight();
        if (!_hasHit) return;
        if (_currentHit.collider == null) return;

        var item = _currentHit.collider.GetComponent<IHighlightable>();
        if (item != null)
        {
            item.Highlight();
            _highlightedItem = item;
        }
    }
}