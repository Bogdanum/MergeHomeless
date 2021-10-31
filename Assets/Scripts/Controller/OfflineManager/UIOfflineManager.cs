using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineManager : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformBonusHud;

    [SerializeField] private Text _TextRevenue,
                                  _TextRevenueWatchAds,
                                  _TextRevenueBuyDiamond;

    [SerializeField] private Button      _WatchAds;
    [SerializeField] private CanvasGroup _GroupWatchAds;
    [SerializeField] private Transform   _IconReward;

    [Header ("Language")] [SerializeField] private Text _LabelOfflineEarning;

    [SerializeField] private Text _LabelEarned,
                                  _DescriptionGetRevenueFromAds,
                                  _DescriptionGetRevenueFromDiamonds;

    [SerializeField] private Text _LabelDiamondLost;

    [SerializeField] private Text _DescriptionTimeGoAway;

    private bool _IsWatchAds;

    #region Action

    public void Init ()
    {
        _LabelDiamondLost.text = GameConfig.DiamondUseForRevenueOffline.ToString ();
    }

    public void EnableHud ()
    {
        if (_TransformBonusHud != null) _TransformBonusHud.gameObject.SetActive (true);

        RefreshLanguage ();
    }

    public void Close ()
    {
        if (_TransformBonusHud != null) _TransformBonusHud.gameObject.SetActive (false);
    }

    public void UpdateTextRevenue (string value)
    {
        if (_TextRevenue != null) _TextRevenue.text = value;
    }

    public void DisableWatchAds ()
    {
        _GroupWatchAds.alpha = 0.5f;
        _IsWatchAds          = false;
    }

    public void EnableWatchAds ()
    {
        _GroupWatchAds.alpha = 1f;
        _IsWatchAds          = true;
    }

    public void SetTimeAway (string value)
    {
        _DescriptionTimeGoAway.text = string.Format (ApplicationLanguage.Text_description_get_revenue_for_times, value);
    }

    public void SetRevenueFromAds (string value)
    {
        _TextRevenueWatchAds.text = value;
    }

    public void SetRevenueFromDiamonds (string value)
    {
        _TextRevenueBuyDiamond.text = value;
    }

    public void RefreshLanguage ()
    {
        _LabelOfflineEarning.text = ApplicationLanguage.Text_label_offline_earning;
        _LabelEarned.text         = ApplicationLanguage.Text_description_you_earned;

        _DescriptionGetRevenueFromAds.text      = string.Format (ApplicationLanguage.Text_description_revenue_from_ads, GameConfig.RevenueCanGetFromAds.ToString ());
        _DescriptionGetRevenueFromDiamonds.text = string.Format (ApplicationLanguage.Text_description_revenue_from_diamonds, GameConfig.RevenueCanGetFromDiamond.ToString ());
    }

    #endregion

    #region Helper

    public Vector3 GetPositionRewardIcon ()
    {
        return _IconReward.position;
    }

    public Vector3 GetPositionWatchAds ()
    {
        return _WatchAds.transform.position;
    }

    #endregion

    #region Interface Interact

    public void WatchAdsForX2 ()
    {
        if (!_IsWatchAds)
            return;

        // =============================== WATCH ADS ================================ //

        if (OfflineManager.Instance != null) OfflineManager.Instance.WatchAdsForRevenue ();

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
    }

    public void InteractUseDiamonds ()
    {
        if (OfflineManager.Instance != null) OfflineManager.Instance.UseDiamondForRevenue ();

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
    }

    #endregion
}