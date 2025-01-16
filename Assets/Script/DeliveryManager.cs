using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    public static DeliveryManager Instance { get ; private set; }
    
    [SerializeField] private RecipeListSO recipeListSo;
    
    private List<RecipeSO> WaitingRecipeSOList;
    private float SpawnRecipeTimer ;
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
        SpawnRecipeTimer -= Time.deltaTime;
        if (SpawnRecipeTimer <= 0f)
        {
            SpawnRecipeTimer = SpawnRecipeTimerMax;
            if (WaitingRecipeSOList.Count<WaitingRecipesMax)
            {
                RecipeSO WaitRecipeSo = recipeListSo.RecipeSoList[Random.Range(0,recipeListSo.RecipeSoList.Count)];
                
                WaitingRecipeSOList.Add(WaitRecipeSo);
                
                OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
            }
        }
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
                    SuccessfulRecipe++;
                    OnRecipeCompleted?.Invoke(this,EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    WaitingRecipeSOList.RemoveAt(i);
                    return;
                }
                
            }
        }
        OnRecipeFailed?.Invoke(this,EventArgs.Empty);
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
