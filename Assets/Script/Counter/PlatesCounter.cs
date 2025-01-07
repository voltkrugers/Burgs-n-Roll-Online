using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned; 
    public event EventHandler OnPlateRemove; 
    
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

    public override void Interact(CharacterController Player)
    {
        if (!Player.HasKitchenObj())
        {
            if (platesSpawnedAmout>0)
            {
                platesSpawnedAmout--;

                KitchenObj.SpawnKitchenObj(plateKitchenObjectSo, Player);
                
                OnPlateRemove?.Invoke(this,EventArgs.Empty);
            }
        }
    }
}
