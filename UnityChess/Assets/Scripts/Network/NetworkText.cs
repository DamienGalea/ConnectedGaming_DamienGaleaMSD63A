using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityChess;
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

    [ClientRpc]
    public void ShowConnectionFaliureClientRpc()
    {

        string message = $"Failed to connect to host";

        FindObjectOfType<UIManager>().ShowPurchaseMessage(message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(string fromSquare, string toSquare)
    {
        Square from = new Square(fromSquare);
        Square to = new Square(toSquare);

        if (!GameManager.Instance.game.TryGetLegalMove(from, to, out Movement move))
        {
            Debug.LogWarning($"[Server] Invalid move from {from} to {to}");
            return;
        }

        if (GameManager.Instance.TryExecuteMove(move))
        {
            NotifyTurnChangedClientRpc(GameManager.Instance.SideToMove);
        }
    }

    [ClientRpc]
    public void NotifyTurnChangedClientRpc(Side nextTurn)
    {
        BoardManager.Instance.UpdateTurnIndicators(nextTurn);
    }

    [ClientRpc]
    public void SetClientTurnClientRpc(Side side)
    {
        GameManager.Instance.SetClientSideToMove(side);
    }
}
