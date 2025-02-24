using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjSO ObjSo;

    public EventHandler OnPlayerGrabbedObject;
    
    public override void Interact(CharacterController player)
    {
        if (!player.HasKitchenObj())
        {
            KitchenObj.SpawnKitchenObj(ObjSo, player);
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
        OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);
    }
}  
