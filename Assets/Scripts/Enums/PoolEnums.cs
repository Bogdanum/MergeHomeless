using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolEnums
{
    public enum PoolId
    {
        None                  = 0,
        BaseItemMoving        = 1,
        BaseItemGrid          = 2,
        BaseItemBox           = 3,
        FxRaiseGold           = 4,
        FxExplode_Firework    = 5,
        FxSunshine            = 6,
        FxItem_Diamond        = 7,
        FxStarExp             = 8,
        FxItem_Coins          = 9,
        FxUIDisplaySpeedUp    = 10,
        Mission_Layout        = 11,
        Reward_Layout         = 12,
        FxTapUpgrade          = 13,
        InteractEquipmentView = 14,
        FxUIRaiseGold         = 15,
        FxTapBox              = 16,
        FxTapDiamonds         = 17,
        InteractBoostView     = 18,
        FxTapCoins            = 19,
        FxTapFlower           = 20,
        FxLevelUp             = 21,
        FxTapButton           = 22,
        FxLevelUpItem         = 23,
    }

    public readonly static string[] PoolKey =
    {
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16",
        "17",
        "18",
        "19",
        "20",
        "21",
        "22",
        "23",
    };

    public static string GetPoolKey (PoolId id)
    {
        return PoolKey[(int) id];
    }
}