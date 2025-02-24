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

    private int GetKitchenObjectSOIndex(KitchenObjSO kitchenObjSo)
    {
        return kitchenObjListSo.KitchenObjSoList.IndexOf(kitchenObjSo);
    }

    private KitchenObjSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjListSo.KitchenObjSoList[kitchenObjectSOIndex];
    }
}
