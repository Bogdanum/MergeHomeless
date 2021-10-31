using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WheelLuckyManager : Singleton<WheelLuckyManager>, IDialog
{
    [Header ("UI")] [SerializeField] private UIWheelLucky _UiWheelLucky;

    [SerializeField] private WheelLuckyData _WheelLuckyData;
    [SerializeField] private float          _SpeedRotation;
    [SerializeField] private int            _MaxRoundRotation;
    [SerializeField] private int            _MaxPartItem;

    public bool IsReadyToGetReward;

    private bool IsBusy;
    private bool IsUpdate;
    private bool IsEnable;
    private bool _IsReadyUpdate;

    private RewardEnums.RewardId _RewardId;
    private int                  _TimeForWaiting;


    protected override void Awake ()
    {
        base.Awake ();

        InitConfig ();
        InitIEnumerator ();
        RefreshTime ();
    }

    protected override void OnDestroy ()
    {
        IsUpdate = false;

        base.OnDestroy ();
    }

    #region Controller

    private void InitIEnumerator ()
    {
        Timing.RunCoroutine (IUpdate ());
    }

    private void InitConfig ()
    {
        IsUpdate = true;

        _UiWheelLucky.InitIcons (_WheelLuckyData);
    }

    #endregion

    #region Action

    public void EnableWheelLuckyManager ()
    {
        ApplicationManager.Instance.SetDialog (this);

        EnableHud ();
    }

    public void DisableWheelLuckyManager ()
    {
        if (IsBusy)
            return;

        ApplicationManager.Instance.UnSetDialog (this);

        DisableHud ();
    }

    public void EnableHud ()
    {
        _UiWheelLucky.EnableHud ();

        GameManager.Instance.DisableTouch ();

        RefreshTime ();

        IsEnable = true;
    }

    public void DisableHud ()
    {
        _UiWheelLucky.Close ();

        GameManager.Instance.EnableTouch ();

        IsEnable = false;
    }

    public void DoSpinLuckyGem ()
    {
        if (IsBusy)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (PlayerData._LastNumberTurnSpin == 0)
            return;

        if (!ApplicationManager.Instance.IsMinusCurrency (GameConfig.DiamondsForSpin, 0, CurrencyEnums.CurrencyId.Diamonds))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (CurrencyEnums.CurrencyId.Diamonds);

            return;
        }

        DoSpin ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
    }

    public void DoSpinLuckyWheel ()
    {
        if (GameManager.Instance.IsFreeIndexGrid() == false)
        {
            ApplicationManager.Instance.AlertNoMorePark();

            return;
        }

        UIGameManager.isSpeenWheel = true;

        if (IsBusy)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
        
        if (PlayerData._LastNumberTurnSpin == 0)
        {
            return;
        }

        _UiWheelLucky.SetInteractLuckyButton ();

        if (PlayerData.IsWatchAdsForSpin)
        {
            DoSpin ();
            return;
        }

        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (() =>
            {
                PlayerData.SaveSateWatchAdsForSpin (true);
                Instance._UiWheelLucky.RefreshLanguageSpin ();

            }, null);
            return;
        }
        else
        {
            this.RefreshRewardVideo ();
        }
      
        ApplicationManager.Instance.AlertNoAdsAvailable ();
    }

    private void DoSpin ()
    {
        
        _RewardId = _WheelLuckyData.GetRandomReward ();
        var index      = _WheelLuckyData.GetIndexReward (_RewardId);
        var angle_item = 360 / _MaxPartItem;
        var angle      = _MaxRoundRotation * 360 + angle_item * index + angle_item / 2f;

        _UiWheelLucky.SetRotation (angle, _SpeedRotation);

        if (PlayerData._LastNumberTurnSpin == GameConfig.MaxNumberTurnForSpin)
        {
            PlayerData._LastTimeSpinLucky = Helper.GetUtcTimeString ();
            PlayerData.SaveLastTimeSpinLucky ();
        }

        PlayerData._LastNumberTurnSpin--;
        PlayerData.SaveNumberTurnSpin ();

        IsBusy = true;

        RefreshTime ();

        this.PostMissionEvent (MissionEnums.MissionId.Spin);

        this.PlayAudioSound (AudioEnums.SoundId.ClickSpin);
    }

    public void RefreshTime ()
    {
        var total_time = (Helper.GetUtcTime () - DateTime.Parse (PlayerData._LastTimeSpinLucky)).TotalSeconds;

        if (total_time <= 0)
        {
            total_time = 0;
        }

        if (total_time > GameConfig.NextTimeWaitingForSpin * GameConfig.MaxNumberTurnForSpin)
        {
            total_time = GameConfig.NextTimeWaitingForSpin * GameConfig.MaxNumberTurnForSpin;
        }

        var number_turn = (int) (total_time - total_time % GameConfig.NextTimeWaitingForSpin) / GameConfig.NextTimeWaitingForSpin;

        PlayerData._LastNumberTurnSpin = Mathf.Clamp (PlayerData._LastNumberTurnSpin + number_turn, 0, GameConfig.MaxNumberTurnForSpin);
        PlayerData.SaveNumberTurnSpin ();

        total_time = GameConfig.NextTimeWaitingForSpin - total_time;

        if (PlayerData._LastNumberTurnSpin == GameConfig.MaxNumberTurnForSpin)
        {
            total_time = 0;
        }

        if (PlayerData._LastNumberTurnSpin < GameConfig.MaxNumberTurnForSpin && total_time <= 0)
        {
            total_time = GameConfig.NextTimeWaitingForSpin;

            PlayerData._LastTimeSpinLucky = Helper.GetUtcTimeString ();
            PlayerData.SaveLastTimeSpinLucky ();
        }

        if (PlayerData._LastNumberTurnSpin > 0)
        {
            if (UIGameManager.InstanceAwake () != null)
            {
                UIGameManager.Instance.EnableLuckyWheel ();
            }
        }
        else
        {
            if (UIGameManager.InstanceAwake () != null)
            {
                UIGameManager.Instance.DisableLuckyWheel ();
            }
        }

        _TimeForWaiting = (int) total_time;
        _IsReadyUpdate  = _TimeForWaiting > 0 || PlayerData._LastNumberTurnSpin < GameConfig.MaxNumberTurnForSpin;

        _UiWheelLucky.RefreshTimeSpin (_IsReadyUpdate);
        _UiWheelLucky.UpdateTimeNextSpin (Helper.ConvertToTime (_TimeForWaiting));
        _UiWheelLucky.RefreshUINumberSpin ();
    }

    public void DoCompletedRotation ()
    {
        switch (_RewardId)
        {
            case RewardEnums.RewardId.X5RewardCoins:
                OnGetX5RewardCoins ();
                break;
            case RewardEnums.RewardId.Diamonds:
                OnGetDiamonds ();
                break;
            case RewardEnums.RewardId.SpeedUp:
                OnGetSpeedUp ();
                break;
            case RewardEnums.RewardId.Box:
                OnGetBox ();
                break;
            default:
                throw new ArgumentOutOfRangeException ();
        }
    }

    #endregion

    #region IEmunator

    private IEnumerator<float> IUpdate ()
    {
        while (IsUpdate)
        {
            if (_IsReadyUpdate && IsEnable)
            {
                if (_TimeForWaiting > 0)
                {
                    _TimeForWaiting--;
                }
                else
                {
                    _IsReadyUpdate  = false;
                    _TimeForWaiting = 0;
                    RefreshTime ();
                }

                if (IsEnable)
                {
                    _UiWheelLucky.UpdateTimeNextSpin (Helper.ConvertToTime (_TimeForWaiting));
                }
            }

            yield return Timing.WaitForSeconds (1f);
        }
    }

    #endregion

    #region Interaction

    public void TapToGetReward ()
    {
        if (!IsReadyToGetReward)
        {
            return;
        }
        
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        DoCompletedRotation ();

        _UiWheelLucky.CloseReward ();
        UIGameManager.isSpeenWheel = false;
    }

    #endregion

    #region Callback

    public void OnCompletedRotation ()
    {
        IsReadyToGetReward = false;

        _UiWheelLucky.EnableAnimationReward (_WheelLuckyData.GetIcon (_RewardId));

        _UiWheelLucky.GetDescriptionReward (_WheelLuckyData.GetDescription(_RewardId));

        PlayerData.SaveSateWatchAdsForSpin (false);

        _UiWheelLucky.RefreshLanguageSpin ();
#if !UNITY_EDITOR
        // Add 21.10.2021 ----------Firebase
        Firebase.Analytics.FirebaseAnalytics.LogEvent("spin_wheel", "spin_wheel", "true");
        // --------------------------------
#endif
    }

    private void OnGetX5RewardCoins ()
    {
        var get_quantity = _WheelLuckyData.GetQuantity (_RewardId);

        if (GameActionManager.Instance != null)
            GameActionManager.Instance.SetMultiRewardCoins (get_quantity);

        if (MoreCashManager.Instance != null)
            MoreCashManager.Instance.RefreshMultiRewardCoins ();

        OnGetRewardCompleted ();
    }

    private void OnGetDiamonds ()
    {
        var get_quantity = _WheelLuckyData.GetQuantity (_RewardId);

        if (GameActionManager.Instance != null)
            GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                           UIGameManager.Instance.GetPositionHubDiamonds (),
                                                           get_quantity);
        else
        {
            PlayerData.Diamonds += get_quantity;
            PlayerData.SaveDiamonds ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
        }

        OnGetRewardCompleted ();
    }

    private void OnGetSpeedUp ()
    {
        var get_quantity = _WheelLuckyData.GetQuantity (_RewardId);

        if (SpeedUpManager.Instance != null) SpeedUpManager.Instance.AddSpeedUP (get_quantity);

        OnGetRewardCompleted ();
    }

    private void OnGetBox ()
    {
        var get_quantity = _WheelLuckyData.GetQuantity (_RewardId);

        for (int i = 0; i < get_quantity; i++)
        {
            if (GameManager.Instance != null) GameManager.Instance.SetRandomReward ();
        }

        OnGetRewardCompleted ();
    }

    private void OnGetRewardCompleted ()
    {
        IsBusy = false;
    }

#endregion

    private void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus == false)
            RefreshTime ();
    }
}