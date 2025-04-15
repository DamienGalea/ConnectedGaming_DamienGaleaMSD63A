public class PlayerPurchase : Unity.Services.Analytics.Event
{
    public PlayerPurchase() : base("PlayerPurchase") { }
    public string item_name { set { SetParameter("item_name", value); } }
}
