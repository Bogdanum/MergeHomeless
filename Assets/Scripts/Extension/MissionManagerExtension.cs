using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MissionManagerExtension
{
    public static MissionEventDispatcher RegisterMissionEvent (this MonoBehaviour mono, MissionEnums.MissionId missionId, System.Action<object> callback)
    {
        if (ReferenceEquals (MissionEventDispatcher.InstanceAwake (), null))
        {
            LogGame.Error ("[Mission Event] Mission Event Dispatcher Is Null!");
            return null;
        }

        return MissionEventDispatcher.Instance.RegisterEvent (missionId, callback);
        ;
    }

    public static MissionEventDispatcher RemoveMissionEvent (this MonoBehaviour mono, MissionEnums.MissionId missionId, System.Action<object> callback)
    {
        if (ReferenceEquals (MissionEventDispatcher.InstanceAwake (), null))
        {
            LogGame.Error ("[Mission Event] Mission Event Dispatcher Is Null!");
            return null;
        }

        return MissionEventDispatcher.Instance.RemoveEvent (missionId, callback);
        ;
    }

    public static MissionEventDispatcher PostMissionEvent (this MonoBehaviour mono, MissionEnums.MissionId missionId, object param = null)
    {
        if (PlayerData.Level < GameConfig.UnlockMissionLevel)
            return null;
        
        if (ReferenceEquals (MissionEventDispatcher.InstanceAwake (), null))
        {
            LogGame.Error ("[Mission Event] Mission Event Dispatcher Is Null!");
            return null;
        }

        return MissionEventDispatcher.Instance.PostEvent (missionId, param);
        ;
    }
}