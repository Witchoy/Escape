using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;
    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference inspectAction;

    private InspectSystem _inspectSystem;

    public PlayerState State { get; private set; } = PlayerState.IDLE;
    public bool IsInputBlocked => State is PlayerState.ININVENTORY or PlayerState.INSPECTING;

    public void SetMovementState(PlayerState state) => State = state;

    private void Awake()
    {
        _inspectSystem = GetComponent<InspectSystem>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Force slots to initialize before inventory is hidden
        inventory.SetActive(true);
        inventory.SetActive(false);
    }

    private void OnEnable()
    {
        openInventoryAction.action.performed += OpenInventory;
        openInventoryAction.action.canceled += OpenInventory;
        inspectAction.action.performed += ToggleInspect;
    }

    private void OnDisable()
    {
        openInventoryAction.action.performed -= OpenInventory;
        openInventoryAction.action.canceled -= OpenInventory;
        inspectAction.action.performed -= ToggleInspect;
    }

    private void OpenInventory(InputAction.CallbackContext ctx)
    {
        if (State == PlayerState.INSPECTING) return;
        var opening = State != PlayerState.ININVENTORY;
        State = opening ? PlayerState.ININVENTORY : PlayerState.IDLE;

        inventory.SetActive(opening);
        cinemachineInputAxisController.enabled = !opening;
        Cursor.lockState = opening ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = opening;
    }

    private void ToggleInspect(InputAction.CallbackContext ctx)
    {
        if (State == PlayerState.ININVENTORY) return;

        if (State == PlayerState.INSPECTING)
        {
            State = PlayerState.IDLE;
            _inspectSystem.StopInspect();
            cinemachineInputAxisController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        var target = HotBarController.Instance.GetEquippedItemTransform();
        if (target == null) return;

        State = PlayerState.INSPECTING;
        _inspectSystem.StartInspect(target);
        cinemachineInputAxisController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
