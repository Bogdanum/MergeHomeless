using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionManager : Singleton<UIMissionManager>, IDialog
{
    [Header ("UI")] [SerializeField] private Transform _TransformHub;

    [SerializeField] private Transform _TransformViewDailyMission;

    [SerializeField] private Text _OutOfDailyQuest, _TextTimeDailyReset;
    [SerializeField] private Text label_daily_quest;

    #region Variables

    private List<MissionItems> _DailyMissionItems;

    private System.Action<object> HandleRefreshLayoutMission;
    private System.Action<object> HandleUpdateMissionValue;

    private int _QuantityDailyQuest;

    private bool IsEnableNoticeOutOfDailyQuest;

    #endregion

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitParameters ();
        RegisterEvent ();

        OnRefreshLayoutMission (0);
    }

    protected override void OnDestroy ()
    {
        UnRegisterEvent ();

        base.OnDestroy ();
    }

    #endregion


    #region Controller

    private void InitParameters ()
    {
        _DailyMissionItems = new List<MissionItems> ();

        IsEnableNoticeOutOfDailyQuest = _OutOfDailyQuest.gameObject.activeSelf;
    }

    private void RegisterEvent ()
    {
        HandleRefreshLayoutMission = param => OnRefreshLayoutMission ((int) param);
        HandleUpdateMissionValue   = param => OnUpdateMissionValue ((int) param);

        this.RegisterActionEvent (ActionEnums.ActionID.RefreshLayoutMission, HandleRefreshLayoutMission);
        this.RegisterActionEvent (ActionEnums.ActionID.UpdateMissionValue, HandleUpdateMissionValue);
    }


    private void UnRegisterEvent ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshLayoutMission, HandleRefreshLayoutMission);
        this.RemoveActionEvent (ActionEnums.ActionID.UpdateMissionValue, HandleUpdateMissionValue);
    }

    private void RegisterMissionEvent ()
    {
        MissionManager.Instance.OnUpdateTime += InstanceOnOnUpdateTime;

        InstanceOnOnUpdateTime (MissionManager.Instance.GetTimeDailyQuest ());
    }

    private void UnRegisterMissionEvent ()
    {
        MissionManager.Instance.OnUpdateTime -= InstanceOnOnUpdateTime;
    }

    private void InstanceOnOnUpdateTime (string obj)
    {
        _TextTimeDailyReset.text = obj;
    }

    #endregion

    #region Action

    private void RefreshLanguage ()
    {
        _OutOfDailyQuest.text = ApplicationLanguage.Text_daily_quest_will_back;

        if (_QuantityDailyQuest > 0)
        {
            var data = MissionManager.Instance.DailyQuest;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < _DailyMissionItems.Count; j++)
                {
                    if (_DailyMissionItems[j].GetId () == data[i].GetId ())
                    {
                        var item = MissionManager.Instance.GetMissionData (data[i].GetId ());

                        if (item == null)
                            continue;

                        _DailyMissionItems[j].SetDescription (MissionManager.Instance.GetDescription (data[i].GetId (), item.MissionProperty));
                    }
                }
            }
        }

        label_daily_quest.text = ApplicationLanguage.Text_label_daily_quest;
    }

    public void DisableHud ()
    {
        _TransformHub.gameObject.SetActive (false);

        GameManager.Instance.EnableTouch ();

        ApplicationManager.Instance.UnSetDialog (this);

        UnRegisterMissionEvent ();
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _TransformHub.gameObject.SetActive (true);

        GameManager.Instance.DisableTouch ();

        RefreshLanguage ();

        RegisterMissionEvent ();
    }

    public void InitDailyMission ()
    {
        _QuantityDailyQuest = 0;

        var data = MissionManager.Instance.DailyQuest;

        for (int i = _DailyMissionItems.Count; i < data.Length; i++)
        {
            var pref   = PoolExtension.GetPool (PoolEnums.PoolId.Mission_Layout, false);
            var script = pref.GetComponent<MissionItems> ();

            pref.SetParent (_TransformViewDailyMission);

            pref.localPosition = Vector.Vector3Zero;
            pref.localScale    = Vector.Vector3One;

            _DailyMissionItems.Add (script);
        }

        for (int i = 0; i < _DailyMissionItems.Count; i++)
        {
            _DailyMissionItems[i].Disable ();
        }

        for (int i = 0; i < data.Length; i++)
        {
            var item = MissionManager.Instance.GetMissionData (data[i].GetId ());

            if (item == null)
            {
                continue;
            }

            if (item.MissionProperty.Level < 0)
            {
                continue;
            }
            
            var script = _DailyMissionItems[i];

            script.SetDescription (MissionManager.Instance.GetDescription (data[i].GetId (), item.MissionProperty))
                  .SetQuantityTarget (item.MissionProperty.QuantityTarget)
                  .SetProcess (item.Quantity)
                  .SetIcon (MissionManager.Instance.MissionIcon.GetIcon (data[i].GetId ()))
                  .SetReward (item.MissionProperty)
                  .SetId (data[i].GetId ())
                  .RefreshStatus ()
                  .Enable ();

            _QuantityDailyQuest++;
        }
    }

    private void RefreshOutOfDailyQuest ()
    {
        if (_QuantityDailyQuest == 0)
        {
            if (IsEnableNoticeOutOfDailyQuest)
            {
                return;
            }

            _OutOfDailyQuest.gameObject.SetActive (true);
            _TextTimeDailyReset.gameObject.SetActive (true);
            IsEnableNoticeOutOfDailyQuest = true;

            MissionManager.Instance.RefreshTime ();
        }
        else
        {
            if (!IsEnableNoticeOutOfDailyQuest)
            {
                return;
            }

            _OutOfDailyQuest.gameObject.SetActive (false);
            _TextTimeDailyReset.gameObject.SetActive (false);
            IsEnableNoticeOutOfDailyQuest = false;
        }
    }

    #endregion

    #region Callback

    private void OnUpdateMissionValue (int obj)
    {
        for (int i = 0; i < _DailyMissionItems.Count; i++)
        {
            var item = _DailyMissionItems[i];

            if (item.GetIntId () != obj)
                continue;

            item.SetProcess (MissionManager.Instance.GetQuantity (item.GetId ()))
                .RefreshStatus ();
        }
    }

    private void OnRefreshLayoutMission (int obj)
    {
        InitDailyMission ();
        RefreshOutOfDailyQuest ();
    }

    #endregion

    #region Interaction

    public void Close ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        DisableHud ();
    }

    #endregion
}