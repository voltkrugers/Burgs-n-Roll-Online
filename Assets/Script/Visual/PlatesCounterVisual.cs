using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> ListPlatesCounterVisual;

    private void Awake()
    {
        ListPlatesCounterVisual = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemove += PlatesCounter_OnPlateRemove;
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float PlateOffsetY = .1f;
        plateVisualTransform.localPosition = new Vector3(0, PlateOffsetY * ListPlatesCounterVisual.Count, 0);
        
        ListPlatesCounterVisual.Add(plateVisualTransform.gameObject);
    }
    private void PlatesCounter_OnPlateRemove(object sender, System.EventArgs e)
    {
        GameObject plateGameObject = ListPlatesCounterVisual[ListPlatesCounterVisual.Count - 1];
        ListPlatesCounterVisual.Remove(plateGameObject);
        Destroy(plateGameObject);

    }
}
