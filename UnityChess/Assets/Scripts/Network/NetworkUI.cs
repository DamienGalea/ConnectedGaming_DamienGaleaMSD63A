using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using UnityChess;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button HostButton;
    private UnityTransport transport;

    private Dictionary<ulong, string> playerSides = new Dictionary<ulong, string>();
    public static ulong BlackPlayerClientId = ulong.MaxValue;

   

    private void Awake()
    {
        ServerButton.onClick.AddListener(StartServer);
        ClientButton.onClick.AddListener(StartClient);
        HostButton.onClick.AddListener(StartHost);
    }

    void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
           
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
           
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }
    private void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log($"Server started listening on {transport.ConnectionData.ServerListenAddress} and port {transport.ConnectionData.Port}");
        CheckIfRunningLocally();
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        BoardManager.Instance.SpawnTiles();
        WaitForClientAndStartGame();


    }

    private IEnumerator WaitForClientAndStartGame()
    {
        Debug.Log("[NetworkUI] Waiting for client before starting game...");

        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count >= 2 && BlackPlayerClientId != ulong.MaxValue);

        BoardManager.Instance.SpawnTiles();
        GameManager.Instance.StartNewGame();

        Debug.Log("[NetworkUI] Game started after both players connected.");
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
       
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log($"Server started listening on {transport.ConnectionData.ServerListenAddress} and port {transport.ConnectionData.Port}");
        CheckIfRunningLocally();
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        BoardManager.Instance.SpawnTiles();
        GameManager.Instance.StartNewGame();
    }

    private void CheckIfRunningLocally()
    {
        if (transport.ConnectionData.ServerListenAddress == "127.0.0.1")
        {
            Debug.LogWarning("Server is listening locally (127.0.0.1) ONLY!");
        }
    }

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Game is running in build mode");
        }
        else if (Application.isEditor)
        {
            Debug.Log("Game is running in Unity Editor");
        }
        else if (Application.platform == RuntimePlatform.LinuxServer && Application.isBatchMode &&
                  !Application.isEditor)
        {
            Debug.Log("Game is running on Linux Dedicated Server");
        }

        if (NetworkManager.Singleton != null)
        {
            Debug.Log($"UTP working with IP:{transport.ConnectionData.Address} and Port:{transport.ConnectionData.Port}");
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"[Netcode] Client connected: {clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"[SERVER] Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");

            // Assign the first non-host client as the black player
            if (clientId != NetworkManager.ServerClientId && BlackPlayerClientId == ulong.MaxValue)
            {
                BlackPlayerClientId = clientId;
                Debug.Log($"[NetworkUI] Assigned black pieces to client: {clientId}");
            }
        }
        else
        {
            Debug.Log("[CLIENT] Connected to host!");
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.LogWarning($"[Netcode] Client disconnected: {clientId}");
    }

    private IEnumerator MonitorClientConnection()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (!NetworkManager.Singleton.IsConnectedClient && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("Failed to connect to server.");
            NetworkText.Instance.ShowConnectionFaliureClientRpc();
            NetworkManager.Singleton.Shutdown();
        }
    }


}
