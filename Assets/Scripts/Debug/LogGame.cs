using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public static class LogGame
{
    [System.Diagnostics.Conditional ("LOG_ENABLE")]
    public static void Log (string message)
    {
        Debug.Log (message);
    }

    [System.Diagnostics.Conditional ("LOG_ENABLE")]
    public static void Error (string message)
    {
        Debug.LogError (message);
    }

    [System.Diagnostics.Conditional ("LOG_ENABLE")]
    public static void Alert (string message)
    {
        Debug.LogWarning (message);
    }
}