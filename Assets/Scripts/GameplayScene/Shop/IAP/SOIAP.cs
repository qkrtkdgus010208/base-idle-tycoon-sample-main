using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/IAP Data/IAP Data")]
public class SOIAP : ScriptableObject
{
    /// <summary>
    /// IAP item id
    /// Reference to google console
    /// </summary>
    [Header("Required ID and Type")]
    [SerializeField] string inAppPurchaseID;

    /// <summary>
    /// IAP item id
    /// Reference to google console
    /// </summary>
    public string InAppPurchaseID => inAppPurchaseID;

    /// <summary>
    /// IAP item type
    /// (consumable, non consumable, subscription)
    /// </summary>
    [SerializeField] ProductType inAppProductType;

    /// <summary>
    /// IAP item type
    /// (consumable, non consumable, subscription)
    /// </summary>
    public ProductType InAppProductType => inAppProductType;

    /// <summary>
    /// IAP item title
    /// </summary>
    [SerializeField] private string inAppPurchaseTitle;

    /// <summary>
    /// IAP item title
    /// </summary>
    public string InAppPurchaseTitle => inAppPurchaseTitle;

    /// <summary>
    /// IAP item icon
    /// </summary>
    [Space(15)] 
    [SerializeField] private Sprite _IAPIcon;

    /// <summary>
    /// IAP item icon
    /// </summary>
    public Sprite IAPIcon => _IAPIcon;


    /// <summary>
    /// IAP item description
    /// </summary>
    private string inAppPurchaseDescription;

    /// <summary>
    /// IAP item description
    /// </summary>
    public string InAppPurchaseDescription
    {
        get => inAppPurchaseDescription;
        set => inAppPurchaseDescription = value;
    }

    /// <summary>
    /// IAP item type
    /// (consumable, non consumable, subscription)
    /// </summary>
    private string inAppPurchaseType;

    /// <summary>
    /// IAP item type
    /// (consumable, non consumable, subscription)
    /// </summary>
    public string InAppPurchaseType
    {
        get => inAppPurchaseType;
        set => inAppPurchaseType = value;
    }

    /// <summary>
    /// IAP item price in string
    /// </summary>
    private string inAppPurchasePricesString;

    /// <summary>
    /// IAP item price in string
    /// </summary>
    public string InAppPurchasePricesString
    {
        get => inAppPurchasePricesString;
        set => inAppPurchasePricesString = value;
    }

    /// <summary>
    /// is item owned state
    /// for non consumable item
    /// </summary>
    private bool inAppPurchaseIsOwned;

    /// <summary>
    /// is item owned state
    /// for non consumable item
    /// </summary>
    public bool InAppPurchaseIsOwned
    {
        get => inAppPurchaseIsOwned;
        set => inAppPurchaseIsOwned = value;
    }

}
