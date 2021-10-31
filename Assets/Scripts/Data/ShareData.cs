using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareData : ScriptableObject
{
    [System.Serializable]
    public struct ShareDataProperty
    {
        public int    ItemLevel;
        public double ValueReward;
        public int    UnitReward;
    }

    [SerializeField] private ShareDataProperty[] _ShareDataProperties;

    public double GetRewardValue (int level)
    {
        for (int i = 0; i < _ShareDataProperties.Length; i++)
        {
            if (_ShareDataProperties[i].ItemLevel == level)
                return _ShareDataProperties[i].ValueReward;
        }

        return 0.0;
    }

    public int GetRewardUnit (int level)
    {
        for (int i = 0; i < _ShareDataProperties.Length; i++)
        {
            if (_ShareDataProperties[i].ItemLevel == level)
                return _ShareDataProperties[i].UnitReward;
        }

        return 0;
    }
}