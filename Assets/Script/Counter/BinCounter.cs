using System;
using Unity.Netcode;
using UnityEngine;

public class BinCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed; 
    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }
    
    public override void Interact(CharacterController player)
    {
        if (player.HasKitchenObj())
        {
            KitchenObj.DestroyKitchenObject(player.GetKitchenObj()); 
            
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObjectTrashed?.Invoke(this,EventArgs.Empty);
    }
}
