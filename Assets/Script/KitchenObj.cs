using UnityEngine;

public class KitchenObj : MonoBehaviour
{

    [SerializeField] private KitchenObjSO kitchenObjSo;

    public KitchenObjSO GetKitchenObjSo()
    {
        return kitchenObjSo; 
    }
}
