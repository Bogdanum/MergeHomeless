using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentIconData : ScriptableObject
{
    [System.Serializable]
    public struct EquipmentIconProperty
    {
        public EquipmentEnums.AbilityId Id;
        public Sprite                   Icon;
    }

    [SerializeField] private EquipmentIconProperty[] _EquipmentIconProperties;

    public Sprite GetIcon (EquipmentEnums.AbilityId id)
    {
        for (int i = 0; i < _EquipmentIconProperties.Length; i++)
        {
            if (_EquipmentIconProperties[i].Id == id)
                return _EquipmentIconProperties[i].Icon;
        }

        return null;
    }

    public EquipmentEnums.AbilityId GetId (int index)
    {
        return _EquipmentIconProperties.Length > index ? _EquipmentIconProperties[index].Id : EquipmentEnums.AbilityId.None;
    }

    public int GetSize ()
    {
        return _EquipmentIconProperties.Length;
    }
}