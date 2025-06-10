using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CHaracterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button ReadyButton;
    [SerializeField] private TextMeshProUGUI LobbyNameText;
    [SerializeField] private TextMeshProUGUI LobbyCodeText;

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

    private void Start()
    {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        LobbyNameText.text = "Lobby Name: " + lobby.Name;
        LobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
