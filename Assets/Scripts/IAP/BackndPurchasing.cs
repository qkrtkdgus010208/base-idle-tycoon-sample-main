using BackEnd;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

public static class BackndPurchasing
{
    /// <summary>
    /// Backnd receipt validation
    /// </summary>
    /// <param name="args"> purchase event args </param>
    /// <param name="OnPurchasedCompleted"> callback after receipt validation success </param>
    /// <returns> purchasing result </returns>
    public static PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args, Action<string> OnPurchasedCompleted)
    {
        string receiptJson = args.purchasedProduct.receipt;
        decimal iapPrice = args.purchasedProduct.metadata.localizedPrice;
        string iapCurrency = args.purchasedProduct.metadata.isoCurrencyCode;
        string completedPurchasedID = args.purchasedProduct.definition.id;

        var bro = Backend.Receipt.IsValidateGooglePurchase(args.purchasedProduct.receipt, "receiptDescription", false, iapPrice, iapCurrency);

        if (bro.IsSuccess())
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", completedPurchasedID));
            OnPurchasedCompleted?.Invoke(completedPurchasedID);
            return PurchaseProcessingResult.Complete;
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", completedPurchasedID));
            return PurchaseProcessingResult.Pending;
        }
    }
}
