using UnityEngine.Purchasing;

public enum ItemType
{
    Consumables,
    NonConsumables,
    Subscription
}

public enum ItemPrices
{
    
}

public class IAPSerialize
{
    public string productID;
    public ItemType itemType;
    
     
}
