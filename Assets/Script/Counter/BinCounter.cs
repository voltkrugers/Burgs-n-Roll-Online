using UnityEngine;

public class BinCounter : BaseCounter
{
    public override void Interact(CharacterController player)
    {
        if (player.HasKitchenObj())
        {
            player.GetKitchenObj().DestroySelf();
        }
    }
}
