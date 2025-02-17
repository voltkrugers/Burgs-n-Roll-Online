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
    public static KitchenObj SpawnKitchenObj(KitchenObjSO kitchenObjSo, IKitchenObjParent kitchenObjParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjSo.prefab);

        KitchenObj kitchenObj = kitchenObjectTransform.GetComponent<KitchenObj>();
        
        kitchenObj.SetKitchenObjParent(kitchenObjParent);

        return kitchenObj;
    }

    public void SetKitchenObjSo(KitchenObjSO dummyKitchenObjSo)
    {
        this.kitchenObjSo = dummyKitchenObjSo;
    }
}
