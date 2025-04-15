using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class UnityAnalyticsService : MonoBehaviour
{
    public static UnityAnalyticsService Instance;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void RecordPurchaseEvent (string item)
    {
        PlayerPurchase myEvent = new PlayerPurchase
        {
            item_name = item,
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        Debug.Log("Item recded");
    }
}
