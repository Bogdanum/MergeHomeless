using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageEnums
{
    public enum LanguageId
    {
        Chinese,
        English,
        Russian,
        Arabic,
        Portuguese,
        Spanish,
        Japanese,
    }

    private readonly static string[] LanguageKey =
    {
        "zh-CHS",
        "en",
        "ru",
        "ar",
        "pt",
        "es",
        "ja",
    };

    public static string GetLanguageDevice ()
    {
        var language = Application.systemLanguage;

        switch (language)
        {
            case SystemLanguage.ChineseSimplified:
                return GetLanguageKey (LanguageId.Chinese);
            case SystemLanguage.ChineseTraditional:
                return GetLanguageKey (LanguageId.Chinese);
            case SystemLanguage.Japanese:
                return GetLanguageKey (LanguageId.Japanese);
            case SystemLanguage.English:
                return GetLanguageKey (LanguageId.English);
            case SystemLanguage.Spanish:
                return GetLanguageKey (LanguageId.Spanish);
            case SystemLanguage.Russian:
                return GetLanguageKey (LanguageId.Russian);
            case SystemLanguage.Arabic:
                return GetLanguageKey (LanguageId.Arabic);
            case SystemLanguage.Portuguese:
                return GetLanguageKey(LanguageId.Portuguese);
            default:
                return GetLanguageKey (LanguageId.English);
        }
    }

    public static string GetLanguageSupportDefault ()
    {
        var language = Application.systemLanguage;

        switch (language)
        {
            case SystemLanguage.Arabic:
                return GetLanguageKey (LanguageId.Arabic);
            case SystemLanguage.Russian:
                return GetLanguageKey(LanguageId.Russian);
            case SystemLanguage.Spanish:
                return GetLanguageKey(LanguageId.Spanish);
            case SystemLanguage.Portuguese:
                return GetLanguageKey(LanguageId.Portuguese);
            case SystemLanguage.Japanese:
                return GetLanguageKey(LanguageId.Japanese);
            default:
                return GetLanguageKey (LanguageId.English);
        }
    }

    public static string GetLanguageKey (LanguageId id)
    {
        return LanguageKey[(int) id];
    }

    public static LanguageId GetLanguageId (string language_code)
    {
        for (int i = 0; i < LanguageKey.Length; i++)
        {
            if (string.CompareOrdinal (LanguageKey[i], language_code) == 0)
            {
                return (LanguageId) i;
            }
        }

        return LanguageId.English;
    }
}