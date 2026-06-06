// Attach to: Player GameObject
using UnityEngine;
using UnityEngine.InputSystem;

public class InspectSystem : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private InputActionReference inspectHoldAction;
    
    private Transform _objectToInspect;
    private Vector3 _objectOriginalPosition;
    private Quaternion _objectOriginalRotation;
    private bool _isHolding;

    private bool IsInspecting => _objectToInspect != null;

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

    private void Update()
    {
        if (!IsInspecting || !_isHolding) return;

        var delta = Mouse.current.delta.ReadValue();
        var rotationX = delta.y * rotationSpeed;
        var rotationY = -delta.x * rotationSpeed;

        _objectToInspect.rotation = Quaternion.Euler(rotationX, rotationY, 0) * _objectToInspect.rotation;
    }

    public void StartInspect(Transform target)
    {
        _objectToInspect = target;
        _objectOriginalPosition = target.transform.localPosition;
        _objectOriginalRotation = target.localRotation;
        _objectToInspect.transform.localPosition = new Vector3(-0.65f, -0.25f, 0.35f);
    }

    public void StopInspect()
    {
        _objectToInspect.transform.localPosition = _objectOriginalPosition;
        _objectToInspect.transform.localRotation = _objectOriginalRotation;
        _objectToInspect = null;
    }

    private void OnHoldStarted(InputAction.CallbackContext ctx) => _isHolding = true;
    private void OnHoldCanceled(InputAction.CallbackContext ctx) => _isHolding = false;
}
