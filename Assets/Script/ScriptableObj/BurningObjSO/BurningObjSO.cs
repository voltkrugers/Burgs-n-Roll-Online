
using UnityEngine;


[CreateAssetMenu(fileName = "BurningObj", menuName = "Scriptable Objects/BurningObj")]
public class BurningObjSO : ScriptableObject
{
    public KitchenObjSO input;
    public KitchenObjSO output;
    public float BurningTimerMax;
}
