using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialManagerExtension
{
    public static void ExecuteTutorial (this MonoBehaviour mono, TutorialEnums.TutorialId id)
    {
        if (ReferenceEquals (TutorialManager.InstanceAwake (), null))
        {
            LogGame.Log ("[Tutorial Manager] Tutorial is null.");

            return;
        }

        TutorialManager.Instance.ExecuteTutorial (id);
    }

    public static bool IsTutorialCompleted (this MonoBehaviour mono, TutorialEnums.TutorialId id)
    {
        return PlayerData.IsTutorialCompleted (id);
    }

    public static void SetStateTutorial (this MonoBehaviour mono, TutorialEnums.TutorialId id, bool isCompleted)
    {
        PlayerData.SaveTutorial (id, isCompleted);

        if (ReferenceEquals (TutorialManager.InstanceAwake (), null))
        {
            LogGame.Log ("[Tutorial Manager] Tutorial is null.");

            return;
        }

        TutorialManager.Instance.SetStateCompleted (isCompleted);
    }

    public static void PostCompletedTutorial (this MonoBehaviour mono, TutorialEnums.TutorialId id)
    {
        if (ReferenceEquals (TutorialManager.InstanceAwake (), null))
        {
            LogGame.Log ("[Tutorial Manager] Tutorial is null.");

            return;
        }

        TutorialManager.Instance.PostCompleted (id);
    }

    public static void PostTapAnyWhere (this MonoBehaviour mono)
    {
        if (ReferenceEquals (TutorialManager.InstanceAwake (), null))
        {
            LogGame.Log ("[Tutorial Manager] Tutorial is null.");

            return;
        }
 
        TutorialManager.Instance.PostTapAnyWhere ();
    }
}