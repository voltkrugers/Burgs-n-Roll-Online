using UnityEngine;

public class CuttingCounter : BaseCounter
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArray;
    
    public override void Interact(CharacterController player)
    {
        if (!HasKitchenObj())
        {
            if (player.HasKitchenObj())
            {
                if (HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSo()))
                {
                    player.GetKitchenObj().SetKitchenObjParent(this);
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
            KitchenObjSO outputKitchenObjSo = GetOutputForInput(GetKitchenObj().GetKitchenObjSo());
            
            GetKitchenObj().DestroySelf();
            
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
        foreach (var cuttingRecipeSo in cuttingRecipeSoArray)
        {
            if (cuttingRecipeSo.input == inputKitchenObjSo)
            {
                return cuttingRecipeSo.output;
            }
        }

        return null;
    }
}
