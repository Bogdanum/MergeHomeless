using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopDetail : ScriptableObject
{
    [System.Serializable]
    public struct ItemShopProperty
    {
        public int    Level;
        public string Name;
        public float  Speed;
        public float  Earning;
    }

    [SerializeField] private ItemShopProperty[] _ItemShopProperties;
    
    public ItemShopProperty GetItem (int item_level)
    {
        for (int i = 0; i < _ItemShopProperties.Length; i++)
        {
            if (_ItemShopProperties[i].Level == item_level)
                return _ItemShopProperties[i];
        }

        return new ItemShopProperty ()
        {
            Level   = -1,
            Name    = "????",
            Speed   = 0,
            Earning = 0,
        };
    }

    public float GetSpeed (int item_level)
    {
        for (int i = 0; i < _ItemShopProperties.Length; i++)
        {
            if (_ItemShopProperties[i].Level == item_level)
                return _ItemShopProperties[i].Speed;
        }

        return 0;
    }

    public float GetEarning (int item_level)
    {
        for (int i = 0; i < _ItemShopProperties.Length; i++)
        {
            if (_ItemShopProperties[i].Level == item_level)
                return _ItemShopProperties[i].Earning;
        }

        return 0;
    }

    public string GetName (int item_level)
    {
        for (int i = 0; i < _ItemShopProperties.Length; i++)
        {
            if (_ItemShopProperties[i].Level == item_level)
                return _ItemShopProperties[i].Name;
        }

        return "????";
    }
}