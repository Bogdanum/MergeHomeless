using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public struct SoundProperties
{
    public AudioEnums.SoundId SoundId;
    public AudioClip          AudioClip;
}

[System.Serializable]
public struct MusicProperties
{
    public AudioEnums.MusicId MusicId;
    public AudioClip          AudioClip;
}


public class SoundManager : Singleton<SoundManager>
{
    [Header ("CONTROLLER")] [SerializeField]
    private AudioSource _MusicAudioSource;

    [SerializeField] private AudioSource _SoundAudioSource;


    // ================================ Variables ========================= //

    #region DICTIONARY

    protected Dictionary<int, List<AudioClip>> _SoundDictionary = new Dictionary<int, List<AudioClip>> ();
    protected Dictionary<int, List<AudioClip>> _MusicDictionary = new Dictionary<int, List<AudioClip>> ();

    #endregion

    protected override void Awake ()
    {
        base.Awake ();

        RegisterAudioSource ();

        SetStateSound (Contains.IsSoundOn);
        SetStateMusic (Contains.IsMusicOn);
    }

    private void OnApplicationPause (bool pauseStatus)
    {
        PauseStateSound (pauseStatus);
    }

    public void PauseStateSound (bool pauseStatus)
    {
        if (pauseStatus)
        {
            SetStateMusic (false);
            SetStateSound (false);
        }
        else
        {
            SetStateMusic (Contains.IsMusicOn);
            SetStateSound (Contains.IsSoundOn);
        }
    }

    private void RegisterAudioSource ()
    {
        // =============================== REGISTER THE AUDIO SOURCE FOR THE GAME PLAY ================================ //

        if (_MusicAudioSource == null)
        {
            _MusicAudioSource = gameObject.AddComponent<AudioSource> ();
        }

        if (_SoundAudioSource == null)
        {
            _SoundAudioSource = gameObject.AddComponent<AudioSource> ();
        }
    }

    public void RegisterSound (SoundProperties soundProperties)
    {
        // TODO: get the index of sound.
        var soundId = (int) soundProperties.SoundId;

        List<AudioClip> audioList;

        if (_SoundDictionary.TryGetValue (soundId, out audioList))
        {
            audioList.Add (soundProperties.AudioClip);
            _SoundDictionary[soundId] = audioList;
        }
        else
        {
            _SoundDictionary.Add (soundId, new List<AudioClip> {soundProperties.AudioClip});
        }
    }

    public void RemoveSound (SoundProperties soundProperties)
    {
        // =============================== REMOVE THE SOUND ITEM FROM THE REGISTER ================================ //

        var indexSound = (int) soundProperties.SoundId;

        List<AudioClip> audioList;

        if (!_SoundDictionary.TryGetValue (indexSound, out audioList)) return;

        audioList.Remove (soundProperties.AudioClip);

        _SoundDictionary[indexSound] = audioList;

        if (audioList.Count == 0)
        {
            _SoundDictionary.Remove (indexSound);
        }
    }

    public void RegisterMusic (MusicProperties musicProperties)
    {
        // TODO: get the index of sound.
        var musicId = (int) musicProperties.MusicId;

        List<AudioClip> audioList;

        if (_MusicDictionary.TryGetValue (musicId, out audioList))
        {
            audioList.Add (musicProperties.AudioClip);
            _MusicDictionary[musicId] = audioList;
        }
        else
        {
            _MusicDictionary.Add (musicId, new List<AudioClip> {musicProperties.AudioClip});
        }
    }

    public void RemoveMusic (MusicProperties musicProperties)
    {
        // =============================== REMOVE THE MUSIC ITEM FROM THE REGISTER ================================ //

        var indexMusic = (int) musicProperties.MusicId;

        List<AudioClip> audioList;

        if (!_MusicDictionary.TryGetValue (indexMusic, out audioList)) return;

        audioList.Remove (musicProperties.AudioClip);

        _MusicDictionary[indexMusic] = audioList;

        if (audioList.Count == 0)
        {
            _MusicDictionary.Remove (indexMusic);
        }
    }

    #region CONTROLLER

    public void SetStateSound (bool isEnable)
    {
        if (isEnable)
        {
            EnableSound ();
        }
        else
        {
            DisableSound ();
        }
    }

    public void SetStateMusic (bool isEnable)
    {
        if (isEnable)
        {
            EnableMusic ();
        }
        else
        {
            DisableMusic ();
        }
    }

    public void StopMusic ()
    {
        _MusicAudioSource.DOKill (true);
        _MusicAudioSource.DOFade (0, VariablesEnums.AnimationDurationFade).OnComplete (() => { _MusicAudioSource.Stop (); });
    }

    public void DisableSound (bool IsUseAnimation = true)
    {
        if (_SoundAudioSource == null) return;

        _SoundAudioSource.DOKill ();

        if (IsUseAnimation)
        {
            _SoundAudioSource.DOFade (0, VariablesEnums.AnimationDurationFade).OnComplete (() => { _SoundAudioSource.mute = true; }).SetUpdate (true);
        }
        else
        {
            _SoundAudioSource.mute   = true;
            _SoundAudioSource.volume = 0;
        }
    }

    public void EnableSound (bool IsUseAnimation = true)
    {
        if (_SoundAudioSource == null) return;

        _SoundAudioSource.mute = false;
        _SoundAudioSource.DOKill ();

        if (IsUseAnimation)
        {
            _SoundAudioSource.DOFade (VariablesEnums.VolumeSoundPlayer, VariablesEnums.AnimationDurationFade).SetUpdate (true);
        }
        else
        {
            _SoundAudioSource.volume = VariablesEnums.VolumeSoundPlayer;
        }
    }

    public void DisableMusic (bool IsUseAnimation = true)
    {
        if (_MusicAudioSource == null) return;
        _MusicAudioSource.DOKill ();

        if (IsUseAnimation)
        {
            _MusicAudioSource.DOFade (0, VariablesEnums.AnimationDurationFade).OnComplete (() => { _MusicAudioSource.mute = true; }).SetUpdate (true);
        }
        else
        {
            _MusicAudioSource.mute = true;

            _MusicAudioSource.volume = 0;
        }
    }

    public void EnableMusic (bool IsUseAnimation = true)
    {
        if (_MusicAudioSource != null)
        {
            _MusicAudioSource.mute = false;

            _MusicAudioSource.DOKill ();

            if (IsUseAnimation)
            {
                _MusicAudioSource.DOFade (VariablesEnums.VolumeMusicPlayer, VariablesEnums.AnimationDurationFade).SetUpdate (true);
                ;
            }
            else
            {
                _MusicAudioSource.volume = VariablesEnums.VolumeMusicPlayer;
            }
        }
    }

    #endregion

    public void PlaySound (AudioEnums.SoundId soundId)
    {
        List<AudioClip> audioFound;

        if (!_SoundDictionary.TryGetValue ((int) soundId, out audioFound)) return;

        if (audioFound.Count > 0)
        {
            _SoundAudioSource.PlayOneShot (audioFound[Random.Range (0, audioFound.Count)]);
        }
    }

    public void PlayMusic (AudioEnums.MusicId musicId, bool IsLoop = false, bool IsReset = true)
    {
        List<AudioClip> audioFound;

        _MusicAudioSource.DOComplete (true);

        if (!_MusicDictionary.TryGetValue ((int) musicId, out audioFound)) return;

        if (audioFound.Count <= 0) return;

        var index = Random.Range (0, audioFound.Count);
        if (!object.ReferenceEquals (_MusicAudioSource.clip, null) && !IsReset && audioFound[index].name == _MusicAudioSource.clip.name) return;

        _MusicAudioSource.DOFade (0, VariablesEnums.AnimationDurationFade).OnComplete (() =>
        {
            _MusicAudioSource.clip = audioFound[index];
            _MusicAudioSource.loop = IsLoop;
            _MusicAudioSource.Play ();
            _MusicAudioSource.DOFade (VariablesEnums.VolumeMusicPlayer, VariablesEnums.AnimationDurationFade);
        });
    }
}