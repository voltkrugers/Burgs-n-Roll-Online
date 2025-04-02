using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    public static DeliveryManager Instance { get ; private set; }
    
    [SerializeField] private RecipeListSO recipeListSo;
    
    private List<RecipeSO> WaitingRecipeSOList;
    private float SpawnRecipeTimer  = 4f;
    private float SpawnRecipeTimerMax = 4f;
    private int WaitingRecipesMax = 4;
    private int SuccessfulRecipe = 0;

    private void Awake()
    {
        Instance = this;
        
        WaitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        SpawnRecipeTimer -= Time.deltaTime;
        if (SpawnRecipeTimer <= 0f)
        {
            SpawnRecipeTimer = SpawnRecipeTimerMax;
            if (KitchenGameManager.Instance.IsGamePlaying() && WaitingRecipeSOList.Count<WaitingRecipesMax)
            {
                int waitRecipeSOIndex = Random.Range(0, recipeListSo.RecipeSoList.Count);
                
                
                SpawnNewWaitingRecipeClientRpc(waitRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int WaitRecipeSoIndex)
    {
        RecipeSO WaitRecipeSo = recipeListSo.RecipeSoList[WaitRecipeSoIndex];
        WaitingRecipeSOList.Add(WaitRecipeSo);
                
        OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObjet plateKitchenObjet)
    {
        for (int i = 0; i < WaitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSo = WaitingRecipeSOList[i];
            if (waitingRecipeSo.KitchenObjSoList.Count == plateKitchenObjet.GetKitchenObjectSoList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjSO recipeKitchenObjSo in waitingRecipeSo.KitchenObjSoList)
                {
                    bool IngredientFound = false;
                    foreach (KitchenObjSO plateKitchenObjSo in plateKitchenObjet.GetKitchenObjectSoList())
                    {
                        if (plateKitchenObjSo == recipeKitchenObjSo)
                        {
                            IngredientFound = true;
                            break;
                        }
                    }
                    if (!IngredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
                
            }
        }

        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this,EventArgs.Empty);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }
    
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int i)
    {
        SuccessfulRecipe++;
        OnRecipeCompleted?.Invoke(this,EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        WaitingRecipeSOList.RemoveAt(i);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return WaitingRecipeSOList;
    }

    public int GetSuccessfullrecipes()
    {
        return SuccessfulRecipe; 
    }
}
