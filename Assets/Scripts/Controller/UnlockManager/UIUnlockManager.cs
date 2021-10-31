using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class UIUnlockManager : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformUnlockHud;

    [SerializeField] private Image _ProcessBarEarning, _ProcessBarSpeed;
    [SerializeField] private Text  _NameItem;
    [SerializeField] private Text  label_level;
    [SerializeField] private Text  label_earning_coin_value;

    [Header ("Share")] [SerializeField] private Text label_value_share;

    [Header ("Language")] [SerializeField] private Text _LabelUnlocked;

    [SerializeField] private Text _LabelSpeed, _LabelEarning;
    [SerializeField] private Text label_share;

    [Header ("Character")] [SerializeField]
    private Image image_icon;

    private bool is_running_unlocked;

    #region Action

    public void Init (Sprite icon, float speed, float earning, string name_item, ItemNodeData data)
    {
        if (is_running_unlocked)
        {
            return;
        }

        is_running_unlocked = true;

        image_icon.sprite = icon;

        _ProcessBarEarning.fillAmount = earning;
        _ProcessBarSpeed.fillAmount   = speed;
        _NameItem.text                = name_item;

        label_level.text = string.Format (ApplicationLanguage.Text_description_lv, "0");
        label_earning_coin_value.text = string.Format (ApplicationLanguage.Text_description_earning_each_sec,
                                                       ApplicationManager.Instance.AppendFromCashUnit (data.ProfitPerSec, data.ProfitPerSecUnit));

        Timing.RunCoroutine (enumerator_animation_unlock ());
    }

    public void EnableHud ()
    {
        if (_TransformUnlockHud != null) _TransformUnlockHud.gameObject.SetActive (true);

        RefreshLanguage ();
    }

    public void Disable ()
    {
        if (_TransformUnlockHud != null) _TransformUnlockHud.gameObject.SetActive (false);
    }

    public void RefreshLanguage ()
    {
        _LabelEarning.text  = ApplicationLanguage.Text_label_earning;
        _LabelSpeed.text    = ApplicationLanguage.Text_label_speed;
        _LabelUnlocked.text = ApplicationLanguage.Text_label_unlocked;
        label_share.text    = string.Format (ApplicationLanguage.Text_description_revenue_from_ads, "2");
    }

    public void SetShareFacebook (string value)
    {
        label_value_share.text = value;
    }

    #endregion

    #region Enumeator

    private IEnumerator<float> enumerator_animation_unlock ()
    {
        image_icon.transform.localScale = Vector3.zero;

        yield return Timing.WaitForSeconds (0.5f);

        this.PlayAudioSound (AudioEnums.SoundId.NewItems);

        image_icon.transform.DOScale (1.2f, Durations.DurationScale).SetEase (Ease.OutBack);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        GameActionManager.Instance.InstanceFxTapFlower (image_icon.transform.position);
        GameActionManager.Instance.InstanceFxTapBox (image_icon.transform.position);

        is_running_unlocked = false;

        yield break;
    }

    #endregion

    #region Interface Interact

    public void WatchAdsDoubleReward ()
    {
        if (is_running_unlocked) return;

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        UnlockManager.Instance.DoubleReward ();
    }

    public void Close ()
    {
        if (is_running_unlocked) return;

        UnlockManager.Instance.DisableHud ();
        UnlockManager.Instance.OnGetReward (1);

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
    }

    #endregion
}