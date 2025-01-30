using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const String PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput Instance { get; private set; }
    
    public EventHandler OnInteractAction;
    public EventHandler OnSecondInteractAction;
    public EventHandler OnPauseAction;
    
    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        SecondInteract,
        Pause,
    }
    
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += interactPerf;
        _playerInputActions.Player.SecondInteract.performed += SecondinteractPerf;
        _playerInputActions.Player.Pause.performed += PausePerf;


        
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= interactPerf;
        _playerInputActions.Player.SecondInteract.performed -= SecondinteractPerf;
        _playerInputActions.Player.Pause.performed -= PausePerf;
        
        _playerInputActions.Dispose();
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

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
                case Binding.Move_Up:
                    return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
                case Binding.Move_Down:
                    return _playerInputActions.Player.Move.bindings[2].ToDisplayString();                
                case Binding.Move_Left:
                    return _playerInputActions.Player.Move.bindings[3].ToDisplayString();                
                case Binding.Move_Right:
                    return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
                case Binding.Interact:
                    return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
                case Binding.SecondInteract:
                    return _playerInputActions.Player.SecondInteract.bindings[0].ToDisplayString();
                case Binding.Pause:
                    return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding,Action onActionRebound)
    {
        InputAction inputAction;
        int bindingIndex;
        
        _playerInputActions.Player.Disable();
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.SecondInteract:
                inputAction = _playerInputActions.Player.SecondInteract;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            _playerInputActions.Player.Enable();
            onActionRebound();
            
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS,_playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }).Start();


    }
}