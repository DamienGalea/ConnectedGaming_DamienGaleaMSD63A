using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkImageApply : NetworkBehaviour
{
    public RawImage rawImagePlayer0;
    public RawImage rawImagePlayer1;

    private Dictionary<ulong, byte[]> savedProfileImages = new();

    public void Start()
    {
        GameObject target = GameObject.Find("PlayerProfile");
        rawImagePlayer0 = target.GetComponent<RawImage>();

        GameObject target2 = GameObject.Find("Player2Profile");
        rawImagePlayer1 = target2.GetComponent<RawImage>();

    }

    private void OnEnable()
    {
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only the server should send stored images
        if (!IsServer) return;

        foreach (var kvp in savedProfileImages)
        {
            ApplyProfileImageClientRpc(kvp.Value, kvp.Key);
        }
    }

    // Called by local player when they want to set their profile image
    public void SetProfileImageLocallyAndSync(string fileName)
    {
        Debug.Log($"IsSpawned: {IsSpawned}");
        string path = Path.Combine(Application.persistentDataPath, fileName);
        
        if (!File.Exists(path))
        {
            Debug.LogWarning("File not found: " + path);
            return;
        }

        Texture2D original = new Texture2D(2, 2);
        original.LoadImage(File.ReadAllBytes(path));

        Texture2D resized = ResizeTexture(original, 128, 128); // Resize to 128x128
        byte[] imageBytes = resized.EncodeToPNG(); // Compress image to PNG

        Debug.Log($"Compressed image size: {imageBytes.Length} bytes");

        ApplyTexture(imageBytes, NetworkManager.Singleton.LocalClientId);
        ApplyTexture(imageBytes, NetworkManager.Singleton.LocalClientId);

        if (IsClient)
        {
            SendProfileImageToServerRpc(imageBytes, NetworkManager.Singleton.LocalClientId);
        }
    }





    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void SendProfileImageToServerRpc(byte[] imageData, ulong senderClientId)
    {
        savedProfileImages[senderClientId] = imageData;

        Debug.Log($"🧪 Attempting ServerRpc call. IsClient: {IsClient}, IsHost: {IsHost}, IsServer: {IsServer}");
        // Broadcast to all clients (including host)
        ApplyProfileImageClientRpc(imageData, senderClientId);
    }

    [ClientRpc]
    private void ApplyProfileImageClientRpc(byte[] imageData, ulong targetClientId)
    {
       

        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} received image from {targetClientId}");

        ApplyTexture(imageData, targetClientId);
    }

    private void ApplyTexture(byte[] imageData, ulong clientId)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        if (clientId == 0 && rawImagePlayer0 != null)
        {
            rawImagePlayer0.texture = texture;
            Debug.Log("Set Player 0's profile image");
        }
        else if (clientId == 1 && rawImagePlayer1 != null)
        {
            rawImagePlayer1.texture = texture;
            Debug.Log("Set Player 1's profile image");
        }
        else
        {
            Debug.LogWarning("Unknown client ID: " + clientId);
        }
    }

    


    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}
