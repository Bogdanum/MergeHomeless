using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyImage : ScriptableObject
{
    [System.Serializable]
    public struct CurrencyProperty
    {
        public CurrencyEnums.CurrencyId CurrencyId;
        public Sprite                   IconCurrency;
    }

    [SerializeField] private CurrencyProperty[] _CurrencyProperties;

    public Sprite GetIconCurrency (CurrencyEnums.CurrencyId id)
    {
        for (int i = 0; i < _CurrencyProperties.Length; i++)
        {
            if (_CurrencyProperties[i].CurrencyId == id)
            {
                return _CurrencyProperties[i].IconCurrency;
            }
        }

        return null;
    }
}