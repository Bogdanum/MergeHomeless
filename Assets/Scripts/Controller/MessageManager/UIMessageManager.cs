using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageManager : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformMessageHud;
    [SerializeField]                 private Transform _TransformNoticeHud;
    [SerializeField]                 private Transform _TransformForceGroup;
    [SerializeField]                 private Transform transform_rate_hud;

    [Header ("Text")] [SerializeField] private Text _MessageText;

    [Header ("Notice")] [SerializeField] private Text  _NoticeText;
    [SerializeField]                     private Image _NoticeIcon;

    [Header ("Update")] [SerializeField] private Text      _ForceText;
    [SerializeField]                     private Transform transform_close_update;

    [Header ("Rating")] [SerializeField] private Text _Description_Rate;
    [SerializeField]                     private Text label_rate_now;

    #region Action

    public void Init () { }

    public void EnableMessageHud ()
    {
        if (_TransformMessageHud == null) return;

        _TransformMessageHud.gameObject.SetActive (true);
        _TransformMessageHud.localScale = Vector3.zero;
        _TransformMessageHud.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);
    }

    public void EnableNoticeHud ()
    {
        if (_TransformNoticeHud == null) return;

        _TransformNoticeHud.gameObject.SetActive (true);
        _TransformNoticeHud.localScale = Vector3.zero;
        _TransformNoticeHud.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);
    }

    public void EnableRateHud ()
    {
        if (transform_rate_hud == null) return;

        transform_rate_hud.gameObject.SetActive (true);
        transform_rate_hud.localScale = Vector3.zero;
        transform_rate_hud.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);
    }

    public void EnableForceHud ()
    {
        if (_TransformForceGroup == null)
            return;

        _TransformForceGroup.gameObject.SetActive (true);
    }

    public void SetMessage (string text)
    {
        if (_MessageText != null) _MessageText.text = text;
    }

    public void SetCloseState (bool status)
    {
        transform_close_update.gameObject.SetActive (status);
    }

    public void SetNotice (string text, Sprite icon)
    {
        _NoticeText.text   = text;
        _NoticeIcon.sprite = icon;
    }

    public void SetForceMessage (string text)
    {
        _ForceText.text = text;
    }

    public void SetRate (string message)
    {
        _Description_Rate.text = message;
        label_rate_now.text    = ApplicationLanguage.Text_label_rate_now;
    }

    #endregion

    #region Interface Interact

    public void CloseMessage ()
    {
        _TransformMessageHud.gameObject.SetActive (false);
    }

    public void CloseNotice ()
    {
        _TransformNoticeHud.gameObject.SetActive (false);
    }

    public void DisableForceHud ()
    {
        _TransformForceGroup.gameObject.SetActive (false);
    }

    public void CloseForceMessage ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        _TransformForceGroup.gameObject.SetActive (false);

        MessageManager.Instance.DoCloseForceMessage ();
    }

    public void AcceptMessage ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        _TransformForceGroup.gameObject.SetActive (false);

        MessageManager.Instance.DoCompletedForceMessage ();
    }

    public void AcceptRate ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        transform_rate_hud.gameObject.SetActive (false);

        ApplicationManager.Instance.OpenUrlStore ();

        PlayerData.IsOpenRate = true;
    }

    public void CloseRate ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        transform_rate_hud.gameObject.SetActive (false);
    }

    #endregion
}