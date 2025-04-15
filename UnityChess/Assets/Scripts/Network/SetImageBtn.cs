using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetImageBtn : MonoBehaviour
{
    public string downloadedFileName = ""; // Set this from your download logic
    public RawImage targetRawImage;
    public void Awake()
    {
        GameObject target = GameObject.Find("PlayerProfile");
        targetRawImage = target.GetComponent<RawImage>();
    }
    public void OnSetButtonClick()
    {
        var manager = FindObjectOfType<NetworkImageApply>();
        if (manager != null)
        {
            manager.SetProfileImageLocallyAndSync(downloadedFileName);

            UIManager.Instance.ShowPurchaseMessage("Player is using a new profile picture");
        }
        else
        {
            //Debug.LogError("No NetworkProfileImageManager found in scene!");
            SetProfileImageLocally(downloadedFileName, targetRawImage);
        }


    }

    public void SetProfileImageLocally(string fileName, RawImage rawImage)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"❌ File not found: {path}");
            return;
        }

        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            rawImage.texture = texture;
            Debug.Log($"✅ Profile image set locally from: {path}");
        }
        else
        {
            Debug.LogWarning("❌ Failed to load image data.");
        }
    }
}
