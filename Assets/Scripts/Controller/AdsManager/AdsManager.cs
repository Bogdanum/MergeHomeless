using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using MEC;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof (AdsManager))]
public class AdsManagerEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        EditorGUILayout.HelpBox ("PLEASE CHECK FILE Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.assets to update App ID before bulding", MessageType.Warning);

        base.OnInspectorGUI ();
    }
}

#endif


public class AdsManager : Singleton<AdsManager>
{
    #region Admobs

    [Header ("CONFIG")] //[SerializeField] private string _BannerId;
    [SerializeField]                     private string _RewardVideoId;

    #endregion

    [HideInInspector] public bool IsRewardVideoAvailable;
//    [HideInInspector] public bool IsBannerAvailable;

    [Header ("Config")] [SerializeField] private bool IsAutoReloadAgain;

    public System.Action OnCompletedRewardVideo;
    public System.Action OnFailedRewardVideo;

    public System.Action OnFailedFullScreen;
    public System.Action OnCompletedFullScreen;

    private bool IsCompletedTheRewards;
    private bool IsRewardClosed;
    private bool IsRewardValid;

    private RewardBasedVideoAd reward;
    private BannerView         banner;

    private bool IsRemoveAds;

    private bool IsWatchedRewardAds;
    private bool IsFirstTimeLoadBanner;

    private CoroutineHandle handleLoadAds;

    private void Init ()
    {
//        _BannerId      = _BannerId.Trim ();
        _RewardVideoId = _RewardVideoId.Trim ();
        // =============================== INIT THE AD ================================ //

        MobileAds.Initialize (Debug.Log);

        reward = RewardBasedVideoAd.Instance;
//        banner = new BannerView (_BannerId, AdSize.Banner, AdPosition.Bottom);

        LogGame.Log ("[Ad Manager] Init Event Completed!");

        RefreshRemoveAds ();
    }

    public void RefreshRemoveAds ()
    {
        IsRemoveAds = PlayerData.IsRemoveAds;

/*        if (IsRemoveAds)
        {
            HideBanner ();
        }
*/
    }

    private void RegisterEvent ()
    {
        #if UNITY_EDITOR
        IsRewardVideoAvailable = true;
//        IsBannerAvailable      = true;
        return;
        #endif
        RegisterRewardCallBack ();
//        RegisterBannerCallBack ();

        RefreshRewardVideo ();
//        RefreshBanner ();
    }

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        Init ();

        RegisterEvent ();
    }

    private IEnumerator<float> _LoadAds ()
    {
        while (IsWatchedRewardAds == false)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (IsWatchedRewardAds)
        {
            IsWatchedRewardAds = false;

            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_ReloadRewardAds ()));
        }
    }

    private IEnumerator<float> _ReloadRewardAds ()
    {
        yield return Timing.WaitForOneFrame;

        IsRewardVideoAvailable = false;

        if (IsRewardClosed && IsRewardValid)
        {
            DoCompletedRewardVideo ();

            IsRewardClosed = false;
            IsRewardValid  = false;
        }
        else
        {
            DoFailedRewardVideo ();

            IsRewardClosed = false;
            IsRewardValid  = false;
        }

        if (IsAutoReloadAgain)
            RefreshRewardVideo ();
    }

    #endregion

    #region Reward Callback

    private void RegisterRewardCallBack ()
    {
        reward.OnAdClosed       += RewardOnOnAdClosed;
        reward.OnAdRewarded     += RewardOnOnAdRewarded;
        reward.OnAdFailedToLoad += RewardOnOnAdFailedToLoad;
        reward.OnAdLoaded       += RewardOnOnAdLoaded;
    }

    private void RewardOnOnAdLoaded (object sender, EventArgs e)
    {
        IsRewardVideoAvailable = true;
        IsRewardClosed         = false;
        IsRewardValid          = false;
    }

    private void RewardOnOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
    {
        IsRewardVideoAvailable = false;

        DoFailedRewardVideo ();
    }

    private void RewardOnOnAdRewarded (object sender, Reward e)
    {
        IsRewardValid = true;
    }

    private void RewardOnOnAdClosed (object sender, EventArgs e)
    {
        IsRewardClosed     = true;
        IsWatchedRewardAds = true;
    }

    #endregion

    #region Banner Callback
/*
    private void RegisterBannerCallBack ()
    {
        banner.OnAdClosed       += BannerOnOnAdClosed;
        banner.OnAdLoaded       += BannerOnOnAdLoaded;
        banner.OnAdFailedToLoad += BannerOnOnAdFailedToLoad;
    }

    private void BannerOnOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
    {
        IsBannerAvailable = false;
    }

    private void BannerOnOnAdLoaded (object sender, EventArgs e)
    {
        IsBannerAvailable = true;
    }

    private void BannerOnOnAdClosed (object sender, EventArgs e)
    {
        IsBannerAvailable = false;
    }
*/
    #endregion

    private void DoFailedFullScreen ()
    {
        if (OnFailedFullScreen != null)
        {
            OnFailedFullScreen ();
            OnFailedFullScreen = null;
        }
    }

    private void DoCompletedFullScreen ()
    {
        if (OnCompletedFullScreen != null)
        {
            OnCompletedFullScreen ();
            OnCompletedFullScreen = null;
        }
    }

    private void DoFailedRewardVideo ()
    {
        if (OnFailedRewardVideo != null)
        {
            OnFailedRewardVideo ();
            OnFailedRewardVideo = null;
        }

        LogGame.Log ("[Ad Manager] Reward Video Is Failed!");
    }

    private void DoCompletedRewardVideo ()
    {
        if (OnCompletedRewardVideo != null)
        {
            OnCompletedRewardVideo ();
            OnCompletedRewardVideo = null;
        }

        LogGame.Log ("[Ad Manager] Reward Video Is Completed!");
#if !UNITY_EDITOR
        // add 21.10.2021 ------ Firebase
        Firebase.Analytics.FirebaseAnalytics.LogEvent("show_rewarded_ad", "show_rewarded", "true");
#endif
    }

    public void RegisterEvent (System.Action OnCompleted, System.Action OnFailed)
    {
        OnCompletedRewardVideo = OnCompleted;
        OnFailedRewardVideo    = OnFailed;
    }

    public void RegisterEventFullScreen (System.Action OnCompleted, System.Action OnFailed)
    {
        OnFailedFullScreen    = OnFailed;
        OnCompletedFullScreen = OnCompleted;
    }

    public void ShowRewardVideo ()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        DoCompletedRewardVideo ();

        return;

#endif

        if (reward.IsLoaded ())
        {
            Timing.KillCoroutines (handleLoadAds);

            handleLoadAds = Timing.RunCoroutine (_LoadAds ());

            reward.Show ();
        }
        else
        {
            RefreshRewardVideo ();

            DoFailedRewardVideo ();
        }
    }
// AdsManagerExtentions.cs (s34)  && ApplicationManager (s198)
/*    public void ShowBanner ()
    {
        if (IsRemoveAds)
        {
            return;
        }

        if (IsBannerAvailable)
        {
            banner.Show ();
        }
        else
        {
            RefreshBanner ();
        }
    }
*/
// AdsManagerExtentions.cs (s46) && ApplicationManager (s210)
/*    public void HideBanner ()
    {
        if (banner != null)
            banner.Hide ();
    }
*/
    public void RefreshRewardVideo ()
    {
        if (IsRewardVideoAvailable) return;

        AdRequest request = new AdRequest.Builder ().Build ();

        reward.LoadAd (request, _RewardVideoId);
    }
/*
    public void RefreshBanner ()
    {
        if (IsRemoveAds)
            return;

        if (IsBannerAvailable) return;

        AdRequest request = new AdRequest.Builder ().Build ();

        banner.LoadAd (request);
    }
*/
}