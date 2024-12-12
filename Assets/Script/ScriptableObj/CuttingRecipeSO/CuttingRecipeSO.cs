using UnityEngine;

[CreateAssetMenu(fileName = "CuttingRecipeSO", menuName = "Scriptable Objects/CuttingRecipeSO")]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjSO input;
    public KitchenObjSO output;
}
