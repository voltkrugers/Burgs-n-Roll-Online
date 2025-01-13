using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(CharacterController player)
    {
        if (player.HasKitchenObj())
        {
            if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObjet plateKitchenObjet))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObjet);
                player.GetKitchenObj().DestroySelf();
            }
        }
    }
}
