using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI RecipesDeliveredText;
     

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();
            RecipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullrecipes().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
