using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const float PickupRange = 3f;
    
    [Header("References")] 
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private InputActionReference useAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference inspectAction;

    private IHighlightable _highlightedItem;
    private bool _hasHit;
    private RaycastHit _currentHit;

    public event Action<Item> OnGrabPressed;
    public event Action<RaycastHit> OnUsePressed;
    public event Action OnDropPressed;
    public event Action OnInspectPressed;

    private void OnEnable()
    {
        useAction.action.performed += Use;
        interactAction.action.performed += Interact;
        dropAction.action.performed += Drop;
        inspectAction.action.performed += Inspect;
    }

    private void OnDisable()
    {
        useAction.action.performed -= Use;
        interactAction.action.performed -= Interact;
        dropAction.action.performed -= Drop;
        inspectAction.action.performed -= Inspect;
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
        OnUsePressed?.Invoke(_currentHit);
    }
    
    private void Interact(InputAction.CallbackContext context)
    {
        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, PickupRange))
        {
            var item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                ClearHighlight();
                OnGrabPressed?.Invoke(item);
                return;
            }
            
            var usable = hit.collider.GetComponent<IUsable>();
            if (usable != null)
            {
                ClearHighlight();
                usable.Use(_currentHit);
            }
        }
    }

    private void Inspect(InputAction.CallbackContext context)
    {
        OnInspectPressed?.Invoke();
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