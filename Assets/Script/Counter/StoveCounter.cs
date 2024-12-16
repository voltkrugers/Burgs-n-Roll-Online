using System;
using System.Collections;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    private enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    
    [SerializeField] private FryingObjSO[] FryingObjSoArray;
    [SerializeField] private BurningObjSO[] BurningObjSoArray;

    private float fryingTimer;
    private float burningTimer;
    private FryingObjSO fryingObjSo;
    private BurningObjSO burningObjSo;
    private State _state;
    

    private void Start()
    {
        _state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObj())
        {
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    if (fryingTimer> fryingObjSo.fryingTimerMax)
                    {
                        fryingTimer = 0f;
                        GetKitchenObj().DestroySelf();

                        KitchenObj.SpawnKitchenObj(fryingObjSo.output, this);

                        _state = State.Fried;
                        burningTimer = 0f;
                        burningObjSo = getBurningRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());

                    }
                    break;
                case State.Fried:
                    
                    burningTimer += Time.deltaTime;
                    
                    if (burningTimer > burningObjSo.BurningTimerMax)
                    {
                        GetKitchenObj().DestroySelf();

                        KitchenObj.SpawnKitchenObj(burningObjSo.output, this);

                        _state = State.Burned;
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
                    player.GetKitchenObj().SetKitchenObjParent(this);
                    
                    fryingObjSo = getFryingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());

                    _state = State.Frying;
                    fryingTimer = 0f;
                }
            }
        }
        else
        {
            if (player.HasKitchenObj())
            {
                
            }
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
            }
        }
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
}
