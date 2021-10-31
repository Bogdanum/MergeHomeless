using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEventDispatcher : Singleton<MissionEventDispatcher> {

	private readonly Dictionary<string, System.Action<object>> _PoolEvent = new Dictionary<string, System.Action<object>> ();

    private event System.Action<object> paramOut;

    public MissionEventDispatcher RegisterEvent (MissionEnums.MissionId missionId, System.Action<object> action)
    {
        if (_PoolEvent.TryGetValue (MissionEnums.GetKey (missionId), out paramOut))
        {
            paramOut += action;

            _PoolEvent[MissionEnums.GetKey  (missionId)] = paramOut;
        }
        else
        {
            paramOut += action;

            _PoolEvent.Add (MissionEnums.GetKey  (missionId), paramOut);
        }

        LogGame.Log (string.Format ("[Mission Manager] Register the new event with ID: {0}",
                                    MissionEnums.GetKey  (missionId)));

        return this;
    }

    public MissionEventDispatcher RemoveEvent (MissionEnums.MissionId missionId, System.Action<object> action)
    {
        if (_PoolEvent.TryGetValue (MissionEnums.GetKey  (missionId), out paramOut))
        {
            paramOut -= action;

            _PoolEvent[MissionEnums.GetKey  (missionId)] = paramOut;

            LogGame.Log (string.Format ("[Mission Manager] Remove the event with ID: {0}",
                                        MissionEnums.GetKey  (missionId)));
        }
        else
        {
            LogGame.Log (string.Format ("[Mission Manager] Not Found the event with ID: {0}",
                                        MissionEnums.GetKey  (missionId)));
        }

        return this;
    }

    public MissionEventDispatcher PostEvent (MissionEnums.MissionId missionId, object param)
    {
        if (!_PoolEvent.TryGetValue (MissionEnums.GetKey (missionId), out paramOut)) return this;
        
        if (ReferenceEquals (paramOut, null))
        {
            _PoolEvent.Remove (MissionEnums.GetKey (missionId));
            return this;
        }

        paramOut (param);

        LogGame.Log (string.Format ("[Action Manager] Post the event with ID: {0}",
                                    MissionEnums.GetKey (missionId)));
        return this;
    }

    public void Clear ()
    {
        _PoolEvent.Clear ();
    }
}
