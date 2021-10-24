using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Advertisements;

public class GoogleAdsManager : PSManager
{
    private RewardBasedVideoAd rewardBasedVideo;
    private InterstitialAd interstitial;

    // Google Admob
    public string android_RewardBasedVideo_ID;
    public string ios_RewardBasedVideo_ID;

    public string android_Interstitial_ID;
    public string ios_Interstitial_ID;

    // Unity Ads
    public string android_UnityAds_Game_ID;
    public string ios_UnityAds_Game_ID;
    public string unityAds_RewardBasedVideo_ID;

    [HideInInspector]
    public bool IsAdsOn = true;
    [HideInInspector]
    public int IsAdsType = 1;

    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        IsAdsOn = true;

#if UNITY_ANDROID
        string appId = "*************";
#elif UNITY_IPHONE
        string appId = "*************";
#else
        string appId = "unexpected_platform";
#endif

#if UNITY_ANDROID
        Advertisement.Initialize(android_UnityAds_Game_ID);
#elif UNITY_IOS
        Advertisement.Initialize(ios_UnityAds_Game_ID);
#endif

#if UNITY_EDITOR

#else
        MobileAds.Initialize(appId);

        rewardBasedVideo = RewardBasedVideoAd.Instance;

        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;

        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;

        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;

        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;

        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;

        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        RequestInterstitial();
#endif

        yield return StartCoroutine(base.InitManager());
    }

    public void SetAdsOn(bool set)
    {
        IsAdsOn = set;
    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = android_Interstitial_ID;
#elif UNITY_IPHONE
        string adUnitId = ios_Interstitial_ID;
#else
        string adUnitId = "unexpected_platform";
#endif

        interstitial = new InterstitialAd(adUnitId);

        interstitial.OnAdClosed += Interstitial_HandleOnAdClosed;

        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();

        interstitial.LoadAd(request);
    }

    public void Interstitial_HandleOnAdClosed(object sender, EventArgs args)
    {
        CleanUpInterstitial();

        RequestInterstitial();

        Core.STATE.SetVideoAdsState(true);
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        CleanUpInterstitial();
    }

    public bool InterstitialIsLoad()
    {
        return interstitial.IsLoaded();
    }

    public void InterstitialShow()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            Core.STATE.SetVideoAdsState(true);
        }
    }

    public void CleanUpInterstitial()
    {
        interstitial.Destroy();
    }

    public void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = android_RewardBasedVideo_ID;
#elif UNITY_IPHONE
        string adUnitId = ios_RewardBasedVideo_ID;
#else
        string adUnitId = "unexpected_platform";
#endif

        AdRequest request = new AdRequest.Builder().Build();

        rewardBasedVideo.LoadAd(request, adUnitId);
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Core.STATE.SetVideoAdsState(true);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {      
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        RequestRewardBasedVideo();
        Core.STATE.SetVideoAdsState(true);
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        //reward
        // 광고를 다봤을 경우 어떤 보상으로 처리할 것인가? 힐타일, 보상 두배
        if (IsAdsType == 1)
            GameUI.instance.OneMoreReward();
        else if (IsAdsType == 2)
            GameUI.instance.HealPlayer();
        else if (IsAdsType == 3)
        {          
            int num = 15;

            if (Core.STATE.user.can + num < Core.STATE.user.maxCan)
            {
                Core.STATE.user.can += num;
            }
            else
            {
                Core.STATE.user.can += num;
                Core.STATE.user.canFull = true;           
            }
            if (Core.BM.bmLogIn)
            {
                Core.STATE.user.canAdsview = true;
                Core.BM.UpdateUserAsset(Core.STATE.user);

                Core.STATE.SetTimeRecordCanAds(Core.BM.GetBackendServerTime());          
            }
            else
            {
                Core.STATE.user.canAdsview = true;

                DateTime nowTime = DateTime.UtcNow;
                nowTime = nowTime.AddHours(9);
                Core.STATE.SetTimeRecordCanAds(nowTime);
            }
            CanFillTimer.instance.SyncCanTimer();
        }
        else if (IsAdsType == 4)
        {
            Core.STATE.AddUpstone_KMD(50);
            Core.STATE.AddUpstone_IMT(50);
            Core.STATE.AddUpstone_288(50);
            Core.STATE.AddUpstone_MEL(50);

            if (Core.BM.bmLogIn)
            {
                Core.STATE.user.stoneAdsView = true;
                Core.BM.UpdateUserAsset(Core.STATE.user);

                Core.STATE.SetTimeRecordStoneAds(Core.BM.GetBackendServerTime());
                ShopPage.instance.stoneAdsFilltime = Core.STATE.timeRecord.stoneAdsTime;
            }
            else
            {
                Core.STATE.user.stoneAdsView = true;

                DateTime nowTime = DateTime.UtcNow;
                nowTime = nowTime.AddHours(9);
                Core.STATE.SetTimeRecordStoneAds(nowTime);
                ShopPage.instance.stoneAdsFilltime = Core.STATE.timeRecord.stoneAdsTime;
            }
        }

        Core.STATE.SaveUserData();

        Core.STATE.AddWatchAdCount();
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {

    }

    public bool IsRewardBasedLoad()
    {
        return rewardBasedVideo.IsLoaded();
    }

    public void RewardBasedAdsShow()
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
        else
        {
            Core.STATE.SetVideoAdsState(true);

            switch (IsAdsType)
            {
                case 1:
                    if (GameUI.instance != null)
                        GameUI.instance.OpenNoneAdsPop();
                    break;

                case 2:
                    if (GameUI.instance != null)
                        GameUI.instance.OpenNoneAdsPop();
                    break;

                case 3:
                    if (LobbyUI.instance != null)
                        WarningPop.instance.OpenWarningPop(EWarningType.Ads);
                    break;

                case 4:
                    if (LobbyUI.instance != null)
                        WarningPop.instance.OpenWarningPop(EWarningType.Ads);
                    break;
            }
        }
    }

    public void RewardUnityAdsShow()
    {
        if (Advertisement.IsReady(unityAds_RewardBasedVideo_ID))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };

            Advertisement.Show(unityAds_RewardBasedVideo_ID, options);
        }
        else
        {
            Core.STATE.SetVideoAdsState(true);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Core.STATE.SetVideoAdsState(true);
                // 광고 시청 완료
                break;

            case ShowResult.Skipped:
                Core.STATE.SetVideoAdsState(true);
                // 광고 시청 스킵
                break;

            case ShowResult.Failed:
                Core.STATE.SetVideoAdsState(true);
                // 광고 시청 실패
                break;
        }
    }

    public void AdsRequestCheck()
    {
#if !UNITY_EDITOR
        if (!rewardBasedVideo.IsLoaded())
        {
            RequestRewardBasedVideo();
        }

        if (!interstitial.IsLoaded())
        {
            RequestInterstitial();
        }
#endif
    }

    public void CleanManager()
    {
        if (interstitial != null)
            interstitial.Destroy();
    }
}
