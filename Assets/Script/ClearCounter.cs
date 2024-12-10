using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjSO ObjSo;
    [SerializeField] private Transform CounterTopPoint;
    
    public void Interact()
    {
        Transform objTransform = Instantiate(ObjSo.prefab, CounterTopPoint);
        objTransform.localPosition = Vector3.zero;

        objTransform.GetComponent<KitchenObj>().GetKitchenObjSo();
    }
}
