using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRange;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference interactAction;

    private PlayerController _playerController;
    private IHighlightable _currentHighlighted;
    private RaycastHit _raycastHit;
    private bool _hasRaycastHit;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        interactAction.action.performed += Interact;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= Interact;
    }

    private void Update()
    {
        UpdateRaycast();
        HandleHighlight();
    }

    private void UpdateRaycast()
    {
        var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        _hasRaycastHit = Physics.Raycast(ray, out _raycastHit, interactRange);
    }

    private void OnDrawGizmos()
    {
        if (cameraTransform == null) return;

        var origin = cameraTransform.position;
        var direction = cameraTransform.forward;

        if (_hasRaycastHit)
        {
            // Green ray up to the hit point
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, _raycastHit.point);

            // Yellow sphere at hit point
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_raycastHit.point, 0.05f);

            // Red remaining ray past the hit point
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_raycastHit.point, origin + direction * interactRange);
        }
        else
        {
            // Gray full ray when nothing is hit
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(origin, origin + direction * interactRange);
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (_playerController.IsInputBlocked) return;
        if (!_hasRaycastHit) return;

        var target = _raycastHit.collider.gameObject;
        var usable = HotBarController.Instance.GetEquippedUsable();

        if (usable != null)
        {
            if (target.TryGetComponent(out PlaySFX usableSfx)) usableSfx.Play();
            if (usable.Use(_raycastHit))
                HotBarController.Instance.ConsumeEquippedItem();
        }
        else if (target.TryGetComponent(out IInteractable interactObj))
        {
            if (target.TryGetComponent(out PlaySFX sfx)) sfx.Play();
            interactObj.Interact();
        }
    }

    private void HandleHighlight()
    {
        if (_currentHighlighted is MonoBehaviour mb && !mb)
            _currentHighlighted = null;

        if (_hasRaycastHit)
        {
            var highlightable = _raycastHit.collider ? _raycastHit.collider.GetComponent<IHighlightable>() : null;
            if (highlightable == _currentHighlighted) return;
            _currentHighlighted?.Unhighlight();
            _currentHighlighted = highlightable;
            _currentHighlighted?.Highlight();
        }
        else
        {
            _currentHighlighted?.Unhighlight();
            _currentHighlighted = null;
        }
    }
}
