using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageIcon : ScriptableObject
{
    [System.Serializable]
    public struct LanguageProperty
    {
        public LanguageEnums.LanguageId Id;
        public Sprite                   Icon;
    }

    [SerializeField] private LanguageProperty[] _LanguageProperties;

    public Sprite GetIcon (LanguageEnums.LanguageId id)
    {
        for (int i = 0; i < _LanguageProperties.Length; i++)
        {
            if (_LanguageProperties[i].Id == id)
            {
                return _LanguageProperties[i].Icon;
            }
        }

        return null;
    }
}