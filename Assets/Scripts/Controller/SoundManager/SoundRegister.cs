using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRegister : MonoBehaviour
{
    [Header ("REFERENCES")]
    [SerializeField]
    private SoundProperties[] soundItems;

    [SerializeField]
    private MusicProperties[] musicItems;

    [Header ("CONFIG")]
    [Tooltip ("It auto clear the sound and music when disable. Register again when enabled.")]
    [SerializeField]
    private bool IsAutomaticClear = false;

    protected void OnEnable ()
    {
        // TODO: Check null sound systems.
        if (object.ReferenceEquals (SoundManager.InstanceAwake (), null))
        {
            // TODO: Break the function.
            return;
        }

        // TODO: init the sound.
        for (int i = 0; i < soundItems.Length; i++)
        {
            // TODO: Register the elements.
            SoundManager.Instance.RegisterSound (soundItems[i]);
        }

        // TODO: register the sound.
        for (int i = 0; i < musicItems.Length; i++)
        {
            // TODO: Register the elements.
            SoundManager.Instance.RegisterMusic (musicItems[i]);
        }
    }

    protected void OnDisable ()
    {
        // TODO: Check the condition remove.
        if (IsAutomaticClear)
        {
            // TODO: Clear the Library
            Clear ();
        }
    }

    /// <summary>
    /// Clear the Library of sound and music.
    /// </summary>
    public void Clear ()
    {
        // TODO: Remove the sound.

        // TODO: Check null sound systems.
        if (object.ReferenceEquals (SoundManager.InstanceAwake (), null))
        {
            // TODO: Break the function.
            return;
        }

        // TODO: init the sound.
        for (int i = 0; i < soundItems.Length; i++)
        {
            // TODO: remove the elements.
            SoundManager.Instance.RemoveSound (soundItems[i]);
        }

        // TODO: register the sound.
        for (int i = 0; i < musicItems.Length; i++)
        {
            // TODO: remove the elements.
            SoundManager.Instance.RemoveMusic (musicItems[i]);
        }
    }
}