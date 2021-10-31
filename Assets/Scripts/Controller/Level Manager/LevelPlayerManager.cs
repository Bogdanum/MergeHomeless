using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class LevelPlayerManager : Singleton<LevelPlayerManager>, IDialog
{
    [Header ("Transform")] [SerializeField]
    private Transform transform_hub;

    [SerializeField] private Transform transform_board;
    [SerializeField] private Transform transform_double_reward;
    [SerializeField] private Transform transform_close_reward;


    [Header ("UI")] [SerializeField] private Text text_level;

    [Header ("Animation")] [SerializeField]
    private Animation animation_rotation_level;

    [SerializeField] private AnimationClip clip_animation_level;

    [Header ("Reward")] [SerializeField] private Text reward_text_diamonds;

    [SerializeField] private Text reward_text_coins;

    [Header ("Language")] [SerializeField] private Text label_level_up;
    [SerializeField]                       private Text label_reward;
    [SerializeField]                       private Text label_reward_double;

    private bool                                is_animation_level;
    private RewardLevelup.RewardLevelUpProperty RewardLevelUpProperty;

    #region Actions

    public void Init (int level)
    {
        if (is_animation_level)
        {
            return;
        }

        is_animation_level = true;

        InitReward (level);

        Timing.RunCoroutine (Enumerator_Waiting_Open (level));
    }

    private void InitReward (int level)
    {
        RewardLevelUpProperty = GameData.Instance.RewardLevelup.GetReward (level);

        reward_text_diamonds.text = RewardLevelUpProperty.diamond_reward.ToString ();
        reward_text_coins.text    = ApplicationManager.Instance.AppendFromCashUnit (RewardLevelUpProperty.coins_reward, RewardLevelUpProperty.coins_unit_reward);
    }

    public void DisableHud ()
    {
        GameManager.Instance.EnableTouch ();
        transform_hub.gameObject.SetActive (false);
        ApplicationManager.Instance.UnSetDialog (this);

        ApplicationManager.Instance.CheckPlayerLevelRate ();
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);
        transform_hub.gameObject.SetActive (true);
        GameManager.Instance.DisableTouch ();

        RefreshLanguage ();
    }

    public void RefreshLanguage ()
    {
        label_level_up.text      = ApplicationLanguage.Text_label_level_up;
        label_reward.text        = ApplicationLanguage.Text_description_here_your_reward;
        label_reward_double.text = string.Format (ApplicationLanguage.Text_description_revenue_from_ads, "2");
    }

    #endregion

    #region Enumerator

    private IEnumerator<float> Enumerator_Waiting_Open (int level)
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        EnableHud ();
        Timing.RunCoroutine (enumerator_level_up (level));
    }

    private IEnumerator<float> enumerator_level_up (int level)
    {
        // Send Level to leaderboard
        PlayFabManager.Instance.SendLeaderboard(level);

        transform_board.localScale         = Vector3.zero;
        transform_double_reward.localScale = Vector3.zero;
        transform_close_reward.localScale  = Vector3.zero;

        transform_board.DOScale (Vector3.one, Durations.DurationScale);

        yield return Timing.WaitForSeconds (0.5f);

        this.PlayAudioSound (AudioEnums.SoundId.PlayerLevelUp);

        text_level.text = (level - 1).ToString ();

        var transform_text_level = text_level.transform;

        transform_text_level.localScale = new Vector3 (0, 0, 0);

        transform_text_level.DOScale (2.5f, Durations.DurationScale).SetEase (Ease.OutBack);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        animation_rotation_level.enabled = true;

        animation_rotation_level.Play (clip_animation_level.name);

        yield return Timing.WaitForSeconds (clip_animation_level.length);

        animation_rotation_level.enabled = false;

        transform_text_level.eulerAngles = Vector3.zero;

        transform_text_level.DOScale (1, Durations.DurationScale).SetEase (Ease.InBack).OnComplete (() => { GameActionManager.Instance.InstanceFxTapFlower (transform_text_level.transform.position); });

        text_level.text = level.ToString ();

        is_animation_level = false;

        transform_double_reward.DOScale (Vector3.one, Durations.DurationScale).SetEase (Ease.OutBack);
        transform_close_reward.DOScale (Vector3.one, Durations.DurationScale).SetEase (Ease.OutBack);

        yield break;
    }

    #endregion

    #region Callback

    private void OnWatchAdsCompleted ()
    {
        OnGetReward (2);
    }

    private void OnGetReward (int times)
    {
        var    diamond_value   = RewardLevelUpProperty.diamond_reward;
        double coin_value      = RewardLevelUpProperty.coins_reward;
        var    coin_unit_value = RewardLevelUpProperty.coins_unit_reward;

        diamond_value = diamond_value * times;

        coin_value = coin_value * times;

        Helper.FixUnit (ref coin_value, ref coin_unit_value);

        GameActionManager.Instance.InstanceFxDiamonds (Vector3.zero, UIGameManager.Instance.GetPositionHubDiamonds (), diamond_value);
        GameActionManager.Instance.InstanceFxCoins (Vector3.zero, UIGameManager.Instance.GetPositionHubCoins (), coin_value, coin_unit_value);

        GameActionManager.Instance.FxDisplayGold (Vector3.zero,
                                                  string.Format ("+{0}", ApplicationManager.Instance.AppendFromCashUnit (coin_value, coin_unit_value)));

        DisableHud ();
    }

    #endregion

    #region Interact

    public void InteractClose ()
    {
        if (is_animation_level)
            return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        OnGetReward (1);
    }

    public void InteractDoubleReward ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (Instance.OnWatchAdsCompleted, null);
        }
        else
        {
            this.RefreshRewardVideo ();

            ApplicationManager.Instance.AlertNoAdsAvailable ();
        }
    }

    #endregion
}