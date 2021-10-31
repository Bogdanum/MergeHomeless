using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardIcon : ScriptableObject
{
    [System.Serializable]
    public struct RewardIconProperty
    {
        public RewardEnums.RewardId Id;
        public Sprite               Icon;
    }

    [SerializeField] private RewardIconProperty[] _RewardIconProperties;

    public Sprite GetIcon (RewardEnums.RewardId id)
    {
        for (int i = 0; i < _RewardIconProperties.Length; i++)
        {
            if (_RewardIconProperties[i].Id == id)
                return _RewardIconProperties[i].Icon;
        }

        return null;
    }
}