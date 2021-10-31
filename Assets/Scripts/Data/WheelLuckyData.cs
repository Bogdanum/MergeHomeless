using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartLocalization;

public class WheelLuckyData : ScriptableObject
{
    [System.Serializable]
    public struct WheelLuckyProperty
    {
        public                  RewardEnums.RewardId Id;
        public                  int                  Quantity;
        [Range (0, 100)] public int                  Percent;
        public                  Sprite               Icon;
        public                  string               DescriptionID;
    }

    [SerializeField] private WheelLuckyProperty[] _WheelLuckyProperties;

    public RewardEnums.RewardId GetRandomReward ()
    {
        var total   = 0;
        var current = 0;

        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            total += _WheelLuckyProperties[i].Percent;
        }

        var randomPercent = Random.Range (0, total);

        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            current += _WheelLuckyProperties[i].Percent;

            if (current >= randomPercent)
                return _WheelLuckyProperties[i].Id;
        }

        return _WheelLuckyProperties[Random.Range (0, _WheelLuckyProperties.Length)].Id;
    }

    public int GetSize ()
    {
        return _WheelLuckyProperties.Length;
    }

    public int GetQuantity (RewardEnums.RewardId id)
    {
        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            if (_WheelLuckyProperties[i].Id == id)
                return _WheelLuckyProperties[i].Quantity;
        }

        return _WheelLuckyProperties[Random.Range (0, _WheelLuckyProperties.Length)].Quantity;
    }

    public Sprite GetIcon (RewardEnums.RewardId id)
    {
        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            if (_WheelLuckyProperties[i].Id == id)
                return _WheelLuckyProperties[i].Icon;
        }

        return null;
    }

    public string GetDescription (RewardEnums.RewardId id)
    {
        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            if (_WheelLuckyProperties[i].Id == id)
                return LanguageManager.Instance.GetTextValue(_WheelLuckyProperties[i].DescriptionID);
        }

        return null;
    }

    public Sprite GetIcon (int index)
    {
        return _WheelLuckyProperties.Length > index ? _WheelLuckyProperties[index].Icon : null;
    }

    public int GetQuantity (int index)
    {
        return _WheelLuckyProperties.Length > index ? _WheelLuckyProperties[index].Quantity : 0;
    }

    public int GetIndexReward (RewardEnums.RewardId id)
    {
        for (int i = 0; i < _WheelLuckyProperties.Length; i++)
        {
            if (_WheelLuckyProperties[i].Id == id)
                return i;
        }

        return 0;
    }
}