using System;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
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
                
            }
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
            }
        }
    }

    public override void AllowBirdPickup(IABird bird)
    {
        if (HasKitchenObj() && !bird.HasKitchenObj())
        {
            KitchenObj obj = GetKitchenObj();
            obj.SetKitchenObjParent(bird);
            bird.PickUpObject(obj);
        }else if (!HasKitchenObj() && bird.HasKitchenObj())
        {
            bird.GetKitchenObj().SetKitchenObjParent(this);
        }
    }
}
