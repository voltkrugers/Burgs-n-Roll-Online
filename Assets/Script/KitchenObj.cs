using System;
using Unity.Netcode;
using UnityEngine;

public class KitchenObj : NetworkBehaviour
{

    [SerializeField] private KitchenObjSO kitchenObjSo;

    private IKitchenObjParent kitchenObjParent;
    private FollowTransform followTransform;

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjSO GetKitchenObjSo()
    {
        return kitchenObjSo; 
    }

    public void SetKitchenObjParent(IKitchenObjParent KitchenObjParent)
    {
        SetKitchenObjectParentServerRpc(KitchenObjParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjParent kitchenObjParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjParent>();
        
        if (this.kitchenObjParent != null)
        {
            this.kitchenObjParent.ClearKitchenObject();
        }
        
        this.kitchenObjParent = kitchenObjParent;
        
        kitchenObjParent.SetKitchenObject(this);
        
        followTransform.SetTargetTransform(kitchenObjParent.GetKitchenObjFollowTransform());
    }
    
    public IKitchenObjParent GetKitchenObjParent()
    {
        return kitchenObjParent;
    }

    public void DestroySelf()
    {
        kitchenObjParent.ClearKitchenObject();
        
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObjet plateKitchenObjet)
    {
        if (this is PlateKitchenObjet)
        {
            plateKitchenObjet = this as PlateKitchenObjet;
            return true;
        }
        else
        {
            plateKitchenObjet = null;
            return false;
        }
    }
    public static void SpawnKitchenObj(KitchenObjSO kitchenObjSo, IKitchenObjParent kitchenObjParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObj(kitchenObjSo, kitchenObjParent);
    }

    public void SetKitchenObjSo(KitchenObjSO dummyKitchenObjSo)
    {
        this.kitchenObjSo = dummyKitchenObjSo;
    }
}
