using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIGameManager : Singleton<UIGameManager>
{
    [Header ("UI")] [SerializeField] private Text      _TextDiamonds;
    [SerializeField]                 private Transform transform_diamonds;

    [SerializeField] private Text _TextCoca,
                                  _TextLevel;

    [SerializeField] private TextMesh _TextCoins, _TextProfitPerSec;

    [SerializeField] private Transform transform_box;
    [SerializeField] private Text      label_value_cash_x;

    [Header ("Level Hub")] [SerializeField]
    private Image _LevelProcess;

    [Header ("Hub")] [SerializeField] private Transform _TransformHubLevel,
                                                        _TransformHubDiamonds,
                                                        _TransformHubCoca;

    [Header ("Level")] [SerializeField] private Text _TextExp;

    [Header ("Fx")] [SerializeField] private Transform      _FxMissionAura;
    [SerializeField]                 private Transform      icon_new_mission;
    [SerializeField]                 private ParticleSystem FxMultiCoins;
    [SerializeField]                 private ParticleSystem FxSpeedUp;
    [SerializeField]                 private ParticleSystem FxX5Coins;

    [Header ("Lucky Wheel")] [SerializeField]
    private Transform transform_icon_lucky_wheel;

    [SerializeField] private Animation animation_control_icon_lucky_wheel;

    [SerializeField] private AnimationClip animation_clip_icon_lucky_wheel;

    [Header ("Button")] [SerializeField] private Transform transform_speed_up_button;
    [SerializeField]                     private Transform transform_equipments_button;
    [SerializeField]                     private Transform transform_shop_button;

    [Header ("Notice")] [SerializeField] private Transform transform_notice_equipments;
    [SerializeField]                     private Transform transform_notice_shop;

    [Header ("Items")] [SerializeField] private Text  item_price_buy;
    [SerializeField]                    private Image item_icon;

    #region Variables

    private double _CoinDisplay;

    private float _CurrentProcessTimeGetReward;
    private float _MaxTimeGetReward;

    private bool _IsReady;
    private bool _IsRunningAnimation;
    private bool _IsInteractReady;

    private int _LastDiamondUpdate;

    private CoroutineHandle _HandleAnimationLoadDiamonds;

    private Vector3 position_box;

    private bool is_max_time_touch;

    private double price;
    private int    price_unit;

    private double price_data;
    private int    price_unit_data;

    private float price_buy_coefficient;

    private double discount_prices;

    private int number_buy_times;

    private int level_buying;

    #endregion

    #region Handle

    private System.Action         HandleOnUpdateTime;
    private System.Action<object> HandleOnRefreshCoins;
    private System.Action<object> HandleOnRefreshCoca;
    private System.Action<object> HandleOnRefreshDiamonds;
    private System.Action<object> HandleStateInteractUi;
    private System.Action<object> HandleStateRefreshLanguage;
    private System.Action<object> HandleStateMissionComplete;
    private System.Action<object> HandleSpeedUp;
    private System.Action<object> HandleMoreCash;

    #endregion

    public static bool isSpeenWheel;

    #region System

    private void Start ()
    {
        InitConfig ();
        InitParameter ();
        InitDataBuy (1);

        RegisterTimeUpdate ();

        UpdateTextCoins ();
        UpdateTextDiamonds ();
        UpdateTextCoca ();
        UpdateTextLevel ();
        UpdateTextExp ();

        RegisterAction ();

        OnRefreshLanguage (null);
        OnMissionCompleted (null);
    }

    protected override void OnDestroy ()
    {
        UnRegisterTimeUpdate ();
        UnRegisterAction ();

        base.OnDestroy ();
    }

    #endregion

    #region CallBack

    private void OnUpdateTime ()
    {
        if (is_max_time_touch || isSpeenWheel)
            return;

        _CurrentProcessTimeGetReward -= Time.deltaTime;

        if (_CurrentProcessTimeGetReward < 0.1f)
        {
            _CurrentProcessTimeGetReward = 0;

            if (!is_max_time_touch)
            {
                GetRewardCraft ();
            }
        }
    }

    private void OnRefreshCoins (object obj)
    {
        UpdateTextCoins ();
    }

    private void OnRefreshCoca (object obj)
    {
        UpdateTextCoca ();
    }

    public void OnRefreshDiamonds (object obj)
    {
        UpdateTextDiamonds ();
    }

    public void OnRefreshInteractUi (bool state)
    {
        _IsInteractReady = state;
    }

    private void OnRefreshLanguage (object param)
    {
        UpdateTextProfitPerSec ();
    }

    private void OnMissionCompleted (object param)
    {
        var isCompleted = MissionManager.Instance.IsHaveMissionCompleted ();

        if (_FxMissionAura != null && _FxMissionAura.gameObject.activeSelf != isCompleted)
        {
            _FxMissionAura.gameObject.SetActive (isCompleted);
        }

        if (icon_new_mission != null && icon_new_mission.gameObject.activeSelf != isCompleted)
        {
            icon_new_mission.gameObject.SetActive (isCompleted);
        }
    }

    private void OnSpeedUp (bool state)
    {
        SetStateFxSpeedUp (state);
    }

    private void OnMoreCash (bool state)
    {
        SetStateMoreCash (state);
    }

    #endregion

    #region Controller

    private void RegisterAction ()
    {
        HandleOnRefreshCoins       = OnRefreshCoins;
        HandleOnRefreshCoca        = OnRefreshCoca;
        HandleOnRefreshDiamonds    = OnRefreshDiamonds;
        HandleStateInteractUi      = param => Instance.OnRefreshInteractUi ((bool) param);
        HandleStateRefreshLanguage = OnRefreshLanguage;
        HandleStateMissionComplete = OnMissionCompleted;
        HandleSpeedUp              = param => Instance.OnSpeedUp ((bool) param);
        HandleMoreCash             = param => Instance.OnMoreCash ((bool) param);

        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUICoins, HandleOnRefreshCoins);
        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUICoca, HandleOnRefreshCoca);
        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUIDiamonds, HandleOnRefreshDiamonds);
        this.RegisterActionEvent (ActionEnums.ActionID.SetStateInteractUI, HandleStateInteractUi);
        this.RegisterActionEvent (ActionEnums.ActionID.RefreshLanguage, HandleStateRefreshLanguage);
        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUICompleteMission, HandleStateMissionComplete);
        this.RegisterActionEvent (ActionEnums.ActionID.SpeedUp, HandleSpeedUp);
        this.RegisterActionEvent (ActionEnums.ActionID.MoreCash, HandleMoreCash);
    }


    private void UnRegisterAction ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUICoins, HandleOnRefreshCoins);
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUICoca, HandleOnRefreshCoca);
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUIDiamonds, HandleOnRefreshDiamonds);
        this.RemoveActionEvent (ActionEnums.ActionID.SetStateInteractUI, HandleStateInteractUi);
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshLanguage, HandleStateRefreshLanguage);
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUICompleteMission, HandleStateMissionComplete);
        this.RemoveActionEvent (ActionEnums.ActionID.SpeedUp, HandleSpeedUp);
        this.RemoveActionEvent (ActionEnums.ActionID.MoreCash, HandleMoreCash);
    }

    #endregion

    #region Action

    private void InitConfig ()
    {
        _MaxTimeGetReward            = 12f;
        _CurrentProcessTimeGetReward = _MaxTimeGetReward;
    }

    private void InitParameter ()
    {
        _CoinDisplay = PlayerData.Coins;

        isSpeenWheel        = false;
        _IsReady            = true;
        _IsInteractReady    = true;
        _IsRunningAnimation = false;

        position_box = item_price_buy.transform.position;

        level_buying = 0;
    }

    private void RegisterTimeUpdate ()
    {
        HandleOnUpdateTime = OnUpdateTime;

        if (TimeManager.InstanceAwake () != null)
            TimeManager.Instance.RegisterTimeUpdate (HandleOnUpdateTime);
    }

    private void UnRegisterTimeUpdate ()
    {
        if (TimeManager.InstanceAwake () != null)
            TimeManager.Instance.UnregisterTimeUpdate (HandleOnUpdateTime);
    }

    public void UpdateTextLevel ()
    {
        if (_TextLevel != null) _TextLevel.text = PlayerData.Level.ToString ();
    }

    public void UpdateTextExp ()
    {
        if (Contains.ExpNeedReach > 0)
        {
            if (_LevelProcess != null)
            {
                _LevelProcess.DOComplete ();
                _LevelProcess.DOFillAmount ((float) PlayerData.Exp / Contains.ExpNeedReach, Durations.DurationFillAmount);
            }

            _TextExp.text = string.Format ("{0}/{1}", PlayerData.Exp.ToString (), Contains.ExpNeedReach.ToString ());
        }
        else
        {
            if (_LevelProcess != null) _LevelProcess.fillAmount = 1;

            _TextExp.text = "MAX";
        }
    }

    public void UpdateTextCoca (bool IsAnimation = false)
    {
        if (_TextCoca != null) _TextCoca.text = ApplicationManager.Instance.GetPlayerCocaText (PlayerData.Coca, PlayerData.CocaUnit);
    }

    public void UpdateTextDiamonds (bool IsAnimation = false)
    {
        if (IsAnimation)
        {
            Timing.KillCoroutines (_HandleAnimationLoadDiamonds);
            _HandleAnimationLoadDiamonds = Timing.RunCoroutine (_AnimationLoadDiamonds (), SceneEnums.GetSceneString (SceneEnums.SceneID.SceneDev));
        }
        else
        {
            if (_TextDiamonds != null) _TextDiamonds.text = PlayerData.Diamonds.ToString ();
        }
    }

    public void UpdateTextCoins (bool IsAnimation = false)
    {
        if (!IsAnimation)
        {
            if (!ReferenceEquals (_TextCoins, null))
            {
                _TextCoins.text = ApplicationManager.Instance.GetPlayerCoinsText (PlayerData.Coins, PlayerData.CoinUnit);
            }
        }
        else
        {
            if (_IsRunningAnimation)
            {
                return;
            }

            _IsRunningAnimation = true;
        }
    }

    public void UpdateTextProfitPerSec ()
    {
        if (_TextProfitPerSec != null) _TextProfitPerSec.text = string.Format (ApplicationLanguage.Text_description_earning_each_sec, EarningManager.Instance.GetProfitPerSec ());
    }

    public void EnableLuckyWheel ()
    {
        animation_control_icon_lucky_wheel.Play (animation_clip_icon_lucky_wheel.name);
    }

    public void DisableLuckyWheel ()
    {
        transform_icon_lucky_wheel.eulerAngles = Vector.Vector3Zero;
        animation_control_icon_lucky_wheel.Stop ();
    }


    public void SetStateMoreCash (bool state)
    {
        if (state)
        {
            FxMultiCoins.Play ();
        }
        else
        {
            FxMultiCoins.Stop ();
        }
    }

    public void SetStateFxXCoins (bool state)
    {
        if (state)
        {
            FxX5Coins.Play ();
        }
        else
        {
            FxX5Coins.Stop ();
        }
    }

    public void SetStateFxSpeedUp (bool state)
    {
        if (state)
        {
            FxSpeedUp.Play ();
        }
        else
        {
            FxSpeedUp.Stop ();
        }
    }

    public void SetStateNoticeEquipments (bool state)
    {
        if (transform_notice_equipments.gameObject.activeSelf == !state)
        {
            transform_notice_equipments.gameObject.SetActive (state);
        }
    }

    public void SetStateNoticeShop (bool state)
    {
        if (transform_notice_shop.gameObject.activeSelf == !state)
        {
            transform_notice_shop.gameObject.SetActive (state);
        }
    }

    public void InitDataBuy (int level)
    {
        if (level == 0)
            level = 1;

        if (level != level_buying)
        {
            level_buying = level;

            var data = GameData.Instance.ItemDataGroup.GetDataItemWithLevel (level_buying);

            price_data      = data.Prices;
            price_unit_data = data.PricesUnit;

            price_buy_coefficient = data.PricesCoefficient;

            item_icon.sprite = GameData.Instance.ItemNodeIdleImage.GetIcon (level_buying);
        }

        RefreshPriceBuy ();
        UpdatePriceBuy ();
    }

    public void UpdatePriceBuy ()
    {
        item_price_buy.text = ApplicationManager.Instance.AppendFromCashUnit (price, price_unit);
    }

    public void RefreshPriceBuy ()
    {
        var UpgradeData   = GameData.Instance.EquipmentData.GetData (EquipmentEnums.AbilityId.DiscountBuy);
        var level_upgrade = PlayerData.GetEquipmentUpgrade (EquipmentEnums.AbilityId.DiscountBuy);

        discount_prices = Math.Pow (UpgradeData.UpgradeCoefficient, level_upgrade) / 100f;

        number_buy_times = PlayerData.GetNumberBuyItemProfitCoefficient (level_buying);

        if (level_upgrade == 0)
        {
            discount_prices = 0;
        }

        price = price_data - price_data * discount_prices;
        price = price * Math.Pow (price_buy_coefficient, number_buy_times);

        price_unit = price_unit_data;

        Helper.FixUnit (ref price, ref price_unit);
    }

    public void RefreshRandomRewardCondition ()
    {
        if (!is_max_time_touch)
            return;

        is_max_time_touch = GameManager.Instance.IsFreeIndexGrid () == false;
    }

    #endregion

    #region Callbacks

    private void OnBuyCompleted ()
    {
        OnIncreaseNumberTimeBuy ();

        GameManager.Instance.SetBoxReward (level_buying);

        this.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

    }

    private void OnIncreaseNumberTimeBuy ()
    {
        number_buy_times++;

        price = price * price_buy_coefficient;

        Helper.FixUnit (ref price, ref price_unit);

        PlayerData.SaveItemProfitCoefficient (level_buying, number_buy_times);

        UpdatePriceBuy ();
    }

    #endregion

    #region Emulator

    private IEnumerator<float> _AnimationLoadCoins ()
    {
        yield break;
    }

    private IEnumerator<float> _AnimationLoadDiamonds ()
    {
        yield return Timing.WaitForOneFrame;

        if (_TextDiamonds != null) _TextDiamonds.text = PlayerData.Diamonds.ToString ();
    }

    #endregion

    #region Helper

    public Vector3 GetPositionShopButton ()
    {
        return transform_shop_button.position;
    }

    public Vector3 GetPositionEquipmentsButton ()
    {
        return transform_equipments_button.position;
    }

    public Vector3 GetPositionSpeedUpButton ()
    {
        return transform_speed_up_button.position;
    }

    public Vector3 GetPositionHubDiamonds ()
    {
        return _TransformHubDiamonds.position;
    }

    public Vector3 GetPositionHubCoca ()
    {
        return _TransformHubCoca.position;
    }

    public Vector3 GetPositionHubExp ()
    {
        return _TextLevel.transform.position;
    }

    public Vector3 GetPositionHubCoins ()
    {
        return _TextCoins.transform.position;
    }

    public Vector3 GetPositionTouchBox ()
    {
        return item_icon.transform.position;
    }

    #endregion

    #region Fx

    private void FxShakeBox ()
    {
        transform_box.DOComplete ();
        transform_box.DOShakeScale (Durations.DurationShake, 0.2f);
    }

    public void FxShakeDiamonds ()
    {
        transform_diamonds.DOComplete ();
        transform_diamonds.DOShakeScale (Durations.DurationShake, 0.2f);
    }

    #endregion

    #region Interface Interact

    public void InteractBuyItems ()
    {
        // =============================== Buy the cats ================================ //

        this.PlayAudioSound (AudioEnums.SoundId.TapOnCraft);

        GameActionManager.Instance.InstanceFxTapBox (position_box);

        FxShakeBox ();

        if (GameManager.Instance.IsFreeIndexGrid () == false)
        {
            ApplicationManager.Instance.AlertNoMorePark ();

            return;
        }


        if (!ApplicationManager.Instance.IsMinusCurrency (price, price_unit, CurrencyEnums.CurrencyId.Cash))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (CurrencyEnums.CurrencyId.Cash);

            return;
        }

        OnBuyCompleted ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
    }

    private bool GetRewardCraft ()
    {
        if (!GameManager.Instance.IsFreeIndexGrid ())
        {
            is_max_time_touch = true;

            return false;
        }

        GameManager.Instance.SetRandomTouchBoxReward ();
        _CurrentProcessTimeGetReward = _MaxTimeGetReward;

        is_max_time_touch = false;

        return true;
    }

    public void OpenShop ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN THE SHOP ================================ //
        if (ShopManager.Instance != null) ShopManager.Instance.EnableHud ();
    }

    public void OpenKittyShop ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN THE SHOP ================================ //
        if (ShopManager.Instance != null) ShopManager.Instance.EnableItemShop ();

        this.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

        SetStateNoticeShop (false);
    }

    public void OpenUpgradeShop ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN THE SHOP ================================ //
        if (ShopManager.Instance != null) ShopManager.Instance.EnableEquipmentShop ();

        this.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

        SetStateNoticeEquipments (false);
    }

    public void OpenSpeedUp ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN HUD SPEED UP ================================ //
        if (SpeedUpManager.Instance != null) SpeedUpManager.Instance.EnableHud ();

        this.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);
    }

    public void OpenTradeCoca ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN TRADE COCA ================================ //
        if (CocaManager.Instance != null) CocaManager.Instance.EnableHud ();
    }

    public void OpenDiamondsShop ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN DIAMOND SHOP ================================ //
        if (DiamondManager.Instance != null) DiamondManager.Instance.EnableHud ();
    }

    public void OpenWheelLucky ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (ApplicationManager.Instance.AlertLevelRequire (GameConfig.UnlockLuckyWheelLevel))
        {
            return;
        }

        // =============================== OPEN WHEEL LUCKY ================================ //
        if (WheelLuckyManager.Instance != null) WheelLuckyManager.Instance.EnableWheelLuckyManager ();
    }

    public void OpenSettings ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== OPEN SETTINGS ================================ //
        if (SettingManager.Instance != null) SettingManager.Instance.EnableHud ();
    }

    public void OpenMission ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (ApplicationManager.Instance.AlertLevelRequire (GameConfig.UnlockMissionLevel))
        {
            return;
        }

        // =============================== Open the mission hud ================================ //

        if (UIMissionManager.Instance != null) UIMissionManager.Instance.EnableHud ();
    }

    public void InteractMoreCash ()
    {
        if (!_IsInteractReady)
            return;
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (MoreCashManager.Instance != null) MoreCashManager.Instance.EnableHud ();
    }

    public void InteractFoodShop ()
    {
        if (!_IsInteractReady)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== Open the food shop ================================ //

    }

    public void InteractBoostShop ()
    {
        if (!_IsInteractReady)
            return;
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (ApplicationManager.Instance.AlertLevelRequire (GameConfig.UnlockBoosterLevel))
        {
            return;
        }

        // =============================== Open the boost shop ================================ //

        if (BoostManager.Instance != null) BoostManager.Instance.EnableHud ();
    }

    #endregion
}