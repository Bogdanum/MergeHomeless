using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData : ScriptableObject
{
    [System.Serializable]
    public struct MissionReward
    {
        public RewardEnums.RewardId RewardId;
        public int                  Value;
        public int                  Unit;
    }

    [System.Serializable]
    public struct MissionProperty
    {
        public int Level;

        [Tooltip ("Level Unlock the mission with player")]
        public int PlayerLevelUnlock;

        public int QuantityTarget;
        public int LevelTarget;

        public MissionReward[] _MissionReward;
    }

    [SerializeField] private MissionEnums.MissionId _MissionId;
    [SerializeField] private MissionProperty[]      _MissionProperties;

    public MissionEnums.MissionId GetId ()
    {
        return _MissionId;
    }

    public MissionProperty GetMissionProperty (int level)
    {
        for (int i = 0; i < _MissionProperties.Length; i++)
        {
            if (_MissionProperties[i].Level == level)
                return _MissionProperties[i];
        }

        return new MissionProperty () {Level = -1};
    }

    public bool IsMaxLevel (int level)
    {
        return level > _MissionProperties[_MissionProperties.Length - 1].Level;
    }
}