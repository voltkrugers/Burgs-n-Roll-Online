using UnityEngine;

public class CuttingCounter : BaseCounter
{

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

    public override void SecondInteract(CharacterController player)
    {
        if (HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSo()))
        {
            cuttingProgress++;

            CuttingRecipeSO cuttingRecipeSo = getCuttingRecipeSoWithInput(GetKitchenObj().GetKitchenObjSo());

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
