using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodeData : ScriptableObject
{
    [System.Serializable]
    public struct GridNodeProperty
    {
        public CurrencyEnums.CurrencyId CurrencyId;
        public int                      Level;
        public float                    Price;
        public int                      PriceUnit;
    }

    [SerializeField] private GridNodeProperty[] grid_node_properties;

    #region Helper

    public GridNodeProperty GetData (int level)
    {
        for (int i = 0; i < grid_node_properties.Length; i++)
        {
            if (grid_node_properties[i].Level == level)
                return grid_node_properties[i];
        }

        return new GridNodeProperty ()
        {
            Level      = 0,
            CurrencyId = CurrencyEnums.CurrencyId.Lock,
            Price      = 0,
            PriceUnit  = 0,
        };
    }

    #endregion
}