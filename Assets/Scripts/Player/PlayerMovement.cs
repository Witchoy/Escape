using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;

    [Header("Jump and Fall")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float initialFallVelocity = -2f;

    [Header("Crouching")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float cameraOffset = 0.4f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;

    private PlayerController _playerController;
    private CharacterController _characterController;

    private Vector2 _moveInput;
    private float _verticalVelocity;
    private bool _isCrouchHeld;
    private bool _isSprintHeld;
    private bool _wantsToStandUp;
    private bool _isGrounded;

    private bool IsCrouching => _playerController.State is PlayerState.CROUCHING or PlayerState.CROUCHWALKING;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        jumpAction.action.performed += Jump;
        crouchAction.action.performed += Crouch;
        crouchAction.action.canceled += Crouch;
        sprintAction.action.performed += Sprint;
        sprintAction.action.canceled += Sprint;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        jumpAction.action.performed -= Jump;
        crouchAction.action.performed -= Crouch;
        crouchAction.action.canceled -= Crouch;
        sprintAction.action.performed -= Sprint;
        sprintAction.action.canceled -= Sprint;
    }

    private void Update()
    {
        _isGrounded = _characterController.isGrounded;
        UpdateMovementState();
        HandleGravity();
        HandleCrouchTransition();
        HandleMovement();
    }

    private void UpdateMovementState()
    {
        if (_playerController.IsInputBlocked) return;

        if (_wantsToStandUp && CanStandUp())
            _wantsToStandUp = false;

        var isCrouching = _isCrouchHeld || _wantsToStandUp;
        var isMoving = _moveInput != Vector2.zero;

        _playerController.SetMovementState((isCrouching, _isSprintHeld, isMoving) switch
        {
            (true, _, true)    => PlayerState.CROUCHWALKING,
            (true, _, false)   => PlayerState.CROUCHING,
            (false, true, true)=> PlayerState.RUNNING,
            (false, _, true)   => PlayerState.WALKING,
            _                  => PlayerState.IDLE
        });
    }

    private void HandleMovement()
    {
        var inputDir = _playerController.IsInputBlocked ? Vector2.zero : _moveInput;
        var move = cameraTransform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y)).normalized;
        var speed = _playerController.State switch
        {
            PlayerState.RUNNING       => runSpeed,
            PlayerState.CROUCHWALKING => crouchSpeed,
            PlayerState.WALKING       => walkSpeed,
            _                         => 0f
        };
        var finalMove = move * speed;
        finalMove.y = _verticalVelocity;

        var collisions = _characterController.Move(finalMove * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0) _verticalVelocity = initialFallVelocity;
    }

    private void HandleGravity()
    {
        if (_isGrounded && _verticalVelocity < 0) _verticalVelocity = initialFallVelocity;
        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleCrouchTransition()
    {
        var targetHeight = IsCrouching ? crouchingHeight : standingHeight;
        var currentHeight = _characterController.height;

        if (Mathf.Abs(currentHeight - targetHeight) < 0.01f)
        {
            _characterController.height = targetHeight;
            return;
        }

        var newHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        _characterController.height = newHeight;
        _characterController.center = Vector3.up * (newHeight * 0.25f);

        var camPos = cameraTransform.localPosition;
        camPos.y = targetHeight - cameraOffset;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, camPos, crouchTransitionSpeed * Time.deltaTime);
    }

    private bool CanStandUp()
    {
        var castDistance = standingHeight - _characterController.height;
        return !Physics.CapsuleCast(
            transform.position + _characterController.center,
            transform.position + Vector3.up * _characterController.height / 2,
            _characterController.radius,
            Vector3.up,
            out _,
            castDistance
        );
    }

    private void StoreMovementInput(InputAction.CallbackContext ctx) => _moveInput = ctx.ReadValue<Vector2>();

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (_isGrounded && !_playerController.IsInputBlocked) _verticalVelocity = jumpForce;
    }

    private void Crouch(InputAction.CallbackContext ctx)
    {
        if (_playerController.IsInputBlocked) return;
        if (ctx.performed) { _isCrouchHeld = true; _wantsToStandUp = false; }
        else if (ctx.canceled) { _isCrouchHeld = false; _wantsToStandUp = true; }
    }

    private void Sprint(InputAction.CallbackContext ctx) => _isSprintHeld = ctx.performed;
}
