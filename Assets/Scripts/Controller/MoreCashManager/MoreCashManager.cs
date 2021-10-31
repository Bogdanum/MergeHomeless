using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class MoreCashManager : Singleton<MoreCashManager>, IDialog
{
    [SerializeField] private Transform transform_hub;
    [SerializeField] private Text      text_time_duration;
    [SerializeField] private Image     time_process_bar;

    [Header ("Language")] [SerializeField] private Text text_watch_ads;
    [SerializeField]                       private Text label_get_profit;

    [SerializeField] private Button button_watch_ads;

    #region Variables

    private bool IsUpdate     = false;
    private bool IsReadyToUse = false;
    private bool IsEnable;

    private int _CurrentTime;

    public event System.Action<bool>   HandleStateTimeHub;
    public event System.Action<string> HandleStateTimeText;

    private CoroutineHandle handle_multi_reward_coins;

    #endregion

    #region Controller

    private void InitIEnumerator ()
    {
        Timing.RunCoroutine (IUpdate ());
    }

    #endregion

    #region Helper

    public bool IsEnableMoreCash ()
    {
        return PlayerData.TotalTimeMoreCash > 1;
    }

    #endregion

    #region Action

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);
        transform_hub.gameObject.SetActive (true);
        IsEnable = true;
        UpdateProcessTime ();
        GameManager.Instance.DisableTouch ();

        RefreshLanguage ();
        RefreshWatchAds ();
    }

    public void DisableHud ()
    {
        GameManager.Instance.EnableTouch ();

        transform_hub.gameObject.SetActive (false);
        ApplicationManager.Instance.UnSetDialog (this);

        IsEnable = false;
    }

    public void RefreshLanguage ()
    {
        text_watch_ads.text   = ApplicationLanguage.Text_label_gimme_that;
        label_get_profit.text = string.Format (ApplicationLanguage.Text_label_get_x_profit, GameConfig.RevenueCanGetFromMoreCash.ToString ());
    }

    public void RefreshWatchAds ()
    {
        button_watch_ads.interactable = PlayerData.TotalTimeMoreCash < GameConfig.TimeMoreCash / 2f;
    }

    #endregion

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
        PlayerData.SaveTotalTimeMoreCash ();

        base.OnDestroy ();
    }

    #endregion

    #region Controller

    private void InitConfig ()
    {
        IsUpdate = true;
    }

    private void InitTime ()
    {
        var time      = (DateTime.UtcNow - DateTime.Parse (PlayerData._LastTimeForMoreCash)).TotalSeconds;
        var last_time = PlayerData.TotalTimeMoreCash - time;

        if (last_time < 0)
        {
            PlayerData.TotalTimeMoreCash = 0;
        }
        else
        {
            PlayerData.TotalTimeMoreCash = PlayerData.TotalTimeMoreCash - (int) time;
        }

        PlayerData.SaveTotalTimeMoreCash ();

        _CurrentTime = PlayerData.TotalTimeMoreCash;

        if (PlayerData.TotalTimeMoreCash > 0)
        {
            IsReadyToUse = true;

            this.PostActionEvent (ActionEnums.ActionID.MoreCash, true);
        }
        else
        {
            IsReadyToUse = false;

            this.PostActionEvent (ActionEnums.ActionID.MoreCash, false);

            DisableTimeProcess ();
        }

        RefreshMoreCashValue ();
    }

    #endregion

    #region Action

    public void RegisterMoreCashHub (System.Action<bool> action)
    {
        HandleStateTimeHub += action;
    }

    public void RegisterMoreCashTime (System.Action<string> action)
    {
        HandleStateTimeText += action;
    }

    public void UnRegisterMoreCashHub (System.Action<bool> action)
    {
        HandleStateTimeHub -= action;
    }

    public void UnRegisterMoreCashTime (System.Action<string> action)
    {
        HandleStateTimeText -= action;
    }


    public void AddMoreCashTime (int value)
    {
        if (PlayerData.TotalTimeMoreCash <= 0)
        {
            PlayerData._LastTimeForMoreCash = Helper.GetUtcTimeString ();
            PlayerData.SaveTimeForMoreCash ();
        }

        PlayerData.TotalTimeMoreCash = Mathf.Clamp (PlayerData.TotalTimeMoreCash + value, 0, (int) GameConfig.TimeMoreCash);

        _CurrentTime = PlayerData.TotalTimeMoreCash;

        PlayerData.SaveTotalTimeMoreCash ();

        UpdateProcessTime ();
        EnableTimeProcess ();

        IsReadyToUse = true;

        RefreshMoreCashValue ();

        this.PostActionEvent (ActionEnums.ActionID.MoreCash, true);
        this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
    }

    public void UpdateProcessTime ()
    {
        var stringSpeedTime = Helper.ConvertToTime (_CurrentTime);

        if (IsEnable)
        {
            time_process_bar.fillAmount = (float) _CurrentTime / GameConfig.TimeMoreCash;
            text_time_duration.text     = stringSpeedTime;
        }

        if (!ReferenceEquals (HandleStateTimeText, null)) HandleStateTimeText (stringSpeedTime);
    }

    public void EnableTimeProcess ()
    {
        if (!ReferenceEquals (HandleStateTimeHub, null))
        {
            HandleStateTimeHub (true);
        }
    }

    public void DisableTimeProcess ()
    {
        if (!ReferenceEquals (HandleStateTimeHub, null))
        {
            HandleStateTimeHub (false);
        }
    }

    public void RefreshMoreCashValue ()
    {
        if (PlayerData.TotalTimeMoreCash > 0)
        {
            Contains.MultiRewardFromCoins = GameConfig.RevenueCanGetFromMoreCash - 1;
        }
        else
        {
            Contains.MultiRewardFromCoins = 0;
        }
    }

    public void RefreshMultiRewardCoins ()
    {
        var total_time_reward_multi = (DateTime.Parse (PlayerData._LastTimeMultiRewardCoins) - Helper.GetUtcTime ()).TotalSeconds + PlayerData.TotalTimeMultiRewardCoins;

        if (total_time_reward_multi < 0)
        {
            total_time_reward_multi = 0;
        }

        if (total_time_reward_multi > 0)
        {
            Timing.KillCoroutines (handle_multi_reward_coins);
            handle_multi_reward_coins = Timing.RunCoroutine (ITimeRewards ((float) total_time_reward_multi));

            UIGameManager.InstanceAwake ().SetStateFxXCoins (true);
        }
        else
        {
            PlayerData.TotalTimeMultiRewardCoins = 0;
            PlayerData.SaveTotalTimeMultiRewardCoins ();

            UIGameManager.InstanceAwake ().SetStateFxXCoins (false);
            GameActionManager.InstanceAwake ().SetMultiRewardCoins (0);
        }
    }

    #endregion

    #region Enumerator

    private IEnumerator<float> ITimeRewards (float time)
    {
        yield return Timing.WaitForSeconds (time + 1f);

        RefreshMultiRewardCoins ();
    }

    private IEnumerator<float> IUpdate ()
    {
        while (IsUpdate)
        {
            if (IsReadyToUse)
            {
                if (_CurrentTime == 0)
                {
                    IsReadyToUse = false;

                    PlayerData.TotalTimeMoreCash = 0;

                    PlayerData.SaveTotalTimeMoreCash ();

                    DisableTimeProcess ();

                    this.PostActionEvent (ActionEnums.ActionID.MoreCash, false);
                    this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
                }
                else
                {
                    _CurrentTime--;

                    PlayerData.TotalTimeMoreCash = _CurrentTime;

                    UpdateProcessTime ();
                }
            }

            yield return Timing.WaitForSeconds (1f);
        }
    }

    #endregion

    #region Callback

    private void OnWatchAdsCompleted ()
    {
        AddMoreCashTime (GameConfig.TimeMoreCash);

        RefreshWatchAds ();
    }

    #endregion

    #region Interact

    public void InteractWatchAds ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (() =>
            {                Instance.OnWatchAdsCompleted ();

            }, null);
        }
        else
        {
            this.RefreshRewardVideo ();

            ApplicationManager.Instance.AlertNoAdsAvailable ();

            // -- add 20.10.2021
            this.ExecuteRewardAds(() =>
            {
                Instance.OnWatchAdsCompleted();

            }, null);
            // --
        }
    }

    public void InteractClose ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        DisableHud ();
    }

    #endregion
}