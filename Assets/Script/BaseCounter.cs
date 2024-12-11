using UnityEngine;

public class BaseCounter : MonoBehaviour , IKitchenObjParent
{
    
    [SerializeField] private Transform CounterTopPoint;
    
    protected KitchenObj kitchenObj;
    public virtual void Interact(CharacterController player)
    {
        
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
