using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference interactionAction;
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private PlayerInteraction playerInteraction;
    
    public static event Action OnDropPressed;
    
    private void OnEnable()
    {
        interactionAction.action.performed += Interact;
        dropAction.action.performed += Drop;
    }

    private void OnDisable()
    {
        interactionAction.action.performed -= Interact;
        dropAction.action.performed -= Drop;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        playerInteraction.TryInteract();
    }

    private void Drop(InputAction.CallbackContext context)
    {
        OnDropPressed?.Invoke();
    }

}
