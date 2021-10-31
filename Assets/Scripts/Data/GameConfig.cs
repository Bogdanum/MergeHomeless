using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{
    // =========================================== WHEN PLAYER REACH NEW LEVEL PLAYER CAN RECEIVE NEW ITEM WITH NEW LEVEL ============================================ //
    // =========================================== LEVEL = LAST LEVEL PLAYER REACHED - DISTANCE LEVEL ============================================ //
    public const int DistanceLoadLevel = 4;

    // =============================== The number of node can use to earn when start game ================================ //
    public static int _DefaultNumberBaseActive;

    // =============================== The number of node can add for each level upgrade ================================ //
    public static int _NumberBasePlusForEachLevel;

    // =============================== The maximum number of node can add when level completed ================================ //
    public static int _MaxNumberBaseActiveForEarning;

    // =============================== The number of diamonds need to buy the speed up ================================ //
    public static int _DiamondBuySpeedUp;

    // =============================== The max of level player can reach ================================ //
    public static int _MaxLevel;

    // =============================== The max time for speed up ================================ //
    // =============================== Time: /s ================================ //
    public const float MaxTimeSpeedUp = 480;

    // =============================== Time for speed up each watch videos ads ================================ //
    // =============================== Time: /s ================================ //
    public const int TimeSpeedUp = 150;

    // =============================== Time for more cash ================================ //
    public const int TimeMoreCash = 1800;

    // =============================== The number time open shop will the show full screen ================================ //
    // =============================== unit: /turn ================================ //
    public const int AdsShopFullScreen = 4;

    // =============================== The maximum time will reload the new full screen ads ================================ //
    // =============================== Time: /s ================================ //
    public const int AdsTimeReloadFullscreen = 120;

    // =============================== The next player need waiting for another spin ================================ //
    public const int NextTimeWaitingForSpin = 600;

    // =============================== The maximum number time player can spin for each waiting ================================ //
    public const int MaxNumberTurnForSpin = 3;

    // =============================== The maximum time player need to waiting for another ads is appear ================================ //
    public const int MaxTimeWaitingAdsForFreeDiamonds = 120;

    // =============================== The maximum time player can watch ads for free diamonds ================================ //
    public const int MaxTimeEarnDiamondsFree = 3;

    // =============================== The maximum time player need to waiting for another bonus appear ================================ //
    public const int MaxTimeWaitingAppearBonus = 120;

    // =============================== The maximum time player need to waiting for another bonus appear ================================ //
    public const int MaxTimeWaitingAppearCurrency = 30;

    // =============================== The maximum time player can earn coins with multi time ================================ //
    public const int MaxTimeMultiRewardCoins = 120;

    // =============================== The default value player can earn coins with multi time ================================ //
    public const float ValueEarnCoinsMultiTime = 0;

    // =============================== The number diamonds need to use for lucky wheel ================================ //
    public const int DiamondsForSpin = 2;

    // =============================== The maximum time player can get new daily quest ================================ //
    public const int MaxTimeForNextDailyQuest = 86400;

    // =============================== The number diamond will use for get revenue ================================ //
    public const int DiamondUseForRevenueOffline = 5;

    // =============================== The times revenue can get from ads ================================ //
    public const int RevenueCanGetFromAds = 7;

    // =============================== The times revenue can get from diamonds ================================ //
    public const int RevenueCanGetFromDiamond = 3;

    // =============================== The revenue can get during more cash turn on ================================ //
    public const int RevenueCanGetFromMoreCash = 3;

    // =============================== The revenue can get from start ================================ //
    public const int DefaultCoinEarn = 1;

    // =============================== The revenue can get form touch item ================================ //
    public const float PercentCoinEarnFromHitItem = 0.05f;

    public const float PercentCoinEarnFromIdle = 0.1f;

    // =============================== Unlock System ================================ //
    public const int UnlockLuckyWheelLevel = 2;
    public const int UnlockBoosterLevel    = 3;
    public const int UnlockMissionLevel    = 3;
    public const int UnlockEquipmentLevel  = 2;
    public const int UnlockShopLevel       = 2;

    // =============================== The label of version ================================ //
    public static string VersionLabel = "v{0} Merge World © OMan 2019";

    // =============================== Save the total item in the store ================================ //
    public const int TotalItem = 30;
}