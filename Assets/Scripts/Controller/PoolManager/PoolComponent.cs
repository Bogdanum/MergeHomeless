using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class PoolComponent : MonoBehaviour
{
    [Header ("Pool Options")] [SerializeField]
    public PoolEnums.PoolId poolID;

    [Header ("Float")][SerializeField]
    private float duration;

    private CoroutineHandle handleCoroutine;

    public void OnEnable ()
    {
        if (duration < 0.01f)
            return;

        Timing.KillCoroutines (handleCoroutine);

        handleCoroutine = Timing.RunCoroutine (DoReturn ());
    }

    private IEnumerator<float> DoReturn ()
    {
        yield return Timing.WaitForSeconds (duration);

        ReturnPools ();
    }

    public void OnDisable ()
    {
        Timing.KillCoroutines (handleCoroutine);
    }

    public void ReturnPools ()
    {
        PoolExtension.SetPool (poolID, transform);
    }
}