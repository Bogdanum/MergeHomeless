using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    [Header ("SCENE CONFIG")] [SerializeField]
    private SceneEnums.SceneID sceneID = SceneEnums.SceneID.SceneEmpty;

    // Use this for initialization
    private void Start ()
    {
        this.ExecuteLoading (sceneID);
    }
}