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
                        GetKitchenObj().DestroySelf();
                    }
                    
                }
                else
                {
                    if (GetKitchenObj().TryGetPlate(out plateKitchenObjet ))
                    {
                        if (plateKitchenObjet.TryAddIngredient(player.GetKitchenObj().GetKitchenObjSo()))
                        {
                            player.GetKitchenObj().DestroySelf();
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
