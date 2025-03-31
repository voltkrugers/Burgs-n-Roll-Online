using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter , IHasProgress
{

    public static event EventHandler OnAnyCut;

    public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] public CuttingRecipeSO[] cuttingRecipeSoArray;

    private int cuttingProgress;
    
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
                    InteractLogicPlaceObjectOnCounterServerRpc();
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
                    }
                }
            }
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
                OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
                {
                    ProgressNormalized = 0f
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;
        
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = 0f
        });
    }

    public override void SecondInteract(CharacterController player)
    {
        if (HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSo()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this,EventArgs.Empty);
        OnAnyCut?.Invoke(this,EventArgs.Empty);
            
        CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());
            
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            ProgressNormalized = (float)cuttingProgress/ cuttingRecipeSo.cuttingProgressMax
        });


    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());
        
        if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
        {
            KitchenObjSO outputKitchenObjSo = GetOutputForInput(GetKitchenObj().GetKitchenObjSo());
            
            KitchenObj.DestroyKitchenObject(GetKitchenObj());
            
            KitchenObj.SpawnKitchenObj(outputKitchenObjSo, this);
        }
    }
    
    private bool HasRecipeWithInput(KitchenObjSO inputKitchenObjSo)
    {
        foreach (var cuttingRecipeSo in cuttingRecipeSoArray)
        {
            if (cuttingRecipeSo.input == inputKitchenObjSo)
            {
                return true;
            }
        }

        return false;
    }
    
    private KitchenObjSO GetOutputForInput(KitchenObjSO inputKitchenObjSo)
    {
        CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(inputKitchenObjSo);
        if (cuttingRecipeSo != null)
        {
                return cuttingRecipeSo.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipeSO getCuttingRecipeSoWithInput(KitchenObjSO inputKitchenObjectSo)
    {
        foreach (var cuttingRecipeSo in cuttingRecipeSoArray)
        {
            if (cuttingRecipeSo.input == inputKitchenObjectSo)
            {
                return cuttingRecipeSo;
            }
        }

        return null;
    }
}
