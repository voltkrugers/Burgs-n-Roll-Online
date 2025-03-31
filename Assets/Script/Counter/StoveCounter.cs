using System;
using System.Collections;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    
    [SerializeField] private FryingObjSO[] FryingObjSoArray;
    [SerializeField] private BurningObjSO[] BurningObjSoArray;

    private NetworkVariable<float>  fryingTimer = new NetworkVariable<float>(0f); 
    private NetworkVariable<float>  burningTimer = new NetworkVariable<float>(0f);
    private FryingObjSO fryingObjSo;
    private BurningObjSO burningObjSo;
    private NetworkVariable<State>  _state = new NetworkVariable<State>(State.Idle);
    

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousvalue, State newvalue)
    {
       OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
       {
           state = _state.Value
       });
       if (_state.Value == State.Burned || _state.Value== State.Idle)
       {
           OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
           {
               ProgressNormalized = 0f
           });
       }
    }

    private void BurningTimer_OnValueChanged(float previousvalue, float newvalue)
    {
        float burningTimerMax = burningObjSo != null ? burningObjSo.BurningTimerMax : 1f;
        
        OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = burningTimer.Value/burningTimerMax
        });
    }

    private void FryingTimer_OnValueChanged(float previousvalue, float newvalue)
    {
        float fryingTimerMax = fryingObjSo != null ? fryingObjSo.fryingTimerMax : 1f;
        
        OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = fryingTimer.Value/fryingTimerMax
        });
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (HasKitchenObj())
        {
            switch (_state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    
                    if (fryingTimer.Value> fryingObjSo.fryingTimerMax)
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());

                        KitchenObj.SpawnKitchenObj(fryingObjSo.output, this);

                        _state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        
                        SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObj().GetKitchenObjSo()));
                        
                    }
                    break;
                case State.Fried:
                    
                    burningTimer.Value += Time.deltaTime;
                    
                    
                    if (burningTimer.Value > burningObjSo.BurningTimerMax)
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());

                        KitchenObj.SpawnKitchenObj(burningObjSo.output, this);

                        _state.Value = State.Burned;
                        
                    }
                    break;
                case State.Burned:
                    break;
            }   
        }
    }

    public override void Interact(CharacterController player)
    {
        if (!HasKitchenObj())
        {
            if (player.HasKitchenObj())
            {
                if (HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSo()))
                {
                    KitchenObj kitchenObj = player.GetKitchenObj();
                    kitchenObj.SetKitchenObjParent(this);

                    InteractLogicPlaceObjOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObj.GetKitchenObjSo()));

                }
            }
        }
        else
        {
            if (player.HasKitchenObj())
            {
                if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObjet plateKitchenObjet))
                {
                    if (plateKitchenObjet.TryAddIngredient(GetKitchenObj().GetKitchenObjSo()))
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());
                        SetStateIdleServerRPC();
                    }
                }
            }
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
                SetStateIdleServerRPC();

            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRPC()
    {
        _state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjOnCounterServerRpc(int kitchenObjSOIndex)
    {
        fryingTimer.Value = 0f;
        _state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjSOIndex);
    }
    
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjSOIndex);
        fryingObjSo = getFryingRecipeSoWithInput(kitchenObjSO);
    }
    
    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjSOIndex);
        burningObjSo = getBurningRecipeSoWithInput(kitchenObjSO);
    }
    private bool HasRecipeWithInput(KitchenObjSO inputKitchenObjSo)
    {
        foreach (var fryingObjSo in FryingObjSoArray)
        {
            if (fryingObjSo.input == inputKitchenObjSo)
            {
                return true;
            }
        }

        return false;
    }
    
    private KitchenObjSO GetOutputForInput(KitchenObjSO inputKitchenObjSo)
    {
        FryingObjSO fryingObjSo = getFryingRecipeSoWithInput(inputKitchenObjSo);
        if (fryingObjSo != null)
        {
            return fryingObjSo.output;
        }
        else
        {
            return null;
        }
    }

    private FryingObjSO getFryingRecipeSoWithInput(KitchenObjSO inputKitchenObjectSo)
    {
        foreach (var fryingObjSo in FryingObjSoArray)
        {
            if (fryingObjSo.input == inputKitchenObjectSo)
            {
                return fryingObjSo;
            }
        }

        return null;
    }
    
    private BurningObjSO getBurningRecipeSoWithInput(KitchenObjSO inputKitchenObjectSo)
    {
        foreach (BurningObjSO BurningObjSO in BurningObjSoArray)
        {
            if (BurningObjSO.input == inputKitchenObjectSo)
            {
                return BurningObjSO;
            }
        }

        return null;
    }

    public bool IsFried()
    {
        return _state.Value == State.Fried;
    }

}
