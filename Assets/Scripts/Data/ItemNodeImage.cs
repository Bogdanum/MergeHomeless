using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemNodeImage : ScriptableObject
{
    [System.Serializable]
    public struct ItemProperties
    {
        public int    Level;
        public Sprite Icon;
    }

    [Header ("Data")] [SerializeField] private ItemProperties[] _Item;

    public Sprite GetIcon (int level)
    {
        for (int i = 0; i < _Item.Length; i++)
        {
            if (_Item[i].Level == level)
            {
                return _Item[i].Icon;
            }
        }

        return null;
    }
}