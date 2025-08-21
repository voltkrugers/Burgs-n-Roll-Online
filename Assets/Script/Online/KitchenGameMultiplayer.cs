using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public const int Max_Player_Amount = 4;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static bool playMultiplayer;

    public static KitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjListSO kitchenObjListSo;
    [SerializeField] private List<Color> playerColorList;

    // MODIF: liste accessible uniquement par le serveur
    private NetworkList<PlayerData> playerDataNetworkList;

    private string playerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(
            PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER,
            "PlayerName" + UnityEngine.Random.Range(100, 1000)
        );

        // MODIF: initialisation sécurisée de la liste
        playerDataNetworkList = new NetworkList<PlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;

        if (IsServer)
        {
            playerDataNetworkList.Clear();
            Debug.Log("KitchenGameMultiplayer: OnNetworkSpawn -> reset player list");
        }
    }

    public override void OnNetworkDespawn()
    {
        playerDataNetworkList.OnListChanged -= playerDataNetworkList_OnListChanged;
    }

    private void Start()
    {
        if (!playMultiplayer)
        {
            ConfigureTransportForLocal();
            StartHost();
            Loader.LoadNetwork(Loader.Scene.Game);
        }
    }

    private void ConfigureTransportForLocal()
    {
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777);
    }

    public string GetPlayerName() => playerName;

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changevent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        // MODIF: on nettoie avant de relancer un host
        if (NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();

        NetworkManager.Singleton.ConnectionApprovalCallback = NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
                return; // MODIF: on sort direct
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        // MODIF: on évite d’ajouter deux fois le même joueur
        if (GetPlayerDataIndexFromClientId(clientId) != -1) return;

        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId()
        });

        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.Game.ToString())
        {
            res.Approved = false;
            res.Reason = "Game already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= Max_Player_Amount)
        {
            res.Approved = false;
            res.Reason = "Game full";
            return;
        }
        res.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams rpcParams = default)
    {
        int index = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        if (index == -1) return; // MODIF: protection

        var playerData = playerDataNetworkList[index];
        playerData.PlayerName = playerName;
        playerDataNetworkList[index] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams rpcParams = default)
    {
        int index = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        if (index == -1) return; // MODIF: protection

        var playerData = playerDataNetworkList[index];
        playerData.PlayerId = playerId;
        playerDataNetworkList[index] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    // --- KitchenObj methods (inchangées sauf indentation) ---

    public void SpawnKitchenObj(KitchenObjSO kitchenObjSo, IKitchenObjParent kitchenObjParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjSo), kitchenObjParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjSoIndex, NetworkObjectReference kitchenObjParentNetworkObjectReference)
    {
        KitchenObjSO kitchenObjSo = GetKitchenObjectSOFromIndex(kitchenObjSoIndex);
        kitchenObjParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);

        IKitchenObjParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjParent>();

        if (kitchenObjectParent.HasKitchenObj()) return;

        Transform kitchenObjectTransform = Instantiate(kitchenObjSo.prefab);
        NetworkObject kitchenObjecNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjecNetworkObject.Spawn(true);

        IKitchenObjParent kitchenObjParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjParent>();
        KitchenObj kitchenObject = kitchenObjectTransform.GetComponent<KitchenObj>();
        kitchenObject.SetKitchenObjParent(kitchenObjParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjSO kitchenObjSo) => kitchenObjListSo.KitchenObjSoList.IndexOf(kitchenObjSo);
    public KitchenObjSO GetKitchenObjectSOFromIndex(int index) => kitchenObjListSo.KitchenObjSoList[index];

    public void DestroyKitchenObject(KitchenObj kitchenObj)
    {
        DestroyKitchenObjectServerRpc(kitchenObj.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        if (kitchenObjectNetworkObject == null) return;

        KitchenObj kitchenObj = kitchenObjectNetworkObject.GetComponent<KitchenObj>();
        ClearKitchenObjOnParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObj.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObj kitchenObj = kitchenObjectNetworkObject.GetComponent<KitchenObj>();
        kitchenObj.ClearKitchenObjOnParent();
    }

    // --- PlayerData utils ---

    public bool IsPlayerIndexConnected(int index) => index < playerDataNetworkList.Count;

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataNetworkList)
            if (playerData.clientId == clientId)
                return playerData;

        return default;
    }

    public PlayerData GetPlayerData() => GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    public PlayerData GetPlayerDataFromPlayerIndex(int index) => playerDataNetworkList[index];

    public Color GetPlayerColor(int colorId) => playerColorList[colorId];

    public void ChangePlayerColor(int colorId) => ChangePlayerColorServerRpc(colorId);

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams rpcParams = default)
    {
        if (!IsColorAvailable(colorId)) return;

        int index = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        if (index == -1) return; // MODIF: sécurité

        var playerData = playerDataNetworkList[index];
        playerData.colorId = colorId;
        playerDataNetworkList[index] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDataNetworkList)
            if (playerData.colorId == colorId)
                return false;
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
            if (IsColorAvailable(i))
                return i;
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
