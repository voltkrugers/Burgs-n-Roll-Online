
using UnityEngine;


[CreateAssetMenu(fileName = "KitchenOBJ", menuName = "Scriptable Objects/KitchenOBJ")]
public class KitchenObjSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objName;
}
