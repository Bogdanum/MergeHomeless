using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsData : ScriptableObject
{
    [System.Serializable]
    public struct ItemsProperty
    {
        public int id;
        public Sprite item_icon;
    }
    
    [SerializeField] private ItemsProperty[] ItemIcon;

    #region Helper

    public Sprite GetIcon (int id)
    {
        for (int i = 0; i < ItemIcon.Length; i++)
        {
            if (id == ItemIcon[i].id)
                return ItemIcon[i].item_icon;
        }

        return null;
    }
    
    public ItemsProperty GetRandom ()
    {
        return ItemIcon[Random.Range (0, ItemIcon.Length)];
    }

    #endregion
}
