using System;
using UnityEngine;

public class CuttingCounter : BaseCounter , IHasProgress
{

    public static event EventHandler OnAnyCut; 
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArray;

    private int cuttingProgress;
    
    public override void Interact(CharacterController player)
    {
        if (!HasKitchenObj())
        {
            if (player.HasKitchenObj())
            {
                if (HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSo()))
                {
                    player.GetKitchenObj().SetKitchenObjParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        ProgressNormalized = (float)cuttingProgress/ cuttingRecipeSo.cuttingProgressMax
                    });
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
                        GetKitchenObj().DestroySelf();
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

    public override void SecondInteract(CharacterController player)
    {
        if (HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSo()))
        {
            cuttingProgress++;

            OnCut?.Invoke(this,EventArgs.Empty);
            OnAnyCut?.Invoke(this,EventArgs.Empty);
            
            CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());
            
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = (float)cuttingProgress/ cuttingRecipeSo.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
            {
                KitchenObjSO outputKitchenObjSo = GetOutputForInput(GetKitchenObj().GetKitchenObjSo());
            
                GetKitchenObj().DestroySelf();
            
                KitchenObj.SpawnKitchenObj(outputKitchenObjSo, this);
            }
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
