
using System.Collections;
using System.IO;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemPurchase: MonoBehaviour
{
    public StoreItem Item;
    public PriceManager priceManager;
    public Button SetBtn;
    public TMP_Text PurchaseText;

    public void Start()
    {
        GameObject target = GameObject.Find("Purchase");
        PurchaseText = target.GetComponent<TMP_Text>();

        SetBtn.interactable = false;
        priceManager = FindObjectOfType<PriceManager>();

    }
    public void DownloadItem()
    {
        if (priceManager.PurchaseItem(Item.Price))
        {
            //GetComponent<Button>().enabled = false;
            // GetComponent<Animator>().SetTrigger("Disabled");

            if (Item == null || string.IsNullOrEmpty(Item.ThumbnailUrl))
            {
                Debug.LogError("❌ Missing item or URL.");
                return;
            }

            string internalUrl = Item.ThumbnailUrl; // e.g., "dlc/profile1.png"
            string filename = Item.Name.Replace(" ", "") + Path.GetExtension(internalUrl);
            string filepath = Path.Combine(Application.persistentDataPath, filename);

            Debug.Log($"⬇️ Downloading from Firebase: {internalUrl} → {filepath}");

            FirebaseStorageManager.Instance.DownloadToFile(internalUrl, filepath);

            SetBtn.interactable = true;
            SetBtn.GetComponent<SetImageBtn>().downloadedFileName = filename;


            if (NetworkText.Instance != null && NetworkManager.Singleton != null)
            {
                NetworkText.Instance.ShowPurchaseMessageClientRpc(NetworkManager.Singleton.LocalClientId, Item.Name);
            }

           

            if (FirestoreService.Instance != null) {
                FirestoreService.Instance.LogAction($"{Item.Name} was downloaded");
            }
            else
            {
                Debug.LogError("FirestoreService null");
            }

            PurchaseText.text = "Item has been purchased";
            StartCoroutine(ResetText(PurchaseText));
        }
        else
        {
            PurchaseText.text = "Insuffient balance!!";
            Debug.LogWarning("Insuffient balance!!");
        }

    }

    private IEnumerator ResetText(TMP_Text textField)
    {
        yield return new WaitForSeconds(5f);
        textField.text = "";
    }
}