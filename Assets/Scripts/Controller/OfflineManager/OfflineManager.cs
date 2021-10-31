using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineManager : Singleton<OfflineManager>, IDialog
{
    [Header ("UI")] [SerializeField] private UIOfflineManager _UiOfflineManager;

    [Header ("Data")] [SerializeField] private OfflineData _OfflineData;

    private double _RevenueOffline;
    private int    _RevenueUnit;

    private bool IsWatchAdsCompleted;

    protected override void Awake ()
    {
        base.Awake ();

        _UiOfflineManager.Init ();
    }

    #region Action

    public void EnableOfflineProfit (double profitPerSec, int profitUnit)
    {
        IsWatchAdsCompleted = false;

        var last_time     = DateTime.Parse (PlayerData._LastTimeOnline);
        var current_time  = Helper.GetUtcTime ();
        var total_seconds = _OfflineData.GetMaxSeconds ((current_time - last_time).TotalSeconds);

        if (total_seconds <= 0 || (int) profitPerSec == 0 && profitUnit == 0)
        {
            PlayerData.SaveLastTimeOnline ();

            return;
        }

        var total_hours = (int) (total_seconds / 3600f);
        var percents    = _OfflineData.GetPercentProfit (total_hours);

        total_seconds = total_seconds * percents;

        Helper.AddValue (ref _RevenueOffline, ref _RevenueUnit, profitPerSec, profitUnit, total_seconds);

        _UiOfflineManager.UpdateTextRevenue (string.Format ("${0}", ApplicationManager.Instance.AppendFromUnit (_RevenueOffline, _RevenueUnit)));
        _UiOfflineManager.EnableWatchAds ();

        EnableOfflineManager ();

        _UiOfflineManager.SetTimeAway (Helper.ConvertToTime (total_seconds));

        // =============================== Get revenue by watch ads ================================ //


        var revenue_ads      = _RevenueOffline * GameConfig.RevenueCanGetFromAds;
        var unit_revenue_ads = _RevenueUnit;

        Helper.FixUnit (ref revenue_ads, ref unit_revenue_ads);


        _UiOfflineManager.SetRevenueFromAds (string.Format ("${0}", ApplicationManager.Instance.AppendFromUnit (revenue_ads, unit_revenue_ads)));

        // =============================== Get revenue by use diamonds ================================ //

        var revenue_diamonds      = _RevenueOffline * GameConfig.RevenueCanGetFromDiamond;
        var unit_revenue_diamonds = _RevenueUnit;

        Helper.FixUnit (ref revenue_diamonds, ref unit_revenue_diamonds);

        _UiOfflineManager.SetRevenueFromDiamonds (string.Format ("${0}", ApplicationManager.Instance.AppendFromUnit (revenue_diamonds, unit_revenue_diamonds)));
    }

    public void EnableOfflineManager ()
    {
        ApplicationManager.Instance.SetDialog (this);

        EnableHud ();

        this.PlayAudioSound (AudioEnums.SoundId.WelcomeBackDialog);
    }

    public void DisableOfflineManager ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        ApplicationManager.Instance.UnSetDialog (this);

        DisableHud ();

        this.PlayAudioSound (AudioEnums.SoundId.OfflineGetCoins);
    }

    public void EnableHud ()
    {
        _UiOfflineManager.EnableHud ();

        GameManager.Instance.DisableTouch ();
    }

    public void DisableHud ()
    {
        _UiOfflineManager.Close ();

        GameManager.Instance.EnableTouch ();

        UpdateValueEarned ();
    }

    public void UseDiamondForRevenue ()
    {
        if (!ApplicationManager.Instance.IsMinusCurrency (GameConfig.DiamondUseForRevenueOffline, 0, CurrencyEnums.CurrencyId.Diamonds))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (CurrencyEnums.CurrencyId.Diamonds);
            
            return;
        }

        OnBuyCompleted ();
    }

    public void WatchAdsForRevenue ()
    {
        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (OnWatchAdsCompleted, null);

            return;
        }
        
        ApplicationManager.Instance.AlertNoAdsAvailable ();
    }

    private void OnBuyCompleted ()
    {
        _RevenueOffline = _RevenueOffline * GameConfig.RevenueCanGetFromDiamond;

        Helper.FixUnit (ref _RevenueOffline, ref _RevenueUnit);

        _UiOfflineManager.DisableWatchAds ();
        _UiOfflineManager.UpdateTextRevenue (ApplicationManager.Instance.AppendFromUnit (_RevenueOffline, _RevenueUnit));

        DisableOfflineManager ();
    }

    private void OnWatchAdsCompleted ()
    {
        _RevenueOffline = _RevenueOffline * GameConfig.RevenueCanGetFromAds;

        Helper.FixUnit (ref _RevenueOffline, ref _RevenueUnit);

        _UiOfflineManager.DisableWatchAds ();
        _UiOfflineManager.UpdateTextRevenue (ApplicationManager.Instance.AppendFromUnit (_RevenueOffline, _RevenueUnit));

        DisableOfflineManager ();
    }

    private void UpdateValueEarned ()
    {
        if (GameActionManager.Instance != null && UIGameManager.Instance != null)
        {
            GameActionManager.Instance.InstanceFxCoins (Vector3.zero, UIGameManager.Instance.GetPositionHubCoins (),
                                                        _RevenueOffline, _RevenueUnit);

            PlayerData._LastTimeOnline = Helper.GetUtcTimeString ();
            PlayerData.SaveLastTimeOnline ();

            GameActionManager.Instance.FxDisplayGold (Vector3.zero,
                                                      string.Format ("+{0}", ApplicationManager.Instance.AppendFromCashUnit (_RevenueOffline, _RevenueUnit)));
        }
        else
        {
            Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, _RevenueOffline, _RevenueUnit);

            PlayerData.SaveCoins ();

            PlayerData._LastTimeOnline = Helper.GetUtcTimeString ();
            PlayerData.SaveLastTimeOnline ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
        }
    }

    #endregion
}