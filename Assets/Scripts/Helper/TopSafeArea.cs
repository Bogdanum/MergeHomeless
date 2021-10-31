using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSafeArea : MonoBehaviour
{
    [SerializeField] private Transform _TransformArea;

    // Use this for initialization
    private void OnEnable ()
    {
        var screen_area   = Screen.safeArea;
        var screen_height = Screen.height;

        _TransformArea.localPosition = new Vector3 (_TransformArea.localPosition.x,
                                                    _TransformArea.localPosition.y - Mathf.Abs (screen_height - screen_area.height - screen_area.y),
                                                    0);
    }
}