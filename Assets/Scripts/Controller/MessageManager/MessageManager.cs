using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    [Header ("UI")] [SerializeField] private UIMessageManager _UiMessageManager;

    private float _TimeDisable;
    private float _TimeDisableNotice;

    private bool _IsEnableMessage;
    private bool _IsEnableNotice;
    private bool _IsCheckEnableRate;

    #region Variables

    private System.Action _OnCompletedForceMessage;
    private System.Action OnCloseForceMessage;

    #endregion

    protected override void Awake ()
    {
        base.Awake ();

        _UiMessageManager.Init ();
    }

    private IEnumerator<float> DisableMessage ()
    {
        while (_TimeDisable > 0)
        {
            _TimeDisable -= 1;

            yield return Timing.WaitForSeconds (1f);
        }

        _IsEnableMessage = false;

        DisableMessageHud ();
    }

    private IEnumerator<float> DisableNotice ()
    {
        while (_TimeDisableNotice > 0)
        {
            _TimeDisableNotice -= 1;

            yield return Timing.WaitForSeconds (1f);
        }

        _IsEnableNotice = false;

        DisableNoticeHud ();
    }

    private IEnumerator<float> CheckShowRate ()
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        _UiMessageManager.EnableRateHud ();

        _IsCheckEnableRate = false;
    }

    #region Action

    public void ShowMessage (string text)
    {
        _UiMessageManager.SetMessage (text);

        _TimeDisable = 1f;

        if (_IsEnableMessage == false)
        {
            _UiMessageManager.EnableMessageHud ();

            Timing.RunCoroutine (DisableMessage ());

            _IsEnableMessage = true;
        }
    }


    public void ShowNotice (string text, Sprite icon)
    {
        _UiMessageManager.SetNotice (text, icon);

        _TimeDisableNotice = 2f;

        if (_IsEnableNotice == false)
        {
            _UiMessageManager.EnableNoticeHud ();

            Timing.RunCoroutine (DisableNotice ());

            _IsEnableNotice = true;

            this.PlayAudioSound (AudioEnums.SoundId.NoticeMissionCompleted);
        }
    }

    public void ShowForceMessage (string text, bool can_close, System.Action onCompleted, System.Action onClose)
    {
        _UiMessageManager.SetForceMessage (text);
        _UiMessageManager.SetCloseState (can_close);

        _OnCompletedForceMessage = onCompleted;
        OnCloseForceMessage      = onClose;

        _UiMessageManager.EnableForceHud ();
    }

    public void DisableMessageHud ()
    {
        _UiMessageManager.CloseMessage ();
    }

    public void DisableNoticeHud ()
    {
        _UiMessageManager.CloseNotice ();
    }

    public void DisableForceHud ()
    {
        _UiMessageManager.DisableForceHud ();
    }

    public void DoCompletedForceMessage ()
    {
        if (_OnCompletedForceMessage == null) return;

        _OnCompletedForceMessage ();
        _OnCompletedForceMessage = null;
    }

    public void DoCloseForceMessage ()
    {
        if (OnCloseForceMessage == null) return;

        OnCloseForceMessage ();
        OnCloseForceMessage = null;
    }

    public void ShowRate (bool is_force_open = false)
    {
        if (PlayerData.IsOpenRate && !is_force_open || _IsCheckEnableRate)
        {
            return;
        }

        _IsCheckEnableRate = true;

        Timing.RunCoroutine (CheckShowRate ());

        _UiMessageManager.SetRate (ApplicationLanguage.Text_description_rate_me);
    }

    #endregion
}

public static class MessageManagerExtension
{
    public static void ShowMessage (this MonoBehaviour mono, string message)
    {
        if (MessageManager.InstanceAwake () == null)
            return;

        MessageManager.Instance.ShowMessage (message);
    }
}