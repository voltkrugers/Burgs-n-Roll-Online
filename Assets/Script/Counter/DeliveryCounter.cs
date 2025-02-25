using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(CharacterController player)
    {
        if (player.HasKitchenObj())
        {
            if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObjet plateKitchenObjet))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObjet);
                KitchenObj.DestroyKitchenObject(player.GetKitchenObj());
                
            }
        }
    }
}
