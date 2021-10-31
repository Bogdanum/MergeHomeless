using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorTools
{
    [MenuItem ("Editor/Clear")]
    public static void Clear ()
    {
        PlayerData.Clear ();
    }
    
    
}