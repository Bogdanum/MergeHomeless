using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoreCashHub : MonoBehaviour
{
    [Header ("Hud Menu")] [SerializeField] private Transform _TransformTimeProcess;
    [SerializeField]                       private Text      _TextProcessTimeOnHud;

    private System.Action<bool>   HandleStateMoreCashHub;
    private System.Action<string> HandleStateMoreCashTime;

    #region System

    private void OnEnable ()
    {
        RegisterSpeedHub ();

        OnStateMoreCashHub (MoreCashManager.InstanceAwake ().IsEnableMoreCash ());
    }

    private void OnDisable ()
    {
        UnRegisterSpeedHub ();
    }

    #endregion

    #region Controller

    private void RegisterSpeedHub ()
    {
        HandleStateMoreCashHub  = OnStateMoreCashHub;
        HandleStateMoreCashTime = OnUpdateTimeMoreCashHub;

        if (MoreCashManager.InstanceAwake () != null)
        {
            MoreCashManager.Instance.RegisterMoreCashHub (HandleStateMoreCashHub);
            MoreCashManager.Instance.RegisterMoreCashTime (HandleStateMoreCashTime);
        }
    }

    private void UnRegisterSpeedHub ()
    {
        if (MoreCashManager.Instance == null) return;
        MoreCashManager.Instance.UnRegisterMoreCashHub (HandleStateMoreCashHub);
        MoreCashManager.Instance.UnRegisterMoreCashTime (HandleStateMoreCashTime);
    }

    private void OnStateMoreCashHub (bool obj)
    {
        if (_TransformTimeProcess != null) _TransformTimeProcess.gameObject.SetActive (obj);
    }

    private void OnUpdateTimeMoreCashHub (string time)
    {
        if (_TextProcessTimeOnHud != null) _TextProcessTimeOnHud.text = time;
    }

    #endregion
}