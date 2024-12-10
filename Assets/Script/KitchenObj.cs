using UnityEngine;

public class KitchenObj : MonoBehaviour
{

    [SerializeField] private KitchenObjSO kitchenObjSo;

    private IKitchenObjParent kitchenObjParent;

    public KitchenObjSO GetKitchenObjSo()
    {
        return kitchenObjSo; 
    }

    public void SetKitchenObjParent(IKitchenObjParent KitchenObjParent)
    {
        if (this.kitchenObjParent != null)
        {
            this.kitchenObjParent.ClearKitchenObject();
        }
        
        this.kitchenObjParent = KitchenObjParent;

        if (KitchenObjParent.HasKitchenObj())
        {
            
        }
        
        KitchenObjParent.SetKitchenObject(this);
        transform.parent = KitchenObjParent.GetKitchenObjFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjParent GetKitchenObjParent()
    {
        return kitchenObjParent;
    }
}
