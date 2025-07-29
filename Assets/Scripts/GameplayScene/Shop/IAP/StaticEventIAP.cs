using System;

public static class StaticEventIAP
{
    /// <summary>
    /// Initialize IAP Manager
    /// </summary>
    public static Action InitializeIAPManager;

    /// <summary>
    /// Initiate purchase for iap flow
    /// </summary>
    public static Action onInitiatePurchase;

    /// <summary>
    /// Initiate purchase failed
    /// </summary>
    public static Action onInitiateFailed;

    /// <summary>
    /// Events to set iap item to purchase
    /// </summary>
    public static Action<string> SetPurchaseIAPItem;

    /// <summary>
    /// Events after purchase success
    /// string: iap id that purchased
    /// </summary>
    public static Action<string> onPurchaseSuccess;

    /// <summary>
    /// Events after purchase failed
    /// string: iap id that purchased
    /// </summary>
    public static Action<string> onPurchaseFailed;
}
