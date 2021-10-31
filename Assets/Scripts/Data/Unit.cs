using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : ScriptableObject
{
    [System.Serializable]
    private struct UnitProperty
    {
        public int    Id;
        public string Extension;
    }

    [SerializeField] private UnitProperty[] _UnitProperties;

    public string GetExtensionWithIndex (int id)
    {
        return id < _UnitProperties.Length ? _UnitProperties[id].Extension : _UnitProperties[_UnitProperties.Length - 1].Extension;
    }
}