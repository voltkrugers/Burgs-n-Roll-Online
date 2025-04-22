using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class HostDisconnectUi : MonoBehaviour
{
    [SerializeField] private Button _playAgainButton;

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnclientDisconnectCallback;
        Hide();
    }

    private void NetworkManager_OnclientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Show();
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

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnclientDisconnectCallback;
    }
}
