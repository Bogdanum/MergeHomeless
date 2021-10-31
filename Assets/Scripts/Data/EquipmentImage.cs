using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentImage : ScriptableObject
{
    [System.Serializable]
    public struct EquipmentProperties
    {
        public EquipmentEnums.AbilityId id;
        public Sprite                   Texture;
    }

    [Header ("Data")] [SerializeField] private EquipmentProperties[] _Equipment;

    public Sprite GetIcon (EquipmentEnums.AbilityId id)
    {
        for (int i = 0; i < _Equipment.Length; i++)
        {
            if (_Equipment[i].id == id)
            {
                return _Equipment[i].Texture;
            }
        }

        return null;
    }
}