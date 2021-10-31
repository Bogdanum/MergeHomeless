using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FXCoin : MonoBehaviour
{
    [SerializeField] private TextMeshPro      _TextCoins;
    [SerializeField] private PoolEnums.PoolId _PoolId;

    public void Enable (Vector3 position, string value)
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive (true);

        _TextCoins.text    = value;
        transform.position = position;

        transform.DOComplete (true);
        transform.DOMoveY (position.y + 1, Durations.DurationMovingUpFx).OnComplete (() => { PoolExtension.SetPool (_PoolId, transform); });
    }
}