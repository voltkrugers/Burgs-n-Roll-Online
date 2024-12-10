using UnityEngine;

public interface IKitchenObjParent
{
    public Transform GetKitchenObjFollowTransform();


    public void SetKitchenObject(KitchenObj kitchenObj);


    public KitchenObj GetKitchenObj();


    public void ClearKitchenObject();


    public bool HasKitchenObj();

}
