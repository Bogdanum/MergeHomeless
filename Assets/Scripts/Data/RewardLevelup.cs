using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardLevelup : ScriptableObject
{
    [System.Serializable]
    public struct RewardLevelUpProperty
    {
        public int   Level;
        public int   diamond_reward;
        public float coins_reward;
        public int   coins_unit_reward;
    }

    [SerializeField] private RewardLevelUpProperty[] RewardLevelUpProperties;

    #region Helper

    public RewardLevelUpProperty GetReward (int level)
    {
        // =============================== Get the reward when level up ================================ //
        
        for (int i = 0; i < RewardLevelUpProperties.Length; i++)
        {
            if (level == RewardLevelUpProperties[i].Level)
                return RewardLevelUpProperties[i];
        }

        return RewardLevelUpProperties[RewardLevelUpProperties.Length - 1];
    }

    #endregion
}