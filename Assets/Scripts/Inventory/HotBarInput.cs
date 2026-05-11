// Attach to: Player GameObject (same as HotBarController)
using UnityEngine;
using UnityEngine.InputSystem;

public class HotBarInput : MonoBehaviour
{
    private static readonly Key[] DigitKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3,
        Key.Digit4, Key.Digit5, Key.Digit6
    };

    [SerializeField] private InputActionReference dropAction;

    private HotBarController _hotBarController;

    private void Awake()
    {
        _hotBarController = GetComponent<HotBarController>();
    }

    private void OnEnable()
    {
        dropAction.action.performed += OnDrop;
    }

    private void OnDisable()
    {
        dropAction.action.performed -= OnDrop;
    }

    private void Update()
    {
        if (_hotBarController.IsInputBlocked) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        var slots = InventoryManager.Instance.HotBarSlots;
        for (var i = 0; i < Mathf.Min(slots.Count, DigitKeys.Length); i++)
        {
            if (!keyboard[DigitKeys[i]].wasPressedThisFrame) continue;
            _hotBarController.Select(i);
        }
    }

    private void OnDrop(InputAction.CallbackContext ctx) => _hotBarController.DropEquipped();
}
