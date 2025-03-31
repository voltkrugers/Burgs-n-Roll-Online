using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObjet : KitchenObj
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    
    public class OnIngredientAddedEventArgs : EventArgs
    {
    public KitchenObjSO KitchenObjSo;
    }
    
    [SerializeField] private List<KitchenObjSO> validKitchenObjSoList;
    
    private List<KitchenObjSO> KitchenObjSoList;
    protected override void Awake()
    {
        base.Awake();
        KitchenObjSoList = new List<KitchenObjSO>();
    }

    public bool TryAddIngredient(KitchenObjSO kitchenObjSo)
    {
        if (!validKitchenObjSoList.Contains(kitchenObjSo))
        {
            return false;
        }
        if (KitchenObjSoList.Contains(kitchenObjSo))
        {
            return false;
        }
        else
        {

            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjSo));
            return true;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }
    
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjSO kitchenObjSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        KitchenObjSoList.Add(kitchenObjSo);
            
        OnIngredientAdded?.Invoke(this,new OnIngredientAddedEventArgs
        {
            KitchenObjSo = kitchenObjSo
        });
    }

    public List<KitchenObjSO> GetKitchenObjectSoList()
    {
        return KitchenObjSoList;
    }
}
