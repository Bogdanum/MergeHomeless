using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.SceneManagement;

public class IapManager : Singleton<IapManager>, IStoreListener
{
    private static IStoreController   m_StoreController;        // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    private readonly Dictionary<string, string> _PriceLocals = new Dictionary<string, string> ();
    private readonly Dictionary<string, float>  PriceFloats  = new Dictionary<string, float> ();

    [Header ("Data")] [SerializeField] private IAPData[] _IapData;

    private string currency_symbol;

    private void Start ()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing ();
        }
    }

    public void InitializePurchasing ()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized ())
        {
            Debug.Log ("Init Completed");

            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

        for (int i = 0; i < _IapData.Length; i++)
        {
            builder.AddProduct (_IapData[i].IapId, ProductType.Consumable);
            _PriceLocals.Add (_IapData[i].IapId, _IapData[i].PriceOffline);
            PriceFloats.Add (_IapData[i].IapId, 0.1f);
        }

        currency_symbol = "USD";

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize (this, builder);
    }


    #region PRODUCT

    public void BuyProductWithID (string id)
    {
        #if UNITY_EDITOR

        for (int i = 0; i < _IapData.Length; i++)
        {
            if (string.CompareOrdinal (_IapData[i].IapId, id) == 0)
            {
                switch (_IapData[i].id)
                {
                    case IapEnums.IapId.RemoveAdsPack:
                        OnBuyRemoveAdsCompleted (_IapData[i]);
                        break;
                    default:
                        OnBuyDiamondsCompleted (_IapData[i]);
                        break;
                }

                break;
            }
        }

        return;

        #endif

        BuyProductID (id);
    }

    #endregion


    private bool IsInitialized ()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyNonConsumable ()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
    }

    void BuyProductID (string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized ())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID (productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase (product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log ("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases ()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized ())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log ("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log ("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions> ();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions ((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log ("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log ("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        _PriceLocals.Clear ();
        PriceFloats.Clear ();


        foreach (var pro in controller.products.all)
        {
            if (!_PriceLocals.ContainsKey (pro.definition.id))
            {
                _PriceLocals.Add (pro.definition.id, pro.metadata.localizedPriceString);
                PriceFloats.Add (pro.definition.id, (float) pro.metadata.localizedPrice);

                currency_symbol = pro.metadata.isoCurrencyCode;
            }
        }
    }

    public void OnInitializeFailed (InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
    {
        string id = args.purchasedProduct.definition.id;

        if (string.CompareOrdinal (args.purchasedProduct.definition.id, id) == 0)
        {
            Debug.Log (string.Format ("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }

        bool validPurchase = true; // Presume valid for platforms with no R.V.


        for (int i = 0; i < _IapData.Length; i++)
        {
            if (string.CompareOrdinal (_IapData[i].IapId, id) == 0)
            {
                switch (_IapData[i].id)
                {
                    case IapEnums.IapId.RemoveAdsPack:
                        OnBuyRemoveAdsCompleted (_IapData[i]);
                        break;
                    default:
                        OnBuyDiamondsCompleted (_IapData[i]);
                        break;
                }

                break;
            }
        }
        
        return PurchaseProcessingResult.Complete;
    }

    private void OnBuyDiamondsCompleted (IAPData data)
    {
        PlayerData.Diamonds += data.Value;
#if !UNITY_EDITOR
        // add 21.10.2021 ------ Firebase
        switch (data.IapId)
        {
            case "small_pack":
                Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_small_pack", "small_pack", "true");
                break;
            case "medium_pack":
                Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_medium_pack", "medium_pack", "true");
                break;
            case "big_pack":
                Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_big_pack", "big_pack", "true");
                break;
            default:
                break;
        }
        // ---------------------
#endif
        PlayerData.SaveDiamonds ();

        if (GameActionManager.Instance != null)
        {
            if (GameActionManager.Instance != null)
                GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                               UIGameManager.Instance.GetPositionHubDiamonds (),
                                                               data.Value, () =>
                                                               {
                                                                   UIGameManager.Instance.FxShakeDiamonds ();
                                                                   GameActionManager.Instance.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
                                                               });
        }
    }

    private void OnBuyRemoveAdsCompleted (IAPData data)
    {
        PlayerData.Diamonds += data.Value;

        PlayerData.SaveDiamonds ();

        PlayerData.IsRemoveAds = true;

        if (GameActionManager.Instance != null)
        {
            GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                           UIGameManager.Instance.GetPositionHubDiamonds (),
                                                           data.Value,
                                                           () =>
                                                           {
                                                               UIGameManager.Instance.FxShakeDiamonds ();
                                                               GameActionManager.Instance.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
                                                           });
        }

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.RefreshRemoveAds ();
        }
    }

    /// <summary>
    /// Returns the price.
    /// </summary>
    /// <returns>The the price.</returns>
    public string ReturnThePrice (IapEnums.IapId id)
    {
        for (int i = 0; i < _IapData.Length; i++)
        {
            if (_IapData[i].id == id)
            {
                return _PriceLocals[_IapData[i].IapId];
            }
        }

        return "????";
    }

    public float ReturnFloatPrice (IapEnums.IapId id)
    {
        for (int i = 0; i < _IapData.Length; i++)
        {
            if (_IapData[i].id == id)
            {
                return PriceFloats[_IapData[i].IapId];
            }
        }

        return 0.1f;
    }

    /// <summary>
    /// Raises the purchase failed event.
    /// </summary>
    /// <param name="product">Product.</param>
    /// <param name="failureReason">Failure reason.</param>
    public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}