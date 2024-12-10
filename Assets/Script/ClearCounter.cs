using System;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjParent
{
    [SerializeField] private KitchenObjSO ObjSo;
    [SerializeField] private Transform CounterTopPoint;
    [SerializeField] private ClearCounter second;
    [SerializeField] private bool testing;

    private KitchenObj kitchenObj;

    private void Update()
    {
        if (testing && Input.GetKeyDown(KeyCode.T))
        {
            if (kitchenObj!=null)
            {
                kitchenObj.SetKitchenObjParent(second);
            }
        }
    }

    public void Interact(CharacterController player)
    {
        if (kitchenObj==null)
        {
            Transform objTransform = Instantiate(ObjSo.prefab, CounterTopPoint);
            objTransform.localPosition = Vector3.zero;
            objTransform.GetComponent<KitchenObj>().SetKitchenObjParent(this);
        }
        else
        {
            kitchenObj.SetKitchenObjParent(player);
        }
    }

    public Transform GetKitchenObjFollowTransform()
    {
        return CounterTopPoint;
    }

    public void SetKitchenObject(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;
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
}
