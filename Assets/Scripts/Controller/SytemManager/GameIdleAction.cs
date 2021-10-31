using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class GameIdleAction : Singleton<GameIdleAction>
{
    #region Variables

    private readonly List<IIdle>           idle_object            = new List<IIdle> ();
    private readonly List<CoroutineHandle> handle_idle_enumerator = new List<CoroutineHandle> ();

    #endregion

    #region Action

    public void RegisterIdle (IIdle iIdle, float time)
    {
        if (idle_object.Contains (iIdle))
            return;

        idle_object.Add (iIdle);
        
        handle_idle_enumerator.Add (Timing.RunCoroutine (enumerator_idle(iIdle , time)));
    }

    public void UnRegisterIdle (IIdle idle)
    {
        var indexOf = idle_object.IndexOf (idle);

        if (indexOf < 0)
            return;

        Timing.KillCoroutines (handle_idle_enumerator[indexOf]);
        
        idle_object.RemoveAt (indexOf);
        handle_idle_enumerator.RemoveAt (indexOf);
    }

    #endregion

    #region Enumerator

    private IEnumerator<float> enumerator_idle (IIdle iIdle, float time)
    {
        // 21.10.2021 --------  old value (0.0f, 1.0f)
        yield return Timing.WaitForSeconds (Random.Range (0.0f, 1.0f));
        
        while (!iIdle.IsStop ())
        {
            yield return Timing.WaitForSeconds (time);
            
            iIdle.EarnCoins ();
            
            this.PlayAudioSound (AudioEnums.SoundId.TapOnItem);
            
            if (Random.Range (25.00f, 30.00f) < 25.25f)
            {
                this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);
            }
        }
    }

    #endregion
}