using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform IconContainer;
    [SerializeField] private Transform IconTemplate;


    private void Awake()
    {
        IconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSo(RecipeSO recipeSo)
    {
        recipeNameText.text = recipeSo.RecipeName;
        foreach (Transform child in IconContainer)
        {
            if (child == IconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjSO kitchenObjSo in recipeSo.KitchenObjSoList)
        {
            Transform iconTransform = Instantiate(IconTemplate, IconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjSo.sprite;
        }
    }
}
