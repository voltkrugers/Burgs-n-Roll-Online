using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public EventHandler OnInteractAction;
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += interactPerf;
    }

    private void interactPerf(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
     OnInteractAction?.Invoke(this,EventArgs.Empty);   
    }
    public Vector2 GetMouvementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();


        inputVector = inputVector.normalized;
        return inputVector;
    }
}