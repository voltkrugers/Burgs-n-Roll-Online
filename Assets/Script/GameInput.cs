using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    public EventHandler OnInteractAction;
    public EventHandler OnSecondInteractAction;
    public EventHandler OnPauseAction;
    
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += interactPerf;
        _playerInputActions.Player.SecondInteract.performed += SecondinteractPerf;
        _playerInputActions.Player.Pause.performed += PausePerf;
    }
    

    private void PausePerf(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this,EventArgs.Empty);
    }
    
    private void SecondinteractPerf(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSecondInteractAction?.Invoke(this,EventArgs.Empty);   
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