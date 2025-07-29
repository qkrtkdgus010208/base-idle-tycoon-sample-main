using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;


public class CodeIAPManager : MonoBehaviour, IDetailedStoreListener
{
    /// <summary>
    /// Environment data
    /// </summary>
    private const string ENVIRONMENT = "production";

    /// <summary>
    /// Singleton
    /// </summary>
    private static CodeIAPManager _instance;

    /// <summary>
    /// Singleton
    /// </summary>
    public static CodeIAPManager Instance = _instance;

    /// <summary>
    /// Initialize state
    /// </summary>
    private bool _initialInitialized = false;

    /// <summary>
    /// debug state
    /// </summary>
    private bool allowDebug = true;

    /// <summary>
    /// iap data container
    /// </summary>
    [SerializeField] private SOIAPCollections _listOfDataIAP;

    /// <summary>
    /// store controller
    /// </summary>
    private IStoreController storeController;


    /// <summary>
    /// Initialize state
    /// </summary>
    public bool InitialInitialized
    {
        get => _initialInitialized;
        set => _initialInitialized = value;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        StaticEventIAP.InitializeIAPManager = SetUpUnityCloudGames;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// required for IAP
    /// initialize first before Initialize IAP
    /// </summary>
    private async void SetUpUnityCloudGames()
    {
        if (_initialInitialized)
            return;

        try
        {
            var options = new InitializationOptions().SetEnvironmentName(ENVIRONMENT);
            await UnityServices.InitializeAsync(options);
            _initialInitialized = true;
            SetUpUnityIAP();
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            _initialInitialized = false;
        }
    }

    /// <summary>
    /// Initialize unity iap
    /// </summary>
    private void SetUpUnityIAP()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var iapItem in _listOfDataIAP.IAPItemCollections)
            builder.AddProduct(iapItem.InAppPurchaseID, iapItem.InAppProductType);

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// IStoreListener callback after unity iap initialize successful
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;

        foreach (var iapItem in _listOfDataIAP.IAPItemCollections) // initialize all iap item
        {
            var product = storeController.products.WithID(iapItem.InAppPurchaseID);
            var productMetadata = product.metadata;

            iapItem.InAppPurchaseDescription = productMetadata.localizedDescription;
            iapItem.InAppPurchasePricesString = productMetadata.localizedPriceString;

            #if UNITY_EDITOR
                        continue;
            #endif

            if (product.hasReceipt) // purchase restoration
            {
                if (iapItem.InAppProductType == ProductType.NonConsumable)
                    iapItem.InAppPurchaseIsOwned = true;

                else if (iapItem.InAppProductType == ProductType.Subscription)
                    iapItem.InAppPurchaseIsOwned =
                        new SubscriptionManager(product, null).getSubscriptionInfo().isSubscribed() == Result.True;
            }
        }

        StaticEventIAP.SetPurchaseIAPItem = SetupPurchasingItem;

        OnDebug($"On Initialized : {controller} : {extensions} ", LogType.Log);

#if UNITY_EDITOR || UNITY_ANDROID

#else 
                extensions.GetExtension<IAppleExtensions> ().RestoreTransactions ((result, error) => {
            if (result) {
                StaticEventIAP.OnRestorePurchasedAppleSuccess?.Invoke();
                OnDebug($"On Initialized : {result} ",LogType.Log);
            } else {
                StaticEventIAP.OnRestorePurchasedApplefailfed?.Invoke();
                OnDebug($"On Initialized : {result} : {error} ",LogType.Error);
            }
        });
#endif

    }

    /// <summary>
    /// Initialize failed
    /// </summary>
    /// <param name="error"> error reason </param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        StaticEventIAP.onInitiateFailed?.Invoke();
        OnDebug($"INITIALIZE FAILED : {error}",LogType.Error);
    }

    /// <summary>
    /// Initialize failed
    /// </summary>
    /// <param name="error"> error reason </param>
    /// <param name="message"> error message </param>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        StaticEventIAP.onInitiateFailed?.Invoke();
        OnDebug($"INITIALIZE FAILED : {error} : {message}",LogType.Error);
    }

    /// <summary>
    /// IStoreListener callback after purchased iap item
    /// </summary>
    /// <param name="purchaseEvent"></param>
    /// <returns> purchasing result </returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        PurchaseProcessingResult result = PurchaseProcessingResult.Pending;
        
        if (_listOfDataIAP.GetIAPItemByID(purchaseEvent.purchasedProduct.definition.id) != null)
            if (purchaseEvent.purchasedProduct.availableToPurchase)
                result = BackndPurchasing.ProcessPurchase(purchaseEvent, StaticEventIAP.onPurchaseSuccess);

        return result;
    }

    /// <summary>
    /// On purchase failed
    /// </summary>
    /// <param name="product"> iap product </param>
    /// <param name="failureReason"> failure reason </param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        StaticEventIAP.onPurchaseFailed?.Invoke(product.definition.id);
        OnDebug($"PURCHASING FAILED : \n product  {product.definition.id}  : \n reason : {failureReason} ",LogType.Error);
    }

    /// <summary>
    /// On purchase failed
    /// </summary>
    /// <param name="product"> iap product </param>
    /// <param name="failureDescription"> failed message </param>
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        StaticEventIAP.onPurchaseFailed?.Invoke(product.definition.id);
        OnDebug($"PURCHASING FAILED : \n product  {product.definition.id}  : \n reason : {failureDescription} ",LogType.Error);
    }
    
    private void OnDebug(string id,LogType type)
    {
        if (!allowDebug) return;
        switch (type)
        {
            case LogType.Log:
                Debug.Log(id);
                break;
            case LogType.Error:
                Debug.LogError(id);
                break;
            case LogType.Assert:
            case LogType.Warning:
                Debug.LogWarning(id);
                break;
            case LogType.Exception:
                Debug.LogError(id);
                break;
            default:
                Debug.LogError(id);
                break;
        }
    }

    /// <summary>
    /// Set purchasing iap item
    /// </summary>
    /// <param name="iapID"> iap id that will purchase </param>
    public void SetupPurchasingItem(string iapID)
    {
        if (!InitialInitialized)
        {
            Debug.LogError("Error: IAP Manager Not Initialized");
            return;
        }

        var product = storeController.products.WithID(iapID);

        if (product is { availableToPurchase: true })
        {
            #if UNITY_EDITOR
            StaticEventIAP.onPurchaseSuccess?.Invoke(iapID);
            return;
            #endif

            storeController.InitiatePurchase(product);
        }
        else
            StaticEventIAP.onPurchaseFailed?.Invoke(product.definition.id);
    }
}
