using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class DiamondManager : Singleton<DiamondManager>, IDialog
{
    [SerializeField] private UIDiamondManager _UiDiamondManager;

    #region Variables

    private int  _TimeForWaiting;
    private bool IsUpdate;
    private bool _IsReadyUpdate;
    private bool IsEnable;

    #endregion

    protected override void Awake ()
    {
        base.Awake ();

        InitConfig ();
        InitUpdateTime ();
    }

    protected override void OnDestroy ()
    {
        IsUpdate = false;

        base.OnDestroy ();
    }

    #region Controller

    private void InitConfig ()
    {
        _UiDiamondManager.Init ();
    }

    private void InitUpdateTime ()
    {
        IsUpdate = true;

        Timing.RunCoroutine (IUpdate ());
    }

    #endregion

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _UiDiamondManager.EnableHud ();

        GameManager.Instance.DisableTouch ();
       
        RefreshTime ();

        IsEnable = true;
    }

    public void DisableHud ()
    {
        ApplicationManager.Instance.UnSetDialog (this);

        _UiDiamondManager.DisableHud ();

        GameManager.Instance.EnableTouch ();
      
        IsEnable = false;
    }

    #region Action

    public void RefreshTime ()
    {
        var total_time = (DateTime.Parse (PlayerData._LastTimeWatchAdsForFreeDiamonds) - Helper.GetUtcTime ()).TotalSeconds + GameConfig.MaxTimeWaitingAdsForFreeDiamonds;

        if (total_time < 0)
        {
            total_time = 0;
        }

        _TimeForWaiting = (int) total_time;

        _IsReadyUpdate = _TimeForWaiting > 0;

        _UiDiamondManager.UpdateTime (Helper.ConvertToTime (_TimeForWaiting));
        _UiDiamondManager.UpdateState (!_IsReadyUpdate);
    }

    #endregion

    #region IEnumerator

    private IEnumerator<float> IUpdate ()
    {
        while (IsUpdate)
        {
            if (_IsReadyUpdate && IsEnable)
            {
                if (_TimeForWaiting > 0)
                {
                    _TimeForWaiting--;

                    _UiDiamondManager.UpdateTime (Helper.ConvertToTime (_TimeForWaiting));
                }
                else
                {
                    _IsReadyUpdate  = false;
                    _TimeForWaiting = 0;
                    RefreshTime ();
                }
            }

            yield return Timing.WaitForSeconds (1f);
        }
    }

    #endregion

    private void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus == false)
            RefreshTime ();
    }
}