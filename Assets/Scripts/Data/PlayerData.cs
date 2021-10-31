using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class PlayerData
{
    public static double Coins;
    public static int    CoinUnit;

    public static double Coca;
    public static int    CocaUnit;

    public static double Food;
    public static int    FoodUnit;

    public static int Diamonds;
    public static int Level;
    public static int LastLevelUnlocked;
    public static int TotalTimeSpeedUp;
    public static int TotalTimeMoreCash;
    public static int TotalTimeMultiRewardCoins;
    public static int Exp;
    public static int _LastNumberTurnSpin;

    public static bool IsWatchAdsForSpin;

    public static string _LastTimeForSpeedUp;
    public static string _LastTimeForMoreCash;
    public static string _LastTimeOnline;
    public static string _LastTimeSpinLucky;
    public static string _LastTimeWatchAdsForFreeDiamonds;
    public static string _LastTimeAppearBonusReward;
    public static string _LastTimeMultiRewardCoins;
    public static string _LastTimeDailyQuest;
    public static string _LastTimeAppearBonusCurrency;

    public static string DefaultLanguage;


    private const string KeyCoins                       = "PlayerCoins";
    private const string KeyCoinsUnit                   = "PlayerCoinsUnit";
    private const string KeyFood                        = "PlayerFood";
    private const string KeyFoodUnit                    = "PlayerFoodUnit";
    private const string KeyLevel                       = "PlayerLevel";
    private const string KeyDiamonds                    = "PlayerDiamonds";
    private const string KeyCoca                        = "PlayerCoca";
    private const string KeyCocaUnit                    = "PlayerCocaUnit";
    private const string KeyExp                         = "PlayerExp";
    private const string KeyTimeTotalSpeedUp            = "PlayerTotalTimeSpeedUp";
    private const string KeyTimeTotalMoreCash           = "PlayerTotalTimeMoreCash";
    private const string KeyTimeOnline                  = "PlayerTimeOnline";
    private const string KeyTimeSpinLucky               = "PlayerTimeSpin";
    private const string KeyNumberTurnSpin              = "PlayerNumberTurnSpin";
    private const string KeyWatchAdsForFreeDiamonds     = "PlayerTimeWatchAdsFreeDiamonds";
    private const string KeyLastTimeAppearBonusReward   = "PlayerAppearBonus";
    private const string KeyLastTimeMultiRewardCoins    = "PlayerMultiRewardCoins";
    private const string KeyTotalTimeMultiRewardCoins   = "PlayerTotalTimeMultiRewardCoins";
    private const string KeyTimeDailyQuest              = "PlayerTimeDailyQuest";
    private const string KeyLastTimeAppearBonusCurrency = "PlayerAppearCurrency";
    private const string KeyTimeForSpeedUp              = "PlayerTimeSpeedUp";
    private const string KeyTimeForMoreCash             = "PlayerTimeMoreCash";

    private const string KeyLevelUnlocked     = "PlayerLevelUnlocked";
    private const string KeyItemMoving        = "ItemMoving-{0}-{1}";
    private const string KeyItemStatic        = "ItemStatic-{0}-{1}";
    private const string KeyTotalItemMoving   = "PlayerTotalItemMoving";
    private const string KeyNumberBuyItem     = "ShopBuyTimeItem-{0}";
    private const string KeyNumberUpgradeItem = "ShopUpgradeTimeItem-{0}";
    private const string KeyTutorialId        = "TutorialId-{0}";
    private const string KeyDefaultLanguage   = "PlayerLanguage";
    private const string KeyMissionId         = "MissionId-{0}";
    private const string KeyMissionLevel      = "MissionLevel-{0}";
    private const string KeyEquipmentUpgrade  = "EquipmentUpgrade-{0}";
    private const string KeyWatchAdsForSpin   = "PlayerWatchForSpin";
    private const string KeyLengthUnlockGrid  = "LengthUnlockGrid";

    private const string KeyUnlockGrid = "UnlockGrid-{0}";

    private static List<int> ShopItemProfitPercent;
    private static List<int> ShopItemUpgradeProfitPercent;
    private static List<int> TutorialId;

    public static List<int> MissionId;
    public static List<int> MissionLevel;

    private static List<int> EquipmentUpgrade;
    private static List<int> UnlockGrid;

    private static double _LastCoinSaved;
    private static int    _LastCoinUnitSaved;

    private static double LastFoodSaved;
    private static int    LastFoodUnitSaved;

    private static double _LastCocaSaved;
    private static int    _LastCocaUnitSaved;

    private static int length_unlock_grid;

    public static bool IsRemoveAds
    {
        get { return PlayerPrefs.GetInt ("RemoveAds", 0) == 1; }
        set { PlayerPrefs.SetInt ("RemoveAds", value == false ? 0 : 1); }
    }

    public static bool IsOpenRate
    {
        get { return PlayerPrefs.GetInt ("RateOpened", 0) == 1; }
        set { PlayerPrefs.SetInt ("RateOpened", value == false ? 0 : 1); }
    }

    public static void Save ()
    {
        SaveCoins ();
        SaveUnlockItemLevel ();
        SaveDiamonds ();
        SaveCoca ();
        SaveLevel ();
        SaveTotalTimeSpeedUp ();
        SaveExp ();
    }

    public static void SaveUnlockItemLevel ()
    {
        SavePref (KeyLevelUnlocked, LastLevelUnlocked);
    }

    public static void SaveExp ()
    {
        SavePref (KeyExp, Exp);
    }

    public static void SaveDiamonds ()
    {
        SavePref (KeyDiamonds, Diamonds);
    }

    public static void SaveTotalTimeSpeedUp ()
    {
        SavePref (KeyTimeTotalSpeedUp, TotalTimeSpeedUp);
    }

    public static void SaveTotalTimeMoreCash ()
    {
        SavePref (KeyTimeTotalMoreCash, TotalTimeMoreCash);
    }

    public static void SaveCoins ()
    {
        if ((int) _LastCoinSaved != (int) Coins)
        {
            SavePref (KeyCoins, Coins.ToString ());

            _LastCoinSaved = Coins;
        }

        if (_LastCoinUnitSaved != CoinUnit)
        {
            SavePref (KeyCoinsUnit, CoinUnit);

            _LastCoinUnitSaved = CoinUnit;
        }
    }


    public static void SaveFood ()
    {
        if ((int) LastFoodSaved != (int) Food)
        {
            SavePref (KeyFood, Food.ToString ());

            LastFoodSaved = Food;
        }

        if (LastFoodUnitSaved != FoodUnit)
        {
            SavePref (KeyFoodUnit, FoodUnit);

            LastFoodUnitSaved = FoodUnit;
        }
    }


    public static void SaveCoca ()
    {
        if ((int) _LastCocaSaved != (int) Coca)
        {
            SavePref (KeyCoca, Coca.ToString ());

            _LastCocaSaved = Coca;
        }

        if (_LastCocaUnitSaved != CocaUnit)
        {
            SavePref (KeyCocaUnit, CocaUnit);

            _LastCocaUnitSaved = CocaUnit;
        }
    }

    public static void SaveLevel ()
    {
        SavePref (KeyLevel, Level);
    }

    public static void SaveTotalItemMoving (int value)
    {
        SavePref (KeyTotalItemMoving, value);
    }

    public static void SaveItemMoving (int item_level, int xColumn, int yRow)
    {
        SavePref (string.Format (KeyItemMoving, xColumn.ToString (), yRow.ToString ()), item_level);
    }

    public static int LoadItemMoving (int xColumn, int yRow)
    {
        return LoadPref (string.Format (KeyItemMoving, xColumn.ToString (), yRow.ToString ()), -1);
    }

    public static void SaveItemStatic (int item_level, int xColumn, int yRow)
    {
        SavePref (string.Format (KeyItemStatic, xColumn.ToString (), yRow.ToString ()), item_level);
    }

    public static int LoadLevelItemStatic (int xColumn, int yRow)
    {
        return LoadPref (string.Format (KeyItemStatic, xColumn.ToString (), yRow.ToString ()), -1);
    }

    public static void SaveItemProfitCoefficient (int itemLevel, int numberBuy)
    {
        if (itemLevel <= ShopItemProfitPercent.Count)
        {
            ShopItemProfitPercent[itemLevel - 1] = numberBuy;
        }
        else
        {
            for (int i = ShopItemProfitPercent.Count; i < itemLevel; i++)
            {
                ShopItemProfitPercent.Add (0);
            }

            ShopItemProfitPercent.Add (numberBuy);
        }

        SavePref (string.Format (KeyNumberBuyItem, (itemLevel - 1).ToString ()), numberBuy);
    }

    public static void SaveItemUpgradeProfitCoefficient (int item_level, int number_buy)
    {
        if (item_level <= ShopItemUpgradeProfitPercent.Count)
        {
            ShopItemUpgradeProfitPercent[item_level - 1] = number_buy;
        }
        else
        {
            for (int i = ShopItemUpgradeProfitPercent.Count; i < item_level; i++)
            {
                ShopItemUpgradeProfitPercent.Add (0);
            }

            ShopItemUpgradeProfitPercent.Add (number_buy);
        }

        SavePref (string.Format (KeyNumberUpgradeItem, (item_level - 1).ToString ()), number_buy);
    }

    public static int GetNumberBuyItemProfitCoefficient (int itemLevel)
    {
        return ShopItemProfitPercent[itemLevel - 1];
    }

    public static int GetNumberUpgradeItemProfitCoefficient (int item_level)
    {
        return ShopItemUpgradeProfitPercent[item_level - 1];
    }

    public static void SaveLastTimeOnline ()
    {
        SavePref (KeyTimeOnline, _LastTimeOnline);
    }

    public static void SaveLastTimeSpinLucky ()
    {
        SavePref (KeyTimeSpinLucky, _LastTimeSpinLucky);
    }

    public static void SaveNumberTurnSpin ()
    {
        SavePref (KeyNumberTurnSpin, _LastNumberTurnSpin);
    }

    public static bool IsTutorialCompleted (TutorialEnums.TutorialId id)
    {
        return TutorialId[(int) id] == 1;
    }

    public static void SaveTutorial (TutorialEnums.TutorialId id, bool isComplete)
    {
        TutorialId[(int) id] = isComplete ? 1 : 0;
        SavePref (string.Format (KeyTutorialId, ((int) id).ToString ()), isComplete ? 1 : 0);
    }

    public static void SaveLastTimeWatchAdsForFreeDiamonds ()
    {
        SavePref (KeyWatchAdsForFreeDiamonds, _LastTimeWatchAdsForFreeDiamonds);
    }

    public static void SaveLastTimeAppearBonusReward ()
    {
        SavePref (KeyLastTimeAppearBonusReward, _LastTimeAppearBonusReward);
    }

    public static void SaveLastTimeAppearBonusCurrency ()
    {
        SavePref (KeyLastTimeAppearBonusCurrency, _LastTimeAppearBonusCurrency);
    }

    public static void SaveLastTimeMultiRewardCoins ()
    {
        SavePref (KeyLastTimeMultiRewardCoins, _LastTimeMultiRewardCoins);
    }

    public static void SaveTotalTimeMultiRewardCoins ()
    {
        SavePref (KeyTotalTimeMultiRewardCoins, TotalTimeMultiRewardCoins);
    }

    public static void SaveDefaultLanguage ()
    {
        SavePref (KeyDefaultLanguage, DefaultLanguage);
    }

    public static int GetMissionValue (MissionEnums.MissionId id)
    {
        return MissionId[(int) id];
    }

    public static void SaveMissionValue (MissionEnums.MissionId id, int value)
    {
        MissionId[(int) id] = value;
        SavePref (string.Format (KeyMissionId, ((int) id).ToString ()), value);
    }

    public static int GetMissionLevel (MissionEnums.MissionId id)
    {
        return MissionLevel[(int) id];
    }

    public static void SaveMissionLevel (MissionEnums.MissionId id, int level)
    {
        MissionLevel[(int) id] = level;
        SavePref (string.Format (KeyMissionLevel, ((int) id).ToString ()), level);
    }

    public static void SaveTimeDailyQuest ()
    {
        SavePref (KeyTimeDailyQuest, _LastTimeDailyQuest);
    }

    public static void SaveEquipmentUpgrade (EquipmentEnums.AbilityId id, int level)
    {
        EquipmentUpgrade[(int) id] = level;
        SavePref (string.Format (KeyEquipmentUpgrade, ((int) id).ToString ()), level);
    }

    public static int GetEquipmentUpgrade (EquipmentEnums.AbilityId id)
    {
        return EquipmentUpgrade[(int) id];
    }

    public static void SaveSateWatchAdsForSpin (bool is_completed)
    {
        IsWatchAdsForSpin = is_completed;
        SavePref (KeyWatchAdsForSpin, IsWatchAdsForSpin ? 1 : 0);
    }

    public static void SaveTimeForSpeedUp ()
    {
        SavePref (KeyTimeForSpeedUp, _LastTimeForSpeedUp);
    }

    public static void SaveTimeForMoreCash ()
    {
        SavePref (KeyTimeForMoreCash, _LastTimeForMoreCash);
    }

    public static void SaveUnlockState (int node_level, bool is_unlocked)
    {
        SavePref (string.Format (KeyUnlockGrid, node_level), is_unlocked ? 1 : 0);

        if (node_level > length_unlock_grid)
        {
            for (int i = length_unlock_grid; i < node_level; i++)
            {
                UnlockGrid.Add (0);
            }

            length_unlock_grid = UnlockGrid.Count;

            SavePref (KeyLengthUnlockGrid, length_unlock_grid);
        }

        UnlockGrid[node_level - 1] = is_unlocked ? 1 : 0;
    }

    public static bool GetUnlockState (int node_level)
    {
        return length_unlock_grid >= node_level && UnlockGrid[node_level - 1] == 1;
    }

    [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load ()
    {
        // =============================== Start coins with 0 ================================ //
        Coins = double.Parse (LoadPref (KeyCoins, "0"));

        // =============================== Start with coins unit 0 ================================ //
        CoinUnit = LoadPref (KeyCoinsUnit, 0);

        // =============================== Start food with 0 ================================ //
        Food = double.Parse (LoadPref (KeyFood, "0"));

        // =============================== Start with food unit 0 ================================ //
        FoodUnit = LoadPref (KeyFoodUnit, 0);

        // =============================== Start with level unlocked is 1 ================================ //
        LastLevelUnlocked = LoadPref (KeyLevelUnlocked, 1);

        // =============================== Start with number diamonds is zero ================================ //
        Diamonds = LoadPref (KeyDiamonds, 0);

        // =============================== Start with number of coca is zero ================================ //
        Coca = double.Parse (LoadPref (KeyCoca, "0"));

        // =============================== Get the unit of coca ================================ //
        CocaUnit = LoadPref (KeyCocaUnit, 0);

        // =============================== Start with level is 1 ================================ //
        Level = LoadPref (KeyLevel, 1);

        // =============================== Save Time Speed Up ================================ //
        TotalTimeSpeedUp = LoadPref (KeyTimeTotalSpeedUp, 0);

        // =============================== Save time for more cash ================================ //
        TotalTimeMoreCash = LoadPref (KeyTimeTotalMoreCash, 0);

        // =============================== Save Time Multi Reward Coins ================================ //
        TotalTimeMultiRewardCoins = LoadPref (KeyTotalTimeMultiRewardCoins, 0);

        // =============================== Start Exp With 0 ================================ //
        Exp = LoadPref (KeyExp, 0);

        // =============================== Add Shop Profit item ================================ //
        ShopItemProfitPercent = new List<int> ();

        var total = GameConfig.TotalItem;

        for (int i = 0; i < total; i++)
        {
            ShopItemProfitPercent.Add (LoadPref (string.Format (KeyNumberBuyItem, i.ToString ()), 0));
        }

        // =============================== Add shop upgrade profit item ================================ //

        ShopItemUpgradeProfitPercent = new List<int> ();

        total = GameConfig.TotalItem;

        for (int i = 0; i < total; i++)
        {
            ShopItemUpgradeProfitPercent.Add (LoadPref (string.Format (KeyNumberUpgradeItem, i.ToString ()), 0));
        }

        _LastCocaSaved     = Coca;
        _LastCocaUnitSaved = CocaUnit;

        _LastCoinSaved     = Coins;
        _LastCoinUnitSaved = CoinUnit;

        // =============================== Load Last Time Online ================================ //
        _LastTimeOnline = LoadPref (KeyTimeOnline, Helper.GetUtcTimeString ());

        // =============================== Load Last Time Spin ================================ //
        _LastTimeSpinLucky = LoadPref (KeyTimeSpinLucky, Helper.GetDefaultUTCTimeString ());

        // =============================== Get the number turn spin of player ================================ //
        _LastNumberTurnSpin = LoadPref (KeyNumberTurnSpin, GameConfig.MaxNumberTurnForSpin);

        // =============================== Time Watch Ads For Free Diamonds ================================ //
        _LastTimeWatchAdsForFreeDiamonds = LoadPref (KeyWatchAdsForFreeDiamonds, Helper.GetDefaultUTCTimeString ());

        // =============================== Time appear bonus reward ================================ //
        _LastTimeAppearBonusReward = LoadPref (KeyLastTimeAppearBonusReward, Helper.GetDefaultUTCTimeString ());

        // =============================== Time appear multi reward coins ================================ //
        _LastTimeMultiRewardCoins = LoadPref (KeyLastTimeMultiRewardCoins, Helper.GetDefaultUTCTimeString ());

        // =============================== Time appear new quest ================================ //
        _LastTimeDailyQuest = LoadPref (KeyTimeDailyQuest, Helper.GetDefaultUTCTimeString ());

        // =============================== Time appear bonus currency ================================ //
        _LastTimeAppearBonusCurrency = LoadPref (KeyLastTimeAppearBonusCurrency, Helper.GetDefaultUTCTimeString ());

        // =============================== Time for speed up ================================ //
        _LastTimeForSpeedUp = LoadPref (KeyTimeForSpeedUp, Helper.GetDefaultUTCTimeString ());

        // =============================== Time for watch ads ================================ //
        _LastTimeForMoreCash = LoadPref (KeyTimeForMoreCash, Helper.GetDefaultUTCTimeString ());

        // =============================== Add the tutorials ================================ //

        TutorialId = new List<int> ();

        var size = TutorialEnums.GetSizeTutorial ();

        for (int i = 0; i < size; i++)
        {
            TutorialId.Add (LoadPref (string.Format (KeyTutorialId, i.ToString ()), 0));
        }

        // =============================== Load The default language ================================ //
        DefaultLanguage = LoadPref (KeyDefaultLanguage, LanguageEnums.GetLanguageSupportDefault ());

        // =============================== Add the mission value ================================ //

        MissionId = new List<int> ();

        size = MissionEnums.GetSize ();

        for (int i = 0; i < size; i++)
        {
            MissionId.Add (LoadPref (string.Format (KeyMissionId, i.ToString ()), 0));
        }

        // =============================== Add the mission level ================================ //

        MissionLevel = new List<int> ();

        size = MissionEnums.GetSize ();

        for (int i = 0; i < size; i++)
        {
            MissionLevel.Add (LoadPref (string.Format (KeyMissionLevel, i.ToString ()), 0));
        }

        // =============================== Load the equipment upgrade ================================ //

        EquipmentUpgrade = new List<int> ();

        size = EquipmentEnums.GetLength ();

        for (int i = 0; i < size; i++)
        {
            EquipmentUpgrade.Add (LoadPref (string.Format (KeyEquipmentUpgrade, i.ToString ()), 0));
        }

        // =============================== Load Unlock Node Grids ================================ //

        UnlockGrid = new List<int> ();

        length_unlock_grid = LoadPref (KeyLengthUnlockGrid, 0);

        for (int i = 0; i < length_unlock_grid; i++)
        {
            UnlockGrid.Add (LoadPref (string.Format (KeyUnlockGrid, i), 0));
        }

        // =============================== Get the last the state watch ads for spin ================================ //

        IsWatchAdsForSpin = LoadPref (KeyWatchAdsForSpin, 0) == 1;
    }

    #region Player Pref

    private static void SavePref (string key, string value)
    {
        PlayerPrefs.SetString (key, value);
    }

    private static void SavePref (string key, int value)
    {
        PlayerPrefs.SetInt (key, value);
    }

    private static void SavePref (string key, float value)
    {
        PlayerPrefs.SetFloat (key, value);
    }

    private static string LoadPref (string key, string default_value)
    {
        return PlayerPrefs.GetString (key, default_value);
    }

    private static int LoadPref (string key, int default_value)
    {
        return PlayerPrefs.GetInt (key, default_value);
    }

    private static float LoadPref (string key, float default_value)
    {
        return PlayerPrefs.GetFloat (key, default_value);
    }

    #endregion

    public static void Clear ()
    {
        PlayerPrefs.DeleteAll ();
    }
}