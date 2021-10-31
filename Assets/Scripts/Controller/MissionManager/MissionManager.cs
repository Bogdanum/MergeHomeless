using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public class MissionManager : Singleton<MissionManager>
{
    [System.Serializable]
    public class Mission
    {
        public MissionData.MissionProperty MissionProperty;
        public MissionEnums.MissionId      MissionId;
        public int                         Quantity;
        public bool                        IsMax;
        public bool                        IsRefresh;
    }

    [Header ("Daily Quest")] [SerializeField]
    public MissionData[] DailyQuest;

    [Header ("Mission")] [SerializeField] public MissionData[] MissionData;
    [Header ("Icon")] [SerializeField]    public MissionIcon   MissionIcon;

    private List<Mission> _Missions;

    private double _NextTimeForDailyQuest;

    private bool _IsUpdateTime;

    public event System.Action<string> OnUpdateTime;

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        Init ();

        RefreshSave ();
        RefreshInstance ();
        
        this.PostActionEvent (ActionEnums.ActionID.RefreshUICompleteMission);
    }

    protected override void OnDestroy ()
    {
        UnRegisterEvent ();

        base.OnDestroy ();
    }

    private void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus)
            return;

        RefreshTime (false);
    }

    #endregion

    #region Controller

    private void Init ()
    {
        RegisterEvent ();
    }

    private void RefreshSave ()
    {
        var time = (DateTime.Parse (Helper.GetUtcTimeStringNonHours ()) - DateTime.Parse (PlayerData._LastTimeDailyQuest)).TotalDays;

        if (time > 0)
        {
            PlayerData._LastTimeDailyQuest = Helper.GetUtcTimeStringNonHours ();
            PlayerData.SaveTimeDailyQuest ();

            for (int i = 0; i < DailyQuest.Length; i++)
            {
                PlayerData.SaveMissionLevel (DailyQuest[i].GetId (), 0);
                PlayerData.SaveMissionValue (DailyQuest[i].GetId (), 0);
            }
        }

        _Missions = new List<Mission> ();

        var size = PlayerData.MissionLevel.Count;

        for (int i = 0; i < size; i++)
        {
            var id = (MissionEnums.MissionId) i;

            if (IsMaxMission (id, PlayerData.GetMissionLevel (id)) || id == MissionEnums.MissionId.None)
            {
                continue;
            }

            _Missions.Add (new Mission ()
            {
                MissionId = id,
                Quantity  = PlayerData.GetMissionValue (id),
            });
        }

        for (int i = 0; i < DailyQuest.Length; i++)
        {
            var mission = DailyQuest[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                if (_Missions[j].MissionId == mission.GetId ())
                {
                    _Missions[j].MissionProperty = mission.GetMissionProperty (PlayerData.GetMissionLevel (_Missions[j].MissionId));
                    _Missions[j].IsMax           = _Missions[j].Quantity == _Missions[j].MissionProperty.QuantityTarget;
                }
            }
        }

        for (int i = 0; i < MissionData.Length; i++)
        {
            var mission = MissionData[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                if (_Missions[j].MissionId == mission.GetId ())
                {
                    _Missions[j].MissionProperty = mission.GetMissionProperty (PlayerData.GetMissionLevel (_Missions[j].MissionId));
                    _Missions[j].IsMax           = _Missions[j].Quantity == _Missions[j].MissionProperty.QuantityTarget;
                }
            }
        }
    }

    private void RefreshInstance ()
    {
        for (int i = 0; i < DailyQuest.Length; i++)
        {
            var IsHaveMission = false;
            var quest         = DailyQuest[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                var mission = _Missions[j];

                if (mission.MissionId == quest.GetId () || quest.IsMaxLevel (PlayerData.GetMissionLevel (quest.GetId ())))
                {
                    IsHaveMission = true;
                }
            }

            if (!IsHaveMission)
            {
                _Missions.Add (new Mission ()
                {
                    MissionId       = quest.GetId (),
                    Quantity        = 0,
                    MissionProperty = quest.GetMissionProperty (PlayerData.GetMissionLevel (quest.GetId ())),
                    IsMax           = false,
                    IsRefresh       = false,
                });
            }
        }

        for (int i = 0; i < MissionData.Length; i++)
        {
            var IsHaveMission = false;
            var quest         = MissionData[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                var mission = _Missions[j];

                if (mission.MissionId == quest.GetId () || quest.IsMaxLevel (PlayerData.GetMissionLevel (quest.GetId ())))
                {
                    IsHaveMission = true;
                }
            }

            if (!IsHaveMission)
            {
                _Missions.Add (new Mission ()
                {
                    MissionId       = quest.GetId (),
                    Quantity        = 0,
                    MissionProperty = quest.GetMissionProperty (PlayerData.GetMissionLevel (quest.GetId ())),
                    IsMax           = false,
                    IsRefresh       = false,
                });
            }
        }
    }

    private void ClearData (MissionEnums.MissionId id)
    {
        for (int j = 0; j < _Missions.Count (); j++)
        {
            var mission = _Missions[j];

            if (mission.IsMax && mission.MissionId == id)
            {
                var level = PlayerData.GetMissionLevel (mission.MissionId) + 1;

                PlayerData.SaveMissionLevel (mission.MissionId, level);

                if (IsMaxMission (mission.MissionId, level))
                {
                    _Missions.RemoveAt (j);
                    j--;

                    continue;
                }

                mission.IsRefresh = true;
            }
        }
    }

    private void RefreshData ()
    {
        for (int i = 0; i < DailyQuest.Length; i++)
        {
            var quest = DailyQuest[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                var mission = _Missions[j];

                if (mission.MissionId == quest.GetId () && mission.IsRefresh)
                {
                    mission.MissionId       = quest.GetId ();
                    mission.Quantity        = 0;
                    mission.MissionProperty = quest.GetMissionProperty (PlayerData.GetMissionLevel (quest.GetId ()));
                    mission.IsMax           = false;
                    mission.IsRefresh       = false;

                    PlayerData.SaveMissionValue (mission.MissionId, mission.Quantity);
                    break;
                }
            }
        }

        for (int i = 0; i < MissionData.Length; i++)
        {
            var quest = MissionData[i];

            for (int j = 0; j < _Missions.Count; j++)
            {
                var mission = _Missions[j];

                if (mission.MissionId == quest.GetId () && mission.IsRefresh)
                {
                    mission.MissionId       = quest.GetId ();
                    mission.Quantity        = 0;
                    mission.MissionProperty = quest.GetMissionProperty (PlayerData.GetMissionLevel (quest.GetId ()));
                    mission.IsMax           = false;
                    mission.IsRefresh       = false;

                    PlayerData.SaveMissionValue (mission.MissionId, mission.Quantity);
                    break;
                }
            }
        }
    }

    #endregion

    #region Action

    public void RefreshTime (bool is_update = true)
    {
        _NextTimeForDailyQuest = GameConfig.MaxTimeForNextDailyQuest - (Helper.GetUtcTime () - DateTime.Parse (PlayerData._LastTimeDailyQuest)).TotalSeconds;

        if (_NextTimeForDailyQuest < 0)
            _NextTimeForDailyQuest = 0.0;

        if (_IsUpdateTime || !is_update)
            return;

        _IsUpdateTime = true;

        Timing.RunCoroutine (_EnumeratorTimeRefreshDailyQuest ());
    }

    private void RegisterEvent ()
    {
        this.RegisterMissionEvent (MissionEnums.MissionId.Merge, OnUpdateMergeItems);
        this.RegisterMissionEvent (MissionEnums.MissionId.GetBonus, OnUpdateGetBonus);
        this.RegisterMissionEvent (MissionEnums.MissionId.Tutorial, OnCompletedTutorial);
        this.RegisterMissionEvent (MissionEnums.MissionId.UpgradeItem, OnUpgradeItems);
        this.RegisterMissionEvent (MissionEnums.MissionId.BuyItem, OnBuyItem);
        this.RegisterMissionEvent (MissionEnums.MissionId.TapOnItem, OnTapItem);
        this.RegisterMissionEvent (MissionEnums.MissionId.TapOnBox, OnTapBox);
    }

    private void UnRegisterEvent ()
    {
        this.RemoveMissionEvent (MissionEnums.MissionId.Merge, OnUpdateMergeItems);
        this.RemoveMissionEvent (MissionEnums.MissionId.GetBonus, OnUpdateGetBonus);
        this.RemoveMissionEvent (MissionEnums.MissionId.Tutorial, OnCompletedTutorial);
        this.RemoveMissionEvent (MissionEnums.MissionId.UpgradeItem, OnUpgradeItems);
        this.RemoveMissionEvent (MissionEnums.MissionId.BuyItem, OnBuyItem);
        this.RemoveMissionEvent (MissionEnums.MissionId.TapOnItem, OnTapItem);
        this.RemoveMissionEvent (MissionEnums.MissionId.TapOnBox, OnTapBox);
    }

    private void SetQuantity (MissionEnums.MissionId id, int quantity)
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            var item = _Missions[i];

            if (item.MissionId == id)
            {
                if (item.IsMax)
                    return;

                item.Quantity += quantity;

                if (item.Quantity >= item.MissionProperty.QuantityTarget)
                {
                    item.IsMax    = true;
                    item.Quantity = item.MissionProperty.QuantityTarget;
                }

                PlayerData.SaveMissionValue (item.MissionId, item.Quantity);

                break;
            }
        }
    }

    #endregion

    #region Callback

    private void OnUpgradeItems (object obj)
    {
        SetQuantity (MissionEnums.MissionId.UpgradeItem, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.UpgradeItem);
    }

    private void OnTapBox (object obj)
    {
        SetQuantity (MissionEnums.MissionId.TapOnBox, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.TapOnBox);
    }

    private void OnTapItem (object obj)
    {
        SetQuantity (MissionEnums.MissionId.TapOnItem, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.TapOnItem);
    }

    private void OnBuyItem (object obj)
    {
        SetQuantity (MissionEnums.MissionId.BuyItem, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.BuyItem);
    }

    private void OnCompletedTutorial (object obj) { }

    private void OnUpdateMergeItems (object param)
    {
        SetQuantity (MissionEnums.MissionId.Merge, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.Merge);
    }

    private void OnUpdateGetBonus (object param)
    {
        SetQuantity (MissionEnums.MissionId.GetBonus, 1);

        this.PostActionEvent (ActionEnums.ActionID.UpdateMissionValue, (int) MissionEnums.MissionId.GetBonus);
    }

    public void OnGetReward (MissionEnums.MissionId id)
    {
        var level = 0;

        for (int i = 0; i < _Missions.Count; i++)
        {
            var item = _Missions[i];

            if (item.MissionId == id)
            {
                level = item.MissionProperty.Level;

                var rewards = item.MissionProperty._MissionReward;

                for (int j = 0; j < rewards.Length; j++)
                {
                    var reward = rewards[j];

                    switch (reward.RewardId)
                    {
                        case RewardEnums.RewardId.X5RewardCoins:
                            OnGetX5RewardCoins (reward.Value);
                            break;
                        case RewardEnums.RewardId.Diamonds:
                            OnGetDiamonds (reward.Value);
                            break;
                        case RewardEnums.RewardId.SpeedUp:
                            OnGetSpeedUp (reward.Value);
                            break;
                        case RewardEnums.RewardId.Box:
                            OnGetBox (reward.Value);
                            break;
                        case RewardEnums.RewardId.Cash:
                            OnGetCash (reward.Value, reward.Unit);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException ();
                    }
                }
            }
        }

        OnGetRewardCompleted (id, level);
    }

    private void OnGetRewardCompleted (MissionEnums.MissionId id, int level)
    {
        ClearData (id);
        RefreshData ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshLayoutMission, (int) id);
    }


    private void OnGetCash (int quantity, int unit)
    {
        GameManager.Instance.FxEarnCoin (quantity, unit, Vector.Vector3Zero);
    }

    private void OnGetX5RewardCoins (int quantity)
    {
        if (GameActionManager.Instance != null)
            GameActionManager.Instance.SetMultiRewardCoins (quantity);
        
        if (MoreCashManager.Instance != null)
            MoreCashManager.Instance.RefreshMultiRewardCoins ();
    }

    private void OnGetDiamonds (int quantity)
    {
        if (GameActionManager.Instance != null)
            GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                           UIGameManager.Instance.GetPositionHubDiamonds (),
                                                           quantity);
        else
        {
            PlayerData.Diamonds += quantity;
            PlayerData.SaveDiamonds ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
        }
    }

    private void OnGetSpeedUp (int quantity)
    {
        if (SpeedUpManager.Instance != null) SpeedUpManager.Instance.AddSpeedUP (quantity);
    }

    private void OnGetBox (int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (GameManager.Instance != null) GameManager.Instance.SetRandomReward ();
        }
    }

    #endregion

    #region Enumerator

    private IEnumerator<float> _EnumeratorTimeRefreshDailyQuest ()
    {
        while (_NextTimeForDailyQuest > 0)
        {
            _NextTimeForDailyQuest -= 1;

            if (!ReferenceEquals (OnUpdateTime, null))
            {
                OnUpdateTime (Helper.ConvertToTime (_NextTimeForDailyQuest));
            }

            yield return Timing.WaitForSeconds (1f);
        }

        RefreshSave ();
        RefreshInstance ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshLayoutMission);

        _IsUpdateTime = false;

        yield break;
    }

    #endregion

    #region Helper

    public string GetDescription (MissionEnums.MissionId id, MissionData.MissionProperty data)
    {
        switch (id)
        {
            case MissionEnums.MissionId.None:
                return string.Empty;
            case MissionEnums.MissionId.Merge:
                return string.Format (ApplicationLanguage.Text_description_mission_merge, data.QuantityTarget.ToString ());
            case MissionEnums.MissionId.GetBonus:
                return string.Format (ApplicationLanguage.Text_description_mission_get_bonus, data.QuantityTarget.ToString ());
            case MissionEnums.MissionId.UpgradeItem:
                return string.Format (ApplicationLanguage.Text_description_mission_upgrade_item, data.QuantityTarget.ToString ());
            case MissionEnums.MissionId.BuyItem:
                return string.Format (ApplicationLanguage.Text_description_mission_buy_item, data.QuantityTarget.ToString ());
            case MissionEnums.MissionId.TapOnItem:
                return string.Format (ApplicationLanguage.Text_description_mission_tap_item, data.QuantityTarget.ToString ());
            case MissionEnums.MissionId.TapOnBox:
                return string.Format (ApplicationLanguage.Text_description_mission_tap_box, data.QuantityTarget.ToString ());
            default:
                return string.Empty;
        }
    }

    public int GetQuantity (MissionEnums.MissionId id)
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            if (_Missions[i].MissionId == id)
                return _Missions[i].Quantity;
        }

        return 0;
    }

    public int GetQuantityTarget (MissionEnums.MissionId id)
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            if (_Missions[i].MissionId == id)
                return _Missions[i].MissionProperty.QuantityTarget;
        }

        return 0;
    }

    public Mission GetMissionData (MissionEnums.MissionId id)
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            if (_Missions[i].MissionId == id)
                return _Missions[i];
        }

        return null;
    }

    public bool IsSameLevelTarget (MissionEnums.MissionId id, int level)
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            var item = _Missions[i];

            if (item.MissionId == id && item.MissionProperty.LevelTarget == level)
                return true;
        }

        return false;
    }

    public bool IsHaveMissionCompleted ()
    {
        for (int i = 0; i < _Missions.Count; i++)
        {
            var item = _Missions[i];

            if (item.MissionProperty.Level > -1 && item.Quantity == item.MissionProperty.QuantityTarget && item.MissionId != MissionEnums.MissionId.None)
                return true;
        }

        return false;
    }

    public bool IsMaxMission (MissionEnums.MissionId id, int level)
    {
        for (int i = 0; i < DailyQuest.Length; i++)
        {
            if (DailyQuest[i].GetId () == id && !DailyQuest[i].IsMaxLevel (level))
                return false;
        }

        for (int i = 0; i < MissionData.Length; i++)
        {
            if (MissionData[i].GetId () == id && !MissionData[i].IsMaxLevel (level))
                return false;
        }

        return true;
    }

    public string GetTimeDailyQuest ()
    {
        return Helper.ConvertToTime (_NextTimeForDailyQuest);
    }

    #endregion
}