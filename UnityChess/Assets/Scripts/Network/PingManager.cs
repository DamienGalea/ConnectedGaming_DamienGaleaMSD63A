using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PingManager : NetworkBehaviour
{
    private float pingSentTime;
    private float lastPing = -1f;
    public TMP_Text pingText;
   
    private void Start()
    {
        GameObject target = GameObject.Find("Ping");
        pingText = target.GetComponent<TMP_Text>();

        
        StartCoroutine(PingLoop());
    }

    private IEnumerator PingLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (IsHost)
            {
                lastPing = 0f;
                if (pingText != null)
                    pingText.text = "Ping: 0 ms (Host)";
                Debug.Log("[PingManager] Ping: 0 ms (Host)");
            }
            else
            {
                pingSentTime = Time.time;
                SendPingToServerServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPingToServerServerRpc(ServerRpcParams rpcParams = default)
    {
        SendPingBackClientRpc(rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SendPingBackClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            lastPing = (Time.time - pingSentTime) * 1000f;

            pingText.text = $"Ping: {lastPing:F1} ms";
            Debug.Log($"[PingManager] Ping: {lastPing:F1} ms");

        }
    }
}
