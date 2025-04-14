using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkText : NetworkBehaviour
{
    public static NetworkText Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void ShowPurchaseMessageClientRpc(ulong clientId, string itemName)
    {
        string playerName = clientId == 0 ? "Player 1" : $"Player {clientId + 1}";
        string message = $"{playerName} has purchased {itemName}";

        FindObjectOfType<UIManager>().ShowPurchaseMessage(message);
    }
}
