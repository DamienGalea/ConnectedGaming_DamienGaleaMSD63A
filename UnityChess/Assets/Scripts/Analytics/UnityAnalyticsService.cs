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

    public void RecordPlayerStepOverEvent(int tileId)
    {
        PlayerStepOver myEvent = new PlayerStepOver
        {
            GameObjectId = tileId,
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}
