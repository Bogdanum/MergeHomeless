using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class RandomReward : ScriptableObject
{
    [System.Serializable]
    public struct RandomRewardDetails
    {
        public                  int ItemLevel;
        [Range (0, 100)] public int Percent;
    }

    [System.Serializable]
    public struct RandomRewardProperty
    {
        public int                   Level;
        public RandomRewardDetails[] Details;
    }

    [SerializeField] private RandomRewardProperty[] _RewardProperties;

    public int GetRandomItemLevel (int level)
    {
        for (int i = 0; i < _RewardProperties.Length; i++)
        {
            if (_RewardProperties[i].Level == level)
            {
                var item = _RewardProperties[i];

                var total = 0;

                for (int j = 0; j < item.Details.Length; j++)
                {
                    total += item.Details[j].Percent;
                }

                var randomValue = Random.Range (0, total);

                total = 0;

                for (int j = 0; j < item.Details.Length; j++)
                {
                    total += item.Details[j].Percent;

                    if (randomValue < total)
                    {
                        return item.Details[j].ItemLevel;
                    }
                }
            }
        }

        if (level > _RewardProperties[_RewardProperties.Length - 1].Level)
        {
            var item = _RewardProperties[_RewardProperties.Length - 1];

            var total = 0;

            for (int j = 0; j < item.Details.Length; j++)
            {
                total += item.Details[j].Percent;
            }

            var randomValue = Random.Range (0, total);

            total = 0;

            for (int j = 0; j < item.Details.Length; j++)
            {
                total += item.Details[j].Percent;

                if (randomValue < total)
                {
                    return item.Details[j].ItemLevel;
                }
            }
        }

        return 1;
    }
}