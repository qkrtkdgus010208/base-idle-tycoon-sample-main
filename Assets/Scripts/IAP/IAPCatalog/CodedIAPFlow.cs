using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Product = UnityEngine.Purchasing.Product;

/// <summary>
/// Custom IAP (In-App Purchase) flow handler implementing IStoreController interface
/// Provides hooks for triggering purchase initiation via events
/// </summary>
public class CodedIAPFlow : IStoreController
{
    /// <summary>
    /// Initiates a purchase using a Product object and optional developer payload
    /// </summary>
    public void InitiatePurchase(Product product, string payload)
    {
        StaticEventIAP.onInitiatePurchase?.Invoke();
    }

    /// <summary>
    /// Initiates a purchase using a product ID and optional developer payload
    /// </summary>
    public void InitiatePurchase(string productId, string payload)
    {
        StaticEventIAP.onInitiatePurchase?.Invoke();
    }

    /// <summary>
    /// Initiates a purchase using only the Product object
    /// </summary>
    public void InitiatePurchase(Product product)
    {
        Debug.Log("Final step: " + product.definition.id);
        StaticEventIAP.onInitiatePurchase?.Invoke();
    }

    /// <summary>
    /// Initiates a purchase using only the product ID
    /// </summary>
    public void InitiatePurchase(string productId)
    {
        StaticEventIAP.onInitiatePurchase?.Invoke();
    }

    /// <summary>
    /// Used to fetch additional products into the store dynamically
    /// </summary>
    public void FetchAdditionalProducts(HashSet<ProductDefinition> additionalProducts, Action successCallback, Action<InitializationFailureReason> failCallback)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Overload for fetching additional products with detailed failure message
    /// </summary>
    public void FetchAdditionalProducts(HashSet<ProductDefinition> additionalProducts, Action successCallback, Action<InitializationFailureReason, string> failCallback)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Confirms a pending purchase
    /// </summary>
    public void ConfirmPendingPurchase(Product product)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Property to access the current list of available products
    /// </summary>
    public ProductCollection products { get; }
}