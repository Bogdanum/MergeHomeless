using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostData : ScriptableObject
{
    public enum TimeJumpId
    {
        OneDay,
        SevenDay,
        FourteenDay,
        ThirtyDay,
    }

    [System.Serializable]
    public struct BoostProperty
    {
        public TimeJumpId time_jump_id;
        public Sprite     icon;
        public int        price_diamonds;
    }

    [SerializeField] private BoostProperty[] boost_properties;

    #region Helper

    public int GetSize ()
    {
        return boost_properties.Length;
    }

    public BoostProperty GetBoost (int index)
    {
        return boost_properties[index];
    }

    #endregion
}