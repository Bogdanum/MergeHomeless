using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpManager : Singleton<SpeedUpManager>, IDialog
{
    [Header ("UI")] [SerializeField] private UISpeedUpManager _UiSpeedUpManager;

    

    private bool IsReadyToUse     = false;
    private bool IsUpdate         = false;
    private bool IsDisplaySpeedUp = false;

    public static bool IsEnabled = false;
    
    private int _CurrentTime;

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitTime ();
        InitConfig ();
        InitIEnumerator ();
    }

    protected override void OnDestroy ()
    {
        IsUpdate = false;

        PlayerData.SaveTotalTimeSpeedUp ();

        base.OnDestroy ();
    }

    #endregion

    #region IEmurator

    private IEnumerator<float> IUpdate ()
    {
        while (IsUpdate)
        {
            if (IsReadyToUse)
            {
                if (_CurrentTime == 0)
                {
                    IsReadyToUse = false;

                    _UiSpeedUpManager.DisableTimeProcess ();

                    PlayerData.SaveTotalTimeSpeedUp ();

                    this.PostActionEvent (ActionEnums.ActionID.SpeedUp, false);
                    this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
                }
                else
                {
                    _CurrentTime--;

                    PlayerData.TotalTimeSpeedUp = _CurrentTime;
                    _UiSpeedUpManager.UpdateTimeProcess ();
                }
            }
            
            yield return Timing.WaitForSeconds (1f);
        }
    }

    #endregion

    #region Controller

    private void InitIEnumerator ()
    {
        Timing.RunCoroutine (IUpdate ());
    }

    private void InitTime ()
    {
        var time      = (DateTime.UtcNow - DateTime.Parse (PlayerData._LastTimeForSpeedUp)).TotalSeconds;
        var last_time = PlayerData.TotalTimeSpeedUp - (int) time;

        if (last_time < 0)
        {
            PlayerData.TotalTimeSpeedUp = 0;
        }
        else
        {
            PlayerData.TotalTimeSpeedUp = last_time;
        }

        PlayerData.SaveTotalTimeSpeedUp ();

        _CurrentTime = PlayerData.TotalTimeSpeedUp;

        if (_CurrentTime > 0)
        {
            IsReadyToUse = true;
            this.PostActionEvent (ActionEnums.ActionID.SpeedUp, true);
        }
        else
        {
            IsReadyToUse = false;
            this.PostActionEvent (ActionEnums.ActionID.SpeedUp, false);
            
            _UiSpeedUpManager.DisableTimeProcess ();
        }
        
        
        this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
    }

    private void InitConfig ()
    {
        IsUpdate = true;

        _UiSpeedUpManager.Init ();
        _UiSpeedUpManager.UpdateTimeProcess ();
    }

    #endregion

    #region Helper

    public bool IsEnableSpeedUp ()
    {
        return PlayerData.TotalTimeSpeedUp > 1;
    }

    public Vector3 GetPositionSpeedUpWithDiamonds ()
    {
        return _UiSpeedUpManager.GetPositionDiamonds ();
    }
    
    #endregion

    #region Action

    public void RegisterSpeedUpHub (System.Action<bool> action)
    {
        if (_UiSpeedUpManager != null) _UiSpeedUpManager.HandleStateSpeedUpHub += action;
    }

    public void RegisterSpeedUpTime (System.Action<string> action)
    {
        if (_UiSpeedUpManager != null) _UiSpeedUpManager.HandleStateSpeedUpTime += action;
    }

    public void UnRegisterSpeedUpHub (System.Action<bool> action)
    {
        if (_UiSpeedUpManager != null) _UiSpeedUpManager.HandleStateSpeedUpHub -= action;
    }

    public void UnRegisterSpeedUpTime (System.Action<string> action)
    {
        if (_UiSpeedUpManager != null) _UiSpeedUpManager.HandleStateSpeedUpTime -= action;
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _UiSpeedUpManager.EnableHub ();
        _UiSpeedUpManager.UpdateTimeProcess ();

        GameManager.Instance.DisableTouch ();

        IsDisplaySpeedUp = PlayerData.TotalTimeSpeedUp == 0;

        IsEnabled = true;
    }

    public void DisableHud ()
    {
        ApplicationManager.Instance.UnSetDialog (this);

        _UiSpeedUpManager.DisableHub ();

        GameManager.Instance.EnableTouch ();

        if (IsDisplaySpeedUp && PlayerData.TotalTimeSpeedUp > 0)
        {
            var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxUIDisplaySpeedUp, false);

            if (fx == null)
                return;

            fx.transform.position = Vector3.zero;
            fx.gameObject.SetActive (true);
        }

        IsEnabled = false;
    }

    public void FreeSpeedUp ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!this.IsRewardVideoAvailable ())
        {
            ApplicationManager.Instance.AlertNoAdsAvailable ();

            this.RefreshRewardVideo ();
            return;
        }

        this.ExecuteRewardAds (() =>
        {
            Instance.OnBuySpeedUpCompleted ();
        }, null);
    }

    public void BuySpeedUpDiamonds ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (GameConfig._DiamondBuySpeedUp > PlayerData.Diamonds)
            return;

        PlayerData.Diamonds -= GameConfig._DiamondBuySpeedUp;
        PlayerData.SaveDiamonds ();

        OnBuySpeedUpCompleted ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

        GameActionManager.Instance.InstanceFxTapDiamonds (_UiSpeedUpManager.GetPositionDiamonds ());
    }

    public void AddSpeedUP (int quantity)
    {
        if (PlayerData.TotalTimeSpeedUp <= 0)
        {
            PlayerData._LastTimeForSpeedUp = Helper.GetUtcTimeString ();
            PlayerData.SaveTimeForSpeedUp ();
        }

        PlayerData.TotalTimeSpeedUp = Mathf.Clamp (PlayerData.TotalTimeSpeedUp + quantity, 0, (int) GameConfig.MaxTimeSpeedUp);

        _UiSpeedUpManager.UpdateTimeProcess ();
        _UiSpeedUpManager.EnableTimeProcess ();

        _CurrentTime = PlayerData.TotalTimeSpeedUp;

        PlayerData.SaveTotalTimeSpeedUp ();

        IsReadyToUse = true;

        this.PostActionEvent (ActionEnums.ActionID.SpeedUp, true);
        this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
    }

    #endregion

    #region Callback

    private void OnBuySpeedUpCompleted ()
    {
        AddSpeedUP (GameConfig.TimeSpeedUp);
    }

    #endregion
}