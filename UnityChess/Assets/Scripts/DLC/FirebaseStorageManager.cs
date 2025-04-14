using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine.UI;
using System;

public class FirebaseStorageManager : MonoBehaviour
{
    private FirebaseStorage _storage;
    private StorageReference _storageRef;
    public static FirebaseStorageManager Instance;
    public GameObject StoreItemPrefab;
    [SerializeField] private StoreItemsSO storeItemsSO;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        _storage = FirebaseStorage.DefaultInstance;
        // Create a storage reference from our storage service
        _storageRef = _storage.RootReference;
        //Load the Store
        DownloadToByteArray("StoreItems.xml",FirebaseStorageManager.DownloadType.MANIFEST);

        
    }
    
    public void UploadFileToStorage(string path, string filename)
    {
        StorageReference storeItemsRef = _storageRef.Child(filename);
        storeItemsRef.PutFileAsync(path)
            .ContinueWith((Task<StorageMetadata> task) => {
                if (task.IsFaulted || task.IsCanceled) {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                }
                else {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                }
            });
    }

    public enum DownloadType {IMAGE = 0,MANIFEST = 1}
    public void DownloadToByteArray(string filename, DownloadType downloadType, StoreItem storeItem = null)
    {
        StorageReference storeItemsRef = _storageRef.Child(filename);
        Debug.Log($"Downloading from Firebase: {filename}");
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 5 * 1024 * 1024; 
        storeItemsRef.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }else {
                byte[] fileContents = task.Result;
                switch (downloadType)
                {
                    case DownloadType.IMAGE:
                        StartCoroutine(LoadImageContainer(fileContents,storeItem));
                        //storeItem.ThumbnailImage = LoadSpriteFromBytes(byteArr); // 👈 Set the Sprite from Firebase
                        //CreateRuntimeShopItem(storeItem);
                        break;
                    case DownloadType.MANIFEST:
                        StartCoroutine(LoadManifest((fileContents)));
                        break;
                }
                Debug.Log("Finished downloading!");
            }
        });
    }

    private Sprite LoadSpriteFromBytes(byte[] imageData)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    public void CreateLocalStoreItem(StoreItem storeItem)
    {
        try
        {
            var parentGO = GameObject.Find("ShopItems");
            GameObject newStoreitem = Instantiate(StoreItemPrefab, parentGO.transform);
            Debug.Log($"🟢 Instantiated prefab for {storeItem.ID}");

            int childCount = newStoreitem.transform.childCount;
            Debug.Log($"👶 Child count: {childCount}");

            var imgObj = newStoreitem.transform.GetChild(0);
            var nameObj = newStoreitem.transform.GetChild(1);
            var priceObj = newStoreitem.transform.GetChild(2);

            var image = imgObj.GetComponent<Image>();
            var nameText = nameObj.GetComponent<TMP_Text>();
            var priceText = priceObj.GetComponent<TMP_Text>();

            if (image == null) Debug.LogError("❌ Missing Image component on child 0");
            if (nameText == null) Debug.LogError("❌ Missing TMP_Text on child 1");
            if (priceText == null) Debug.LogError("❌ Missing TMP_Text on child 2");

            if (storeItem.ThumbnailImage == null)
            {
                Debug.LogWarning($"⚠️ ThumbnailImage is null for {storeItem.Name}");
            }
            else
            {
                image.sprite = storeItem.ThumbnailImage;
            }

            nameText.text = storeItem.Name;
            priceText.text = storeItem.Price.ToString();

            newStoreitem.GetComponent<ItemPurchase>().Item = storeItem;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Failed to create store item '{storeItem.ID}': {e.Message}");
        }
    }

    IEnumerator LoadManifest(byte[] byteArr)
    {
        string manifestData = System.Text.Encoding.UTF8.GetString(byteArr);
        string[] lines = manifestData.Split('\n');
        string remainingData = string.Join("\n", lines.Skip(1));
        
        XDocument manifest = XDocument.Parse(remainingData);
        foreach (XElement element in manifest.Root.Elements())
        {
            StoreItem item = new StoreItem();
            
            
            // Extract data from each child element
            item.ID = element.Element("ID").Value;

            item.Name = element.Element("Name").Value;
            item.ThumbnailUrl = element.Element("ThumbnailUrl").Value;

            StoreItem matchedSOItem = storeItemsSO.Items.FirstOrDefault(x => x.ID == item.ID);
            if (matchedSOItem != null)
            {
                item.ThumbnailImage = matchedSOItem.ThumbnailImage;
            }
            else
            {
                Debug.LogWarning($"No match found in SO for ID: {item.ID}");
            }

            float price;
            if (float.TryParse(element.Element("Price").Value, out price))
            {
                item.Price = price;
            }
            else
            {
                Debug.LogError("Failed to parse Price for item: " + element.Element("Name").Value);
            }

            float discount;
            if (float.TryParse(element.Element("Discount").Value, out discount))
            {
                item.Discount = discount;
            }
            else
            {
                Debug.LogError("Failed to parse Discount for item: " + element.Element("Name").Value);
            }


            Debug.Log(item.ID);
            //DownloadToByteArray(item.ThumbnailUrl.Split("firebasestorage.app/")[1], DownloadType.IMAGE, item);
            CreateLocalStoreItem(item);
        }
        yield return null;
    }
    

    IEnumerator LoadImageContainer(byte[] byteArr, StoreItem storeItem)
    {
        Texture2D imageTexture = new Texture2D(1, 1);
        imageTexture.LoadImage(byteArr);
        Transform parent = GameObject.Find("ShopItems").GetComponent<Transform>();

        GameObject newStoreitem = Instantiate(StoreItemPrefab, parent);   

        newStoreitem.transform.GetChild(0).GetComponent<RawImage>().texture = imageTexture;
        newStoreitem.transform.GetChild(2).GetComponent<TMP_Text>().text = storeItem.Price.ToString();
        newStoreitem.transform.GetChild(1).GetComponent<TMP_Text>().text = storeItem.Name;
        newStoreitem.GetComponent<ItemPurchase>().Item = storeItem;
        yield return null;
    }

    public void DownloadToFile(string url, string filepath)
    {
        // Create local filesystem URL
        StorageReference storeItemsRef = _storageRef.Child(url);
        // Download to the local filesystem
        storeItemsRef.GetFileAsync(filepath).ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log($"File downloaded to: {filepath}");
            }
        });
    }
}
