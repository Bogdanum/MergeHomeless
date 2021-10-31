using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class OfflineData : ScriptableObject
{
    [System.Serializable]
    public struct OfflineProperty
    {
        // =============================== THE PERCENT OF PROFIT CAN EARN WILL RETURN BASE ON TIME OFFLINE FROM PLAYER ================================ //
        // =============================== PLAYER OFFLINE FROM MIN_TIME TO MAX_TIME WILL GET DIFFERENCE PERCENT OF PROFIT ================================ //

        [Tooltip ("Min Time Offline can get revenue, Unit = Hour.")]
        public int Min_Time;

        [Tooltip ("Max Time Offline can get revenue, Unit = Hour.")]
        public int Max_time;

        [Tooltip ("Percent profit can earn from real profit.")] [Range (0, 100)]
        public int PercentProfit;
    }

    [SerializeField] private OfflineProperty[] _OfflineProperties;

    public float GetPercentProfit (int hours)
    {
        var index_revenue = 0;

        for (int i = 0; i < _OfflineProperties.Length; i++)
        {
            if (hours >= _OfflineProperties[i].Min_Time)
            {
                index_revenue = i;
            }

            if (hours <= _OfflineProperties[i].Max_time)
            {
                break;
            }
        }

        return _OfflineProperties[index_revenue].PercentProfit / 100f;
    }

    public double GetMaxSeconds (double seconds)
    {
        int max_hours = 0;

        for (int i = 0; i < _OfflineProperties.Length; i++)
        {
            if (max_hours < _OfflineProperties[i].Max_time)
            {
                max_hours = _OfflineProperties[i].Max_time;
            }
        }

        if (seconds > max_hours * 3600f)
            return max_hours * 3600f;

        return seconds;
    }
}