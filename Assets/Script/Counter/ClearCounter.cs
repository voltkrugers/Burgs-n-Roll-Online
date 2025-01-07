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
                if (player.GetKitchenObj() is PlateKitchenObjet)
                {
                    PlateKitchenObjet plateKitchenObjet = player.GetKitchenObj() as PlateKitchenObjet;
                    if (plateKitchenObjet.TryAddIngredient(GetKitchenObj().GetKitchenObjSo()))
                    {
                        GetKitchenObj().DestroySelf();
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
