using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSo_GameObject
    {
        public KitchenObjSO KitchenObjSo;
        public GameObject GameObject;
    }
    
    [SerializeField] private PlateKitchenObjet plateKitchenObjet;
    [SerializeField] private List<KitchenObjectSo_GameObject> kitchenObjectSoGameObjectList;

    private void Start()
    {
        plateKitchenObjet.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjectList)
        {
            kitchenObjectSoGameObject.GameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObjet.OnIngredientAddedEventArgs e)
    {
        foreach (var kitchenObjectSoGameObject in kitchenObjectSoGameObjectList)
        {
            if (kitchenObjectSoGameObject.KitchenObjSo == e.KitchenObjSo)
            {
                kitchenObjectSoGameObject.GameObject.SetActive(true);
            }
        }
    }
}
