using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class InAppManager : PSManager, IStoreListener
{
    public static InAppManager instance = null;

    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    #region 상품ID
    // 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
    public const string sample = "****";
    // 이 이상의 정보를 삭제합니다.
    #endregion

    private bool IsFirstRun = true;

    private bool IsDirectBuy = false;

    private bool IsInAppReady = false;

    [HideInInspector]
    public bool isStarterView = true;
    [HideInInspector]
    public bool isLowView = true;
    [HideInInspector]
    public bool isMiddleView = true;
    [HideInInspector]
    public bool isHighView = true;
    [HideInInspector]
    public bool warhornSubscribe = false;

    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        IsFirstRun = true;
        IsDirectBuy = false;
        IsInAppReady = false;

        InitializePurchasing();

        yield return StartCoroutine(base.InitManager());
    }

    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        // -------------------------샘플-----------------------------
        
        builder.AddProduct(sample, ProductType.Consumable, new IDs
        {
#if UNITY_ANDROID
             { sample, GooglePlay.Name },
#elif UNITY_IOS
             { sample, AppleAppStore.Name },
#endif
        });

        UnityPurchasing.Initialize(this, builder);
    }

    public void InAppPurchaseSample()
    {
        BuyProductID(sample);
    }

    public void BuyProductID(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(productId);

                if (p != null && p.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);

                    IsDirectBuy = true;
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }

        ShopPage.instance.isProceeding = false;
    }

    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = extensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        storeController = sc;
        extensionProvider = ep;

        IsInAppReady = true;

        if (ValidateProduct(af_warhorn_subscribe))
        {
            SubscriptionInfo subscriptionInfo = new SubscriptionInfo("af_warhorn_subscribe");
            if (subscriptionInfo.isSubscribed() == Result.False)
            {
                warhornSubscribe = false; // 구독 X              
            }
            else
            {
                if (subscriptionInfo.isExpired() == Result.False)
                {
                    warhornSubscribe = true;
                }
                else
                {
                    warhornSubscribe = false; // 구독 X
                }
            }
        }
        else
        {
            warhornSubscribe = false; // 구독 X          
        }   
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {

    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        switch (args.purchasedProduct.definition.id)
        {
            case sample:
                if (IsDirectBuy)
                {
                    Core.STATE.AddStar(42);
                    Core.STATE.AddMileage(10);
                    LobbyUI.instance.UpdateStar();
                    ShopPage.instance.UpdateMileage();
                }
                break; 
        }

        // 아직 예외 처리가 필요 없다.
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

            var result = validator.Validate(args.purchasedProduct.receipt);

            foreach (IPurchaseReceipt productReceipt in result)
            {
                switch (productReceipt.productID)
                {
                    case sample:
                        // 구독 상품 체크
                        
                        break;
                    default:
                        break;
                }
            }
        }
        catch (IAPSecurityException)
        {

        }
#endif

        IsFirstRun = false;
        IsDirectBuy = false;

        if (Core.BM.bmLogIn && GameEngine.instance.usingLog)
        {
            if(Core.BM.isFirstInAppPurchase())
            {
                Core.BM.UpdatetUserState("isFirstInAppPurchase", false);
                Core.BM.InsertGameLog("first_InAppPurchase_log", "lastStage", Core.STATE.user.mainQuest.nowMapClearCount.ToString());
            }

            Core.BM.InsertGameLog("purchase_log", "product_ID", args.purchasedProduct.definition.id);
        }

        return PurchaseProcessingResult.Complete;
    }

    public bool ValidateProduct(string productId)
    {
        bool validateValue = false;
        Product p = storeController.products.WithID(productId);

#if UNITY_ANDROID || UNITY_IOS
        try
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

            var result = validator.Validate(p.receipt);
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
#if UNITY_ANDROID
                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                if (null != google)
                {
                    if (google.purchaseState == GooglePurchaseState.Purchased)
                        validateValue = true;
                }
#elif UNITY_IOS
                AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                if (null != apple)
                {
                    validateValue = true;
                }
#endif
            }
        }
        catch (IAPSecurityException)
        {
            validateValue = false;
        }
#endif
        return validateValue;
    }
}
