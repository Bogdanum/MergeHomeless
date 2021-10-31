using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MEC;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ApplicationManager : Singleton<ApplicationManager>
{
    [Header ("Data")] [SerializeField] private LevelData _LevelData;

    [SerializeField] private ItemNodeGroupData _ItemNodeGroupData;
    [SerializeField] private Unit              _Unit;
    [SerializeField] private RandomReward      _RandomReward;
    [SerializeField] private RandomReward      _RandomTouchBox;
    [SerializeField] private RewardIcon        _RewardIcon;

    public static bool IsGameReady;

    #region Variables

    public bool IsDialogEnable;

    private IDialog _IDialog;

    private double _CoinsUpdate;
    private int    _CoinsUnit;

    private double _CocaUpdate;
    private int    _CocaUnit;

    private string _CoinsText = "0";
    private string _CocaText  = "0";

    private readonly StringFast _StringFast = new StringFast ();

    private bool _IsUpdate;

    #endregion

    #region System

    protected override void Awake ()
    {
        base.Awake ();


        InitConfig ();

        LoadLevelParameter ();

        Timing.RunCoroutine (SystemClean ());
    }

    private void OnApplicationPause (bool pauseStatus)
    {
        if (!pauseStatus)
        {
            Time.timeScale = 1;
            
            LocalNotification.DisableRetentionNotification ();

            return;
        }

        // =============================== SAVE TIME ONLINE ================================ //

        PlayerData._LastTimeOnline = Helper.GetUtcTimeString ();
        PlayerData.SaveLastTimeOnline ();
        
        LocalNotification.EnableRetentionNotification ();

        Time.timeScale = 0;
    }

    private IEnumerator<float> SystemClean ()
    {
        while (_IsUpdate)
        {
            yield return Timing.WaitForSeconds (30);

            GC.Collect ();
            Resources.UnloadUnusedAssets ();
        }
    }

    protected override void OnDestroy ()
    {
        _IsUpdate = false;

        base.OnDestroy ();
    }

    #endregion

    #region Controller

    private void InitConfig ()
    {
        GameConfig._DefaultNumberBaseActive       = 1;
        GameConfig._NumberBasePlusForEachLevel    = 1;
        GameConfig._MaxNumberBaseActiveForEarning = 15;
        GameConfig._DiamondBuySpeedUp             = 3;
        GameConfig._MaxLevel                      = _LevelData.GetMaxLevel ();

        Application.targetFrameRate = Contains.IsBatteryOn ? 30 : 60;
        Input.multiTouchEnabled     = false;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _IsUpdate = true;

        IsGameReady = false;
    }

    #endregion

    #region Action

    public void OpenUrlStore ()
    {
        #if UNITY_ANDROID
     
        // Remote this when you update the url store
        Application.OpenURL (UISettingManager.GooglePlayURL);
        
        //Application.OpenURL ("market://details?id=com.omstore.mergecars");
        #elif UNITY_IOS
         Application.OpenURL("itms-apps://itunes.apple.com/app/id" + UISettingManager.iosAppID);
        #endif
    }

    public void LoadLevelParameter ()
    {
        Contains.ExpNeedReach = _LevelData.GetExpWithLevel (PlayerData.Level);
    }

    public bool AlertLevelRequire (int level)
    {
        if (level <= PlayerData.Level)
            return false;

        this.ShowMessage (string.Format (ApplicationLanguage.Text_description_require_level, level.ToString ()));

        return true;
    }

    public void AlertNoAdsAvailable ()
    {
        this.ShowMessage (ApplicationLanguage.Text_description_no_more_ads);
    }

    public void AlertNoMorePark ()
    {
        this.ShowMessage (ApplicationLanguage.Text_description_no_more_park);
    }

    public void AlertCompletedMission ()
    {
        this.ShowMessage (ApplicationLanguage.Text_description_mission_not_complete);
    }

    public void AlertUpdateGame () { }

    public void AlertNotEnoughCurrency (CurrencyEnums.CurrencyId id)
    {
        switch (id)
        {
            case CurrencyEnums.CurrencyId.Diamonds:
                this.ShowMessage (ApplicationLanguage.Text_description_not_enough_diamonds);
                break;
            case CurrencyEnums.CurrencyId.Coca:
                this.ShowMessage (ApplicationLanguage.Text_description_not_enough_coca);
                break;
            case CurrencyEnums.CurrencyId.Cash:
                this.ShowMessage (ApplicationLanguage.Text_description_not_enough_cash);
                break;
            case CurrencyEnums.CurrencyId.Ads:
                break;
            case CurrencyEnums.CurrencyId.Lock:
                this.ShowMessage (ApplicationLanguage.Text_description_locked);
                break;
        }
    }

    public void SetDialog (IDialog iDialog)
    {
        if (_IDialog != null)
        {
            _IDialog.DisableHud ();
        }

        _IDialog       = iDialog;
        IsDialogEnable = true;

  //      AdsManager.Instance.ShowBanner ();
    }

    public void UnSetDialog (IDialog dialog)
    {
        if (_IDialog == dialog)
        {
            _IDialog = null;
        }

        IsDialogEnable = false;

//        AdsManager.Instance.HideBanner ();
    }

    public bool IsMinusCurrency (double value, int unit, CurrencyEnums.CurrencyId id)
    {
        switch (id)
        {
            case CurrencyEnums.CurrencyId.Diamonds:

                if (Helper.IsSmaller (PlayerData.Diamonds, 0, value, 0))
                {
                    return false;
                }

                PlayerData.Diamonds -= (int) value;
                PlayerData.SaveDiamonds ();

                break;
            case CurrencyEnums.CurrencyId.Coca:

                if (Helper.IsSmaller (PlayerData.Coca, PlayerData.CocaUnit, value, unit))
                    return false;

                Helper.MinusValue (PlayerData.Coca, PlayerData.CocaUnit, value, unit, out PlayerData.Coca, out PlayerData.CocaUnit);
                PlayerData.SaveCoca ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

                break;
            case CurrencyEnums.CurrencyId.Cash:

                if (Helper.IsSmaller (PlayerData.Coins, PlayerData.CoinUnit, value, unit))
                    return false;

                Helper.MinusValue (PlayerData.Coins, PlayerData.CoinUnit, value, unit, out PlayerData.Coins, out PlayerData.CoinUnit);
                PlayerData.SaveCoins ();

                break;
            case CurrencyEnums.CurrencyId.Ads:
                break;
            case CurrencyEnums.CurrencyId.Lock:
                return false;
            default:
                throw new ArgumentOutOfRangeException ("id", id, null);
        }

        return true;
    }

    public void CheckPlayerLevelRate ()
    {
        if (!this.IsTutorialCompleted (TutorialEnums.TutorialId.HowToPlayGame))
            return;
        
        if (PlayerData.Level > 0 && PlayerData.Level % 5 == 0)
        {
            MessageManager.Instance.ShowRate ();
        }
    }

    public void CheckUnlockLevelRate ()
    {
        if (!this.IsTutorialCompleted (TutorialEnums.TutorialId.HowToPlayGame))
            return;
        
        if (PlayerData.LastLevelUnlocked > 0 && PlayerData.LastLevelUnlocked % 5 == 0)
        {
            MessageManager.Instance.ShowRate ();
        }
    }

    #endregion

    #region Helper

    public Sprite GetIconReward (RewardEnums.RewardId id)
    {
        return _RewardIcon.GetIcon (id);
    }

    public string GetUnit (int unit)
    {
        return _Unit.GetExtensionWithIndex (unit);
    }

    public string AppendFromUnit (int value, int unit)
    {
        _StringFast.Clear ().Append (value);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }

    public string AppendFromUnit (float value, int unit)
    {
        _StringFast.Clear ().Append (value);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }


    public string AppendFromUnit (double value, int unit, int decimal_number)
    {
        _StringFast.Clear ().Append (value, decimal_number);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }

    public string AppendFromUnit (double value, int unit)
    {
        _StringFast.Clear ().Append (value);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }

    public string AppendFromCashUnit (double value, int unit)
    {
        _StringFast.Clear ()
                   .Append ("$")
                   .Append (value);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }

    public string AppendFromCashUnit (double value, int unit, int decimal_number)
    {
        _StringFast.Clear ()
                   .Append ("$")
                   .Append (value, decimal_number);

        if (unit > 0)
        {
            _StringFast.Append (_Unit.GetExtensionWithIndex (unit - 1));
        }

        return _StringFast.ToString ();
    }

    public int GetRandomItemReward ()
    {
        return _RandomReward.GetRandomItemLevel (PlayerData.LastLevelUnlocked);
    }

    public int GetRandomItemTouchBox ()
    {
        return _RandomTouchBox.GetRandomItemLevel (PlayerData.LastLevelUnlocked);
    }

    public string GetPlayerCocaText (double value, int unit)
    {
        if (_CocaUnit == unit && (int) _CocaUpdate == (int) value)
            return _CocaText;

        _CocaText   = AppendFromUnit (value, unit);
        _CocaUpdate = value;
        _CocaUnit   = unit;


        return _CocaText;
    }

    public string GetPlayerCoinsText (double value, int unit)
    {
        if (_CoinsUnit == unit && (int) value == (int) _CoinsUpdate)
            return _CoinsText;

        _CoinsText   = AppendFromCashUnit (value, unit);
        _CoinsUpdate = value;
        _CoinsUnit   = unit;

        return _CoinsText;
    }

    #endregion

    #region Debug

    private void ClearPlayerData ()
    {
        PlayerPrefs.DeleteAll ();
    }


    private void DebugToString (double value)
    {
        LogGame.Log (string.Format ("[Debug] Convert to string: {0}", AppendFromUnit (value, 0)));
    }

    private void AddGold (double value, int unit)
    {
        Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, value, unit);

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
    }

    private void LevelUp ()
    {
        if (GameActionManager.Instance != null) GameActionManager.Instance.LevelUp ();
    }

    private void AddMultiRewardCoins (int time)
    {
        if (GameActionManager.Instance != null)
            GameActionManager.Instance.SetMultiRewardCoins (time);

        if (MoreCashManager.Instance != null)
            MoreCashManager.Instance.RefreshMultiRewardCoins ();
    }

    #endregion
}