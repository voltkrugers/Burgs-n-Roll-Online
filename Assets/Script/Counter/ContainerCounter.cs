using System;
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
            OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);
        }
    }
    
}
