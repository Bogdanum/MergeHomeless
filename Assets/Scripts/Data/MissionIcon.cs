using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionIcon : ScriptableObject
{
    [System.Serializable]
    public struct MissionIconProperty
    {
        public MissionEnums.MissionId MissionId;
        public Sprite                 Icon;
    }

    [SerializeField] private MissionIconProperty[] _MissionIconProperties;

    public Sprite GetIcon (MissionEnums.MissionId missionId)
    {
        for (int i = 0; i < _MissionIconProperties.Length; i++)
        {
            if (_MissionIconProperties[i].MissionId == missionId)
                return _MissionIconProperties[i].Icon;
        }

        return null;
    }
}