using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FxLevelUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro text_level_up;
    
    public void Init (Vector3 position)
    {
        text_level_up.gameObject.SetActive (true);
        
        transform.position = position;

        text_level_up.text = "Level Up!";//ApplicationLanguage.Text_label_level_up_upgrade;

        text_level_up.transform.localPosition = Vector3.zero;
        
        text_level_up.transform.DOComplete ();
        text_level_up.transform.DOLocalMoveY (1, Durations.DurationFade).OnComplete (() =>
        {
            text_level_up.gameObject.SetActive (false);
        });
    }
}
