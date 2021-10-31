using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MissionEnums
{
    public enum MissionId
    {
        None,
        Merge,
        GetBonus,
        Tutorial,
        ReachLevel,
        Spin,
        ReachCats,
        UpgradeItem,
        BuyItem,
        TapOnItem,
        TapOnBox,
    }

    private readonly static string[] MissionKey =
    {
        "none",
        "merge",
        "get_bonus",
        "tutorial",
        "reach_level",
        "spin",
        "reach_cats",
        "upgrade_item",
        "buy_item",
        "tap_item",
        "tap_box",
    };

    public static string GetKey (MissionId id)
    {
        return MissionKey[(int) id];
    }

    public static int GetSize ()
    {
        return MissionKey.Length;
    }
}