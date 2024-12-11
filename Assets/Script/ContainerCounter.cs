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
            Transform objTransform = Instantiate(ObjSo.prefab);
            objTransform.GetComponent<KitchenObj>().SetKitchenObjParent(player);
            OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);
        }
    }


}
