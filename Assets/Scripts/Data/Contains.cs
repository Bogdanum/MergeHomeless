using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Contains
{
    public static int StartIndexPoint = 0;
    public static int ExpNeedReach;

    public const float ShakeScale = 0.2f;

    public static int   NumberTimeOpenShop;
    public static float LastTimeShowFSAShop;

    public static int time_to_push = 36000;

    public static bool IsSoundOn
    {
        get { return PlayerPrefs.GetInt ("IsSoundOn", 1) == 1; }
        set { PlayerPrefs.SetInt ("IsSoundOn", value ? 1 : 0); }
    }

    public static bool IsMusicOn
    {
        get { return PlayerPrefs.GetInt ("IsMusicOn", 1) == 1; }
        set { PlayerPrefs.SetInt ("IsMusicOn", value ? 1 : 0); }
    }

    public static bool IsBatteryOn
    {
        get { return PlayerPrefs.GetInt ("IsBatteryOn", 0) == 1; }
        set { PlayerPrefs.SetInt ("IsBatteryOn", value ? 1 : 0); }
    }

    public static bool IsNotificationOn
    {
        get { return PlayerPrefs.GetInt ("IsNotificationOn", 1) == 1; }
        set { PlayerPrefs.SetInt ("IsNotificationOn", value ? 1 : 0); }
    }

    public static string LastVersionUpdate
    {
        get { return PlayerPrefs.GetString ("VersionUpdate", Version.bundleVersion); }
        set { PlayerPrefs.SetString ("VersionUpdate", value); }
    }

    public static float MultiRewardFromCoins = 0;
    public static float MultiRewardFromSpeed = 0;
}

public static class Vector
{
    public static Vector3 Vector3One        = new Vector3 (1, 1, 1);
    public static Vector3 Vector3Zero       = new Vector3 (0, 0, 0);
    public static Vector3 Vector3Null       = new Vector3 (10000, 10000, 10000);
    public static Vector3 Vector3PunchScale = new Vector3 (0.1f, 0.1f, 0.1f);
}

public static class Durations
{
    public const float DurationMovingMerge = 0.2f;
    public const float DurationMoving      = 0.1f;
    public const float DurationMovingLine  = 0.5f;
    public const float DurationShakeScale  = 0.2f;
    public const float DurationShake       = 0.1f;
    public const float DurationFillAmount  = 0.2f;
    public const float DurationTimeScale   = 0.25f;
    public const float DurationScale       = 0.2f;
    public const float DurationDrop        = 0.2f;
    public const float DurationMovingUpFx  = 0.5f;
    public const float DurationMovingBack  = 1f;
    public const float DurationFade        = 0.35f;
    public const float DurationMovingHand  = 1f;
}