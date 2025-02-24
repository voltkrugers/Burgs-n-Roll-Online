using System;
using Unity.Netcode;
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
        if (!IsServer)
        {
            return;
        }
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer> spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if (platesSpawnedAmout<platesSpawnedAmoutMax)
            {
                SpawnPlateServerRpc();
            }
            
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmout++;
        OnPlateSpawned?.Invoke(this,EventArgs.Empty);
    }

    public override void Interact(CharacterController Player)
    {
        if (!Player.HasKitchenObj())
        {
            if (platesSpawnedAmout>0)
            {

                KitchenObj.SpawnKitchenObj(plateKitchenObjectSo, Player);

                InteractLogicServerRpc();
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnedAmout--;
        OnPlateRemove?.Invoke(this,EventArgs.Empty);
    }
}
