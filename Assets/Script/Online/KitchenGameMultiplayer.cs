using System;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjListSO kitchenObjListSo;

    private void Awake()
    {
        Instance = this;
    }
    
    public  void SpawnKitchenObj(KitchenObjSO kitchenObjSo, IKitchenObjParent kitchenObjParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjSo),kitchenObjParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjSoIndex, NetworkObjectReference kitchenObjParentNetworkObjectReference)
    {
        KitchenObjSO kitchenObjSo = GetKitchenObjectSOFromIndex(kitchenObjSoIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjSo.prefab);
    
        NetworkObject kitchenObjecNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjecNetworkObject.Spawn(true);
    
        KitchenObj kitchenObj = kitchenObjectTransform.GetComponent<KitchenObj>();

        kitchenObjParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjParent kitchenObjParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjParent>();
        kitchenObj.SetKitchenObjParent(kitchenObjParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjSO kitchenObjSo)
    {
        return kitchenObjListSo.KitchenObjSoList.IndexOf(kitchenObjSo);
    }

    public KitchenObjSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjListSo.KitchenObjSoList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObj kitchenObj)
    {
        DestroyKitchenObjectServerRpc(kitchenObj.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObj kitchenObj = kitchenObjectNetworkObject.GetComponent<KitchenObj>();

        ClearKitchenObjOnParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObj.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObj kitchenObj = kitchenObjectNetworkObject.GetComponent<KitchenObj>();
        
        kitchenObj.ClearKitchenObjOnParent();
    }
}
