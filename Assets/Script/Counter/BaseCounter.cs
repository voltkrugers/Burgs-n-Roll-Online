using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour , IKitchenObjParent
{

    public static event EventHandler OnAnyObjectPlacedHere; 
    
    new public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }
    
    
    [SerializeField] private Transform CounterTopPoint;
    
    protected KitchenObj kitchenObj;
    public virtual void Interact(CharacterController player)
    {
        
    }
    public virtual void SecondInteract(CharacterController player)
    {
        
    }

    public Transform GetKitchenObjFollowTransform()
    {
        return CounterTopPoint;
    }

    public void SetKitchenObject(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;

        if (kitchenObj !=null)
        {
            OnAnyObjectPlacedHere?.Invoke(this,EventArgs.Empty);
        }
    }

    public KitchenObj GetKitchenObj()
    {
        return kitchenObj;
    }

    public void ClearKitchenObject()
    {
        kitchenObj = null;
    }

    public bool HasKitchenObj()
    {
        return kitchenObj != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
