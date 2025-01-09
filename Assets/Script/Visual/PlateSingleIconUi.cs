using UnityEngine;
using UnityEngine.UI;

public class PlateSingleIconUi : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSo(KitchenObjSO kitchenObjSo)
    {
        image.sprite = kitchenObjSo.sprite;
    }
}
