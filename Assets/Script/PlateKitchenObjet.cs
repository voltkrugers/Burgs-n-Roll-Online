using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObjet : KitchenObj
{
    [SerializeField] private List<KitchenObjSO> validKitchenObjSoList;
    
    private List<KitchenObjSO> KitchenObjSoList;
    private void Awake()
    {
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
            KitchenObjSoList.Add(kitchenObjSo);
            return true;
        }
        
    }
}
