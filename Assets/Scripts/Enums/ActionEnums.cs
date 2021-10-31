public static class ActionEnums
{
    public enum ActionID
    {
        None,
        SpeedUp,
        UpdateEarningCoins,
        RefreshUICoins,
        RefreshUICoca,
        RefreshUIDiamonds,
        RefreshRunLineOffer,
        AdLoadingCompleted,
        UpdateDiscountBuy,
        UpdateIncreaseRewardCoins,
        SetStateInteractUI,
        SetStateInteractGame,
        RefreshUIEquipments,
        RefreshLanguage,
        RefreshLayoutMission,
        RefreshUICompleteMission,
        UpdateMissionValue,
        RefreshUpgradeItems,
        RefreshEquipmentData,
        MoreCash,
    }

    private static readonly string[] ActionString =
    {
        "None",
        "speed_up",
        "update_earning_coins",
        "refresh_ui_coins",
        "refresh_ui_coca",
        "refresh_ui_diamonds",
        "refresh_run_line_offer",
        "ad_loading_completed",
        "update_discount_buy",
        "update_increase_reward_coins",
        "set_state_interact_ui",
        "set_state_interact_Game",
        "refresh_ui_equipments",
        "refresh_language",
        "refresh_layout_mission",
        "refresh_ui_complete_mission",
        "update_mission_value",
        "refresh_upgrade_cat",
        "refresh_equipment_data",
        "more_cash"
    };

    public static string GetActionString (ActionID actionId)
    {
        return ActionString[(int) actionId];
    }
}