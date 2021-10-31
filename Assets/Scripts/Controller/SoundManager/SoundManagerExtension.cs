
using UnityEngine;

public static class SoundManagerExtension  {

    public static void PlayAudioSound (this MonoBehaviour mono ,AudioEnums.SoundId soundId)
    {
        // =============================== PLAY THE SOUND ================================ //
        
        if (ReferenceEquals (SoundManager.InstanceAwake () , null))
        {
            LogGame.Log("[Sound Manager] Sound Manager is null ... ");
            
            return;
        }
        
        SoundManager.Instance.PlaySound (soundId);
        
        LogGame.Log("[Sound Manager] Play the sound of game ... ");
    }

    public static void PlayAudioMusic (this MonoBehaviour mono, AudioEnums.MusicId musicId, bool IsLoop = false, bool IsReset = true)
    {
        // =============================== PLAY THE MUSIC ================================ //
        
        if (ReferenceEquals (SoundManager.InstanceAwake () , null))
        {
            LogGame.Log("[Sound Manager] Sound Manager is null ... ");
            
            return;
        }
        
        SoundManager.Instance.PlayMusic (musicId, IsLoop , IsReset);
        
        LogGame.Log("[Sound Manager] Play The Music of Game ... ");
    }

    public static void SetStateMusic (bool IsEnable)
    {
        // =============================== SET THE STATE OF MUSIC ================================ //
        
        if (ReferenceEquals (SoundManager.InstanceAwake () , null))
        {
            LogGame.Log("[Sound Manager] Sound Manager is null ... ");
            
            return;
        }
        
        SoundManager.Instance.SetStateMusic (IsEnable);
    }

    public static void SetStateSound (bool IsEnable)
    {
        // =============================== SET THE STATE OF SOUND ================================ //
        
        if (ReferenceEquals (SoundManager.InstanceAwake () , null))
        {
            LogGame.Log("[Sound Manager] Sound Manager is null ... ");
            
            return;
        }
        
        SoundManager.Instance.SetStateSound (IsEnable);
    }
}
