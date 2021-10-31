using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : Singleton<BonusManager>, IDialog
{
    [Header ("UI")] [SerializeField] private Transform      transform_hub_reward;
    [SerializeField]                 private Text           label_value_get;
    [SerializeField]                 private SpriteRenderer icon_currency;
    [SerializeField]                 private SpriteRenderer icon_reward;

    [SerializeField] private FxMoving fx_moving_bonus;

    [SerializeField] private Image icon_currency_ui;

    [Header ("Data")] [SerializeField] private BonusData _BonusData;
    [SerializeField]                   private BonusData _CurrencyData;

    [Header ("Bonus Coins")] [SerializeField]
    private Transform transform_bonus_coins;

    [SerializeField] private float width_bonus_currency;
    [SerializeField] private float height_bonus_currency;

    [Header ("Language")] [SerializeField] private Text description_watch_ads;
    [SerializeField]                       private Text label_bonus_reward;

    private BonusEnums.BonusId _BonusId;
    private BonusEnums.BonusId _BonusCurrencyId;

    private bool is_enabled_bonus_reward;
    private bool is_enabled_bonus_currency;
    private bool IsEnable;
    private bool IsReady;

    private Vector3 position_bonus;
    private Vector3 position_bonus_coins;

    private CoroutineHandle _HandleAppearBonus,
                            handle_appear_bonus_currency;

    private double profit;
    private int    profit_unit;

    private System.Action<object> HandleStateInteractUI;

    protected override void Awake ()
    {
        base.Awake ();

        RefreshAppearBonusCurrency ();
        RegisterAction ();
        OnInteractUI (true);
    }

    protected override void OnDestroy ()
    {
        UnRegisterAction ();

        base.OnDestroy ();
    }

    #region Controller

    private void RegisterAction ()
    {
        HandleStateInteractUI = param => Instance.OnInteractUI ((bool) param);

        this.RegisterActionEvent (ActionEnums.ActionID.SetStateInteractUI, HandleStateInteractUI);

        fx_moving_bonus.OnMovingEnd += FxMovingBonusOnOnMovingEnd;
    }


    private void UnRegisterAction ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.SetStateInteractUI, HandleStateInteractUI);

        if (!ReferenceEquals (fx_moving_bonus, null))
        {
            fx_moving_bonus.OnMovingEnd -= FxMovingBonusOnOnMovingEnd;
        }
    }


    private void RefreshAppearBonusCurrency ()
    {
        Timing.KillCoroutines (handle_appear_bonus_currency);

        handle_appear_bonus_currency = Timing.RunCoroutine (_EnumeratorAppearBonusCurrency ());
    }

    private void RefreshLanguage ()
    {
        description_watch_ads.text = ApplicationLanguage.Text_description_watch_to_get_free;
        label_bonus_reward.text    = ApplicationLanguage.Text_label_bonus_reward;
    }

    private void UpdateReward ()
    {
        icon_currency.enabled = true;

        switch (_BonusCurrencyId)
        {
            case BonusEnums.BonusId.FreeDiamonds:
                icon_currency.sprite = GameData.Instance.CurrencyIcon.GetIconCurrency (CurrencyEnums.CurrencyId.Diamonds);
                break;
            case BonusEnums.BonusId.X5Cash:
                icon_currency.sprite = GameData.Instance.CurrencyIcon.GetIconCurrency (CurrencyEnums.CurrencyId.Cash);
                break;
            default:
                icon_currency.enabled = false;
                break;
        }

        icon_currency_ui.sprite = icon_currency.sprite;

        var real_profit = EarningManager.Instance.ProfitPerSec;
        var real_unit   = EarningManager.Instance.ProfitUnit;

        var quantity = _CurrencyData.GetQuantity (_BonusCurrencyId);

        switch (_BonusCurrencyId)
        {
            case BonusEnums.BonusId.FreeCash:
                if (real_unit == 0 && real_profit < 1)
                {
                    real_profit = GameConfig.DefaultCoinEarn;
                    real_unit   = 0;
                }


                profit      = real_profit * quantity;
                profit_unit = real_unit;

                Helper.FixUnit (ref profit, ref profit_unit);

                label_value_get.text = ApplicationManager.Instance.AppendFromCashUnit (profit, profit_unit);

                break;
            case BonusEnums.BonusId.FreeDiamonds:

                profit      = quantity;
                profit_unit = 0;

                label_value_get.text = ApplicationManager.Instance.AppendFromUnit (profit, profit_unit) + " " + ApplicationLanguage.Text_bonus_gems;

                break;
            case BonusEnums.BonusId.X5Cash:

                if (real_unit == 0 && real_profit < 1)
                {
                    real_profit = GameConfig.DefaultCoinEarn;
                    real_unit   = 0;
                }


                profit      = real_profit * quantity;
                profit_unit = real_unit;

                Helper.FixUnit (ref profit, ref profit_unit);

                label_value_get.text = ApplicationManager.Instance.AppendFromCashUnit (profit, profit_unit);

                break;
        }

        icon_reward.sprite = _CurrencyData.GetIcon (_BonusCurrencyId);
    }

    #endregion

    #region Action

    public bool PostPositionBonusCoins (Vector3 position)
    {
        if (IsEnable || !IsReady)
            return false;

        if (ApplicationManager.Instance.IsDialogEnable)
        {
            return false;
        }

        if (!is_enabled_bonus_currency)
        {
            return false;
        }

        position_bonus_coins = transform_bonus_coins.position;

        if (position.x > position_bonus_coins.x - width_bonus_currency / 2f &&
            position.x < position_bonus_coins.x + width_bonus_currency / 2f &&
            position.y > position_bonus_coins.y - height_bonus_currency / 2f &&
            position.y < position_bonus_coins.y + height_bonus_currency / 2f)
        {
            EnableBonusCurrency ();

            return true;
        }

        return false;
    }

    public void EnableBonusCurrency ()
    {
        if (_BonusCurrencyId == BonusEnums.BonusId.FreeCash)
        {
            OnWatchAdsCompleted (_BonusCurrencyId, _CurrencyData.GetQuantity (_BonusCurrencyId));

            var position = position_bonus_coins;

            position.y -= 1f;

            GameActionManager.Instance.InstanceFxTapCoins (position);
            GameActionManager.Instance.InstanceFxTapFlower (position);

            this.PlayAudioSound (AudioEnums.SoundId.BalloonClick);

            return;
        }

        ApplicationManager.Instance.SetDialog (this);

        EnableHud ();

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
    }

    public void EnableHud ()
    {
        RefreshLanguage ();
        GameManager.Instance.DisableTouch ();

        IsEnable = true;

        transform_hub_reward.gameObject.SetActive (true);
    }

    public void DisableHud ()
    {
        GameManager.Instance.EnableTouch ();
        ApplicationManager.Instance.UnSetDialog (this);

        IsEnable = false;

        transform_hub_reward.gameObject.SetActive (false);
    }


    public void RandomBonus ()
    {
        _BonusId = _BonusData.GetRandomBonus ();
    }

    public void RandomBonusCurrency ()
    {
        _BonusCurrencyId = _CurrencyData.GetRandomBonus ();
    }

    #endregion

    #region Callback

    private void FxMovingBonusOnOnMovingEnd ()
    {
        RandomBonusCurrency ();
        UpdateReward ();
    }

    private void OnInteractUI (bool state)
    {
        IsReady = state;
    }

    private void OnWatchAdsCompleted (BonusEnums.BonusId id, int quantity)
    {
        switch (id)
        {
            case BonusEnums.BonusId.X5Cash:
                OnGetCashCurrency (quantity);
                break;
            case BonusEnums.BonusId.FreeDiamonds:
                OnGetDiamondCurrency (quantity);
                break;
            case BonusEnums.BonusId.FreeCash:
                OnGetCashCurrency (quantity);
                break;
        }

        PlayerData._LastTimeAppearBonusCurrency = Helper.GetUtcTimeString ();
        PlayerData.SaveLastTimeAppearBonusCurrency ();

        RefreshAppearBonusCurrency ();

        DisableHud ();

        this.PostMissionEvent (MissionEnums.MissionId.GetBonus);
    }

    private void OnGetCashCurrency (int quantity)
    {
        if (EarningManager.InstanceAwake () == null)
        {
            return;
        }

        var profit      = EarningManager.Instance.ProfitPerSec;
        var profit_unit = EarningManager.Instance.ProfitUnit;

        if (profit_unit == 0 && profit < 1)
        {
            profit      = GameConfig.DefaultCoinEarn;
            profit_unit = 0;
        }

        profit = profit * quantity;

        Helper.FixUnit (ref profit, ref profit_unit);

        GameManager.Instance.FxEarnCoin (profit, profit_unit, transform_bonus_coins.position);
    }

    private void OnGetDiamondCurrency (int quantity)
    {
        if (GameActionManager.Instance != null)
            GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                           UIGameManager.Instance.GetPositionHubDiamonds (),
                                                           quantity);
        else
        {
            PlayerData.Diamonds += quantity;
            PlayerData.SaveDiamonds ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
        }
    }

    #endregion

    #region IEnumerator

    private IEnumerator<float> _EnumeratorAppearBonusCurrency ()
    {
        var total_Time = (DateTime.Parse (PlayerData._LastTimeAppearBonusCurrency) - Helper.GetUtcTime ()).TotalSeconds + GameConfig.MaxTimeWaitingAppearCurrency;

        if (total_Time < 0)
            total_Time = 0;

        is_enabled_bonus_currency = false;

        transform_bonus_coins.DOScale (0, Durations.DurationScale).SetEase (Ease.InBack);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        transform_bonus_coins.gameObject.SetActive (false);

        yield return Timing.WaitForSeconds ((float) total_Time);

        transform_bonus_coins.gameObject.SetActive (true);

        transform_bonus_coins.DOComplete ();
        transform_bonus_coins.DOScale (1.2f, Durations.DurationScale);

        transform_bonus_coins.localScale = Vector.Vector3Zero;

        is_enabled_bonus_currency = true;

        RandomBonusCurrency ();
        UpdateReward ();
    }

    #endregion

    #region Interact

    public void InteractClose ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        DisableHud ();
    }

    public void InteractWatchAdsForReward ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (() => { Instance.OnWatchAdsCompleted (Instance._BonusCurrencyId, Instance._CurrencyData.GetQuantity (Instance._BonusCurrencyId)); }, null);

            return;
        }
        else
        {
            this.RefreshRewardVideo ();
        }

        ApplicationManager.Instance.AlertNoAdsAvailable ();
    }

    #endregion

    private void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus == false)
        {
            RefreshAppearBonusCurrency ();
        }
    }
}