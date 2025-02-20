using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject visualGameObject;
    void Start()
    {
        if (CharacterController.LocalInstance != null)
        {
            CharacterController.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            CharacterController.OnAnyPlayerSpawned += CharacterController_OnAnyPlayerSpawned;
        }
        
    }

    private void CharacterController_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (CharacterController.LocalInstance != null)
        {
            CharacterController.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            CharacterController.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, CharacterController.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        visualGameObject.SetActive(false);
    }

    private void Show()
    {
        visualGameObject.SetActive(true);
    }
}
