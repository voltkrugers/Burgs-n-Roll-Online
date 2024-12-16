
using UnityEngine;


[CreateAssetMenu(fileName = "FryingObj", menuName = "Scriptable Objects/FryingObj")]
public class FryingObjSO : ScriptableObject
{
    public KitchenObjSO input;
    public KitchenObjSO output;
    public float fryingTimerMax;
}
