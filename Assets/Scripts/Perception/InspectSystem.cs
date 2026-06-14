using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectSystem : MonoBehaviour
{
    private const float RotationSpeed = 5f;
    private readonly Vector3 _inspectLocalPosition = new(-0.75f, 0.15f, 0.0f);
    [SerializeField] private InputActionReference inspectHoldAction;
    [SerializeField] private PlayerInventory playerInventory;

    public event Action<bool> OnInspectStateChanged;

    private Transform _objectToInspect;
    private Vector3 _objectOriginalPosition;
    private Quaternion _objectOriginalRotation;
    private bool _isHolding;

    private bool IsInspecting => _objectToInspect != null;

    private void Awake()
    {
        playerInventory.OnInspectItem += HandleInspect;
    }

    private void OnEnable()
    {
        inspectHoldAction.action.performed += OnHoldStarted;
        inspectHoldAction.action.canceled += OnHoldCanceled;
    }

    private void OnDisable()
    {
        inspectHoldAction.action.performed -= OnHoldStarted;
        inspectHoldAction.action.canceled -= OnHoldCanceled;
    }

    private void HandleInspect(Transform target)
    {
        if (IsInspecting)
            StopInspect();
        else
            StartInspect(target);
    }

    private void Update()
    {
        if (!IsInspecting || !_isHolding) return;

        var delta = Mouse.current.delta.ReadValue();
        var rotationX = delta.y * RotationSpeed;
        var rotationY = -delta.x * RotationSpeed;

        _objectToInspect.rotation = Quaternion.Euler(rotationX, rotationY, 0) * _objectToInspect.rotation;
    }

    public void StartInspect(Transform target)
    {
        _objectToInspect = target;
        _objectOriginalPosition = target.transform.localPosition;
        _objectOriginalRotation = target.localRotation;
        _objectToInspect.transform.localPosition = _inspectLocalPosition;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnInspectStateChanged?.Invoke(true);
    }

    public void StopInspect()
    {
        _objectToInspect.transform.localPosition = _objectOriginalPosition;
        _objectToInspect.transform.localRotation = _objectOriginalRotation;
        _objectToInspect = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnInspectStateChanged?.Invoke(false);
    }

    private void OnHoldStarted(InputAction.CallbackContext ctx) => _isHolding = true;
    private void OnHoldCanceled(InputAction.CallbackContext ctx) => _isHolding = false;
}
