using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadingManagerExtension
{
    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, SceneEnums.SceneID sceneID)
    {
        return ExecuteLoading (SceneEnums.GetSceneString (sceneID), null, null, string.Empty);
    }

    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, SceneEnums.SceneID sceneID, System.Action onEnd)
    {
        return ExecuteLoading (SceneEnums.GetSceneString (sceneID), null, onEnd, string.Empty);
    }
    
    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, string scene)
    {
        return ExecuteLoading (scene, null, null, string.Empty);
    }

    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, string scene, string message)
    {
        return ExecuteLoading (scene, null, null, message);
    }

    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, string scene, System.Action onStart)
    {
        return ExecuteLoading (scene, onStart, null, string.Empty);
    }

    public static LoadingManager ExecuteLoading (this MonoBehaviour mono, string scene, System.Action onStart, System.Action onEnd)
    {
        return ExecuteLoading (scene, onStart, onEnd, string.Empty);
    }

    private static LoadingManager ExecuteLoading (string scene, System.Action onStart, System.Action onEnd, string message)
    {
        if (LoadingManager.InstanceAwake () != null)
            return LoadingManager.Instance
                                 .RegisterEvent (onStart, onEnd)
                                 .Execute (scene, true, true, message);

        if (!ReferenceEquals (onStart, null))
        {
            onStart ();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene (scene);

        if (!ReferenceEquals (onEnd, null))
        {
            onEnd ();
        }

        LogGame.Log ("[Loading Manager] Loading with default scene load system!");

        return null;
    }
}