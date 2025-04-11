using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPiece : NetworkBehaviour
{
    public void Awake()
    {
        Debug.Log("dve");
    }
    /*public override void OnNetworkSpawn()
    {
        Debug.Log("[BoardManager] OnNetworkSpawn triggered");
        if (IsServer)
        {
            Debug.Log("[BoardManager] IsServer is TRUE — subscribing and starting game");
            GameManager.Instance.StartNewGame();

        }
    }*/


}

