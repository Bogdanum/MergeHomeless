using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : Singleton<SettingManager>, IDialog
{
    [Header ("UI")] [SerializeField] private UISettingManager _UiSettingManager;

    #region System

    protected override void Awake ()
    {
        base.Awake ();
    }

    #endregion

    #region Action

    public void DisableHud ()
    {
        _UiSettingManager.DisableHud ();

        GameManager.Instance.EnableTouch ();

        ApplicationManager.Instance.UnSetDialog (this);
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _UiSettingManager.EnableHud ();

        GameManager.Instance.DisableTouch ();
    }

    #endregion
}