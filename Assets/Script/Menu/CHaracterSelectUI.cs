using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CHaracterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button ReadyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener (() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });
        ReadyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
}
