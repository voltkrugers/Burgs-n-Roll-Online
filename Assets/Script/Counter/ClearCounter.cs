using System;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjSO ObjSo;
    

    public override void Interact(CharacterController player)
    {
        if (!HasKitchenObj())
        {
            if (player.HasKitchenObj())
            {
                player.GetKitchenObj().SetKitchenObjParent(this);
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
                else
                {
                    if (GetKitchenObj().TryGetPlate(out plateKitchenObjet ))
                    {
                        if (plateKitchenObjet.TryAddIngredient(player.GetKitchenObj().GetKitchenObjSo()))
                        {
                            KitchenObj.DestroyKitchenObject(player.GetKitchenObj()); 
                        }
                    }
                }
            }
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
            }
        }
    }
    
}
