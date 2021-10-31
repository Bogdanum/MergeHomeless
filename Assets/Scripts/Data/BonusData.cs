using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusData : ScriptableObject
{
    [System.Serializable]
    public struct BonusProperty
    {
        public BonusEnums.BonusId BonusId;
        public int                quantity;
        public Sprite             Icon;

        [Range (0, 100)] public int Percents;
    }

    [SerializeField] private BonusProperty[] _BonusProperties;

    public BonusEnums.BonusId GetRandomBonus ()
    {
        var total   = 0;
        var current = 0;

        for (int i = 0; i < _BonusProperties.Length; i++)
        {
            total += _BonusProperties[i].Percents;
        }

        var randomPercent = Random.Range (0, total);

        for (int i = 0; i < _BonusProperties.Length; i++)
        {
            current += _BonusProperties[i].Percents;

            if (current >= randomPercent)
                return _BonusProperties[i].BonusId;
        }

        return _BonusProperties[Random.Range (0, _BonusProperties.Length)].BonusId;
    }

    public int GetQuantity (BonusEnums.BonusId id)
    {
        for (int i = 0; i < _BonusProperties.Length; i++)
        {
            if (_BonusProperties[i].BonusId == id)
                return _BonusProperties[i].quantity;
        }

        return _BonusProperties[Random.Range (0, _BonusProperties.Length)].quantity;
    }

    public Sprite GetIcon (BonusEnums.BonusId id)
    {
        for (int i = 0; i < _BonusProperties.Length; i++)
        {
            if (_BonusProperties[i].BonusId == id)
                return _BonusProperties[i].Icon;
        }

        return null;
    }
}