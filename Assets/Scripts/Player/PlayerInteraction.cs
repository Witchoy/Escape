using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Material itemHighlightMaterial;
    [SerializeField] private float pickupRange = 3f;

    public static event Action<Item> OnItemPickedUp;

    private Renderer _highlightedItemRenderer;
    private Material _highlightedItemOriginalMaterial;
    private Item _lookedAtItem;
    private IInteractable _lookedAtInteractable;

    private void Update()
    {
        DetectLookedAtItem();
    }

    public void TryInteract()
    {
        if (_lookedAtItem != null)
        {
            Item itemToPickUp = _lookedAtItem; 
            ClearHighlight();
            OnItemPickedUp?.Invoke(itemToPickUp);
            return;
        }

        _lookedAtInteractable?.Interact();
    }
    
    private void ClearHighlight()
    {
        if (_highlightedItemRenderer != null)
        {
            _highlightedItemRenderer.material = _highlightedItemOriginalMaterial;
            _highlightedItemRenderer = null;
            _highlightedItemOriginalMaterial = null;
        }

        _lookedAtItem = null;
        _lookedAtInteractable = null;
    }

    private void DetectLookedAtItem()
    {
        var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        
        if (Physics.Raycast(ray, out var hit, pickupRange))
        {
            var item = hit.collider.GetComponent<Item>();
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (item != null || interactable != null)
            {
                var rend = hit.collider.GetComponent<Renderer>();

                if (rend != _highlightedItemRenderer)
                {
                    ClearHighlight();

                    _lookedAtItem = item;
                    _lookedAtInteractable = interactable;

                    if (rend != null)
                    {
                        _highlightedItemOriginalMaterial = rend.material;
                        rend.material = itemHighlightMaterial;
                        _highlightedItemRenderer = rend;
                    }
                }
                
                return; 
            }
        }

        ClearHighlight();
    }
}