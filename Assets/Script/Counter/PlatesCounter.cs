using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned; 
    
    [SerializeField] private KitchenObjSO plateKitchenObjectSo;
    
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmout;
    private int platesSpawnedAmoutMax = 4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer> spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if (platesSpawnedAmout<platesSpawnedAmoutMax)
            {
                platesSpawnedAmout++;
                OnPlateSpawned?.Invoke(this,EventArgs.Empty);
            }
            
        }
    }
}
