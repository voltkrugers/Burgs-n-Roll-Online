using System;
using UnityEngine;
using UnityEngine.UI;

public class GameClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;


    private void Update()
    {
        timerImage.fillAmount = KitchenGameManager.Instance.GetTimerNormalized();
    }
}
