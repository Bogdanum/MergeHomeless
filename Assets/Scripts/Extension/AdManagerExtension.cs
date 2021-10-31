using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AdManagerExtension
{
    public static void ExecuteRewardAds (this MonoBehaviour mono, System.Action OnCompleted, System.Action OnFailed)
    {
        if (AdsManager.InstanceAwake () == null)
        {
            if (OnFailed != null)
            {
                OnFailed ();
            }

            LogGame.Error ("[Ad Manager] Ad Manager Is Null!");

            return;
        }

        AdsManager.Instance.RegisterEvent (OnCompleted, OnFailed);
        AdsManager.Instance.ShowRewardVideo ();
    }

    public static void ExecuteBanner (this MonoBehaviour mono)
    {
        if (AdsManager.InstanceAwake () == null)
        {
            LogGame.Error ("[Ad Manager] Ad Manager Is Null!");

            return;
        }

//        AdsManager.Instance.ShowBanner ();
    }

    public static void DisableBanner (this MonoBehaviour mono)
    {
        if (AdsManager.InstanceAwake () == null)
        {
            LogGame.Error ("[Ad Manager] Ad Manager Is Null!");

            return;
        }
        
//        AdsManager.Instance.HideBanner ();
    }

    public static bool IsRewardVideoAvailable (this MonoBehaviour mono)
    {
        if (AdsManager.InstanceAwake () == null)
        {
            LogGame.Error ("[Ad Manager] Ad Manager Is Null!");

            return false;
        }

        return AdsManager.Instance.IsRewardVideoAvailable;
    }

    public static void RefreshRewardVideo (this MonoBehaviour mono)
    {
        if (AdsManager.InstanceAwake () == null)
        {
            LogGame.Error ("[Ad Manager] Ad Manager Is Null!");
            return;
        }
        
        AdsManager.Instance.RefreshRewardVideo ();
    }
}