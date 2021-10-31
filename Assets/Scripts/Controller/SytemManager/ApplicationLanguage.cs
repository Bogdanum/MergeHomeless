using System.Collections;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine;

public class ApplicationLanguage : Singleton<ApplicationLanguage>
{
    #region Variables

    private LanguageManager share_instance;

    [Header ("Config")] [SerializeField] private int size_text_talk_slogan;

    [Header ("Notification")] [SerializeField]
    private int size_title_notification;

    [SerializeField] private int size_description_notification;

    [Header ("Text")] public static string Text_description_no_more_ads;
    public static                   string Text_description_no_more_park;
    public static                   string Text_description_discount_shop;
    public static                   string Text_description_earning_shop;
    public static                   string Text_description_earning_each_sec;

    public static string Text_label_language;
    public static string Text_label_music;
    public static string Text_label_restore_purchase;
    public static string Text_label_settings;
    public static string Text_label_sound;

    public static string Text_description_welcome_racer;
    public static string Text_description_racing;

    public static string Text_description_racing_II;
    public static string Text_description_go_go;
    public static string Text_description_tutorial_merge;
    public static string Text_description_tutorial_earning;
    public static string Text_description_tutorial_upgrade;
    public static string Text_description_tutorial_start_game;
    public static string Text_description_tutorial_speed_up;

    public static string Text_description_you_earned;
    public static string Text_label_offline_earning;

    public static string Text_label_free;
    public static string Text_label_tap_to_close;
    public static string Text_label_wheel_lucky;

    public static string Text_label_speed;
    public static string Text_label_unlocked;
    public static string Text_label_earning;
    public static string Text_label_bonus_reward;

    public static string Text_description_add_seconds;
    public static string Text_description_speed_in;
    public static string Text_label_speed_up;

    public static string Text_label_shop,
                         Text_label_item,
                         Text_label_other;

    public static string Text_label_exchange,
                         Text_label_currency_exchange,
                         Text_label_min,
                         Text_label_max;

    public static string Text_label_diamond_shop;

    public static string Text_label_owned;

    public static string Text_description_mission_merge,
                         Text_description_mission_get_bonus,
                         Text_description_mission_upgrade_item,
                         Text_description_mission_buy_item,
                         Text_description_mission_own_item;

    public static string Text_description_mission_not_complete;

    public static string Text_description_not_enough_diamonds,
                         Text_description_not_enough_cash,
                         Text_description_not_enough_coca,
                         Text_description_locked;

    public static string Text_completed_mission;

    public static string Text_daily_quest_will_back,
                         Text_out_of_mission;

    public static string Text_label_daily_quest,
                         Text_label_normal_quest;

    // =============================== Version 1.0 ================================ //

    public static string Text_description_revenue_from_ads,
                         Text_description_revenue_from_diamonds;

    public static string Text_description_get_revenue_for_times;

    public static string Text_description_will_available_in,
                         Text_description_thank_you,
                         Text_label_back,
                         Text_label_claim,
                         Text_label_buy,
                         Text_description_reach_level,
                         Text_description_rate_me,
                         Text_description_require_level,
                         Text_description_tap_to_open,
                         Text_description_tap_open_chest,
                         Text_label_follow,
                         Text_label_share,
                         Text_label_rate_now,
                         Text_label_update_now,
                         Text_label_your_income,
                         Text_label_upgrade,
                         Text_label_invite_friends,
                         Text_label_handful_of_coins,
                         Text_label_level_up,
                         Text_label_pile_of_coins,
                         Text_label_next_in,
                         Text_description_more_come_soon,
                         Text_description_connect_facebook_failed,
                         Text_description_failed_to_sync_cloud,
                         Text_description_follow_on_twitter,
                         Text_description_increase_exp,
                         Text_description_here_your_reward,
                         Text_description_invite_friend_get_profit_boost,
                         Text_description_mission_like_facebook;

    public static string Text_label_level_up_upgrade;

    public static string Text_label_maxed, Text_label_statistics;

    public static string Text_label_event_end_in,
                         Text_label_give_us_five_stars,
                         Text_label_discount_value,
                         Text_label_daily_event_available,
                         Text_label_discount,
                         Text_label_profit_speed,
                         Text_label_sale_off,
                         Text_label_notification,
                         Text_label_later,
                         Text_label_let_spin,
                         Text_description_you_are_claimed,
                         Text_description_we_are_collected,
                         Text_descriptuon_profit_speed_x,
                         Text_label_booster,
                         Text_label_battery_saver,
                         Text_label_come_back_later_for_spin,
                         Text_label_completed,
                         Text_description_spin_more_and_win,
                         Text_description_ufo_gift,
                         Text_description_time_jump,
                         Text_description_follow_and_talk_us,
                         Text_label_spin_and_win,
                         Text_label_watch_to_spin,
                         Text_label_watch_now,
                         Text_description_lv,
                         Text_label_yeah;

    public static string Text_description_time_jump_1_day,
                         Text_description_time_jump_14_day,
                         Text_description_time_jump_30_day,
                         Text_description_time_jump_7_day;

    public static string Text_label_get;

    public static string Text_label_gimme_that;
    public static string Text_label_get_x_profit;

    public static string Text_description_watch_to_get_free;

    public static string Text_description_equip_discount_buy,
                         Text_description_equip_discount_upgrade,
                         Text_description_equip_earning;

    public static string Text_description_mission_tap_item,
                         Text_description_mission_tap_box;

    public static string Text_label_rate_me;

    public static string Text_description_update_ios, Text_description_update_android;

    public static string[] Text_label_item_name;

    public static string[] Text_title_notification;
    public static string[] Text_description_notification;

    // =============================== AskerGame ================================ //

    public static string Text_inp_window_button_ok;
    public static string Text_inp_window_error;
    public static string Text_inp_window_forb;
    public static string Text_inp_window_header;

    public static string Text_lb_panel_loadinglb;
    public static string Text_lb_panel_show_around_players;
    public static string Text_lb_panel_show_all_players;
    public static string Text_lb_panel_header;

    public static string Text_lb_access_panel_loadinglb;
    public static string Text_lb_access_panel_show_around_players;
    public static string Text_lb_access_panel_header;

    public static string Text_bonus_gems;

    #endregion


    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitConfig ();
        InitEvent ();
        InitLoad ();
    }

    #endregion

    #region Controller

    private void InitConfig ()
    {
        share_instance = LanguageManager.InstanceAwake ();

        share_instance.LoadAvailableCultures ();
    }

    private void InitLoad ()
    {
        share_instance.ChangeLanguage (PlayerData.DefaultLanguage);
    }

    private void InitEvent ()
    {
        share_instance.OnChangeLanguage += OnChangeLanguage;
    }

    private void OnChangeLanguage (LanguageManager language)
    {
        LoadLanguage ();
    }

    #endregion

    #region Action

    public void ChangeLanguage (LanguageEnums.LanguageId id)
    {
        ChangeLanguage (LanguageEnums.GetLanguageKey (id));
    }

    public void ChangeLanguage (string language_Code)
    {
        share_instance.ChangeLanguage (language_Code);
        PlayerData.DefaultLanguage = language_Code;
        PlayerData.SaveDefaultLanguage ();
    }

    private void LoadLanguage ()
    {
        Text_description_no_more_ads      = share_instance.GetTextValue ("Text_description_no_more_ads");
        Text_description_no_more_park     = share_instance.GetTextValue ("Text_description_no_more_park");
        Text_description_earning_shop     = share_instance.GetTextValue ("Text_description_earning_shop");
        Text_description_discount_shop    = share_instance.GetTextValue ("Text_description_discount_shop");
        Text_description_earning_each_sec = share_instance.GetTextValue ("Text_description_earning_each_sec");

        Text_label_language         = share_instance.GetTextValue ("Text_label_language");
        Text_label_music            = share_instance.GetTextValue ("Text_label_music");
        Text_label_restore_purchase = share_instance.GetTextValue ("Text_label_restore_purchase");
        Text_label_settings         = share_instance.GetTextValue ("Text_label_settings");
        Text_label_sound            = share_instance.GetTextValue ("Text_label_sound");

        Text_description_welcome_racer       = share_instance.GetTextValue ("Text_description_welcome_racer");
        Text_description_racing              = share_instance.GetTextValue ("Text_description_racing");
        Text_description_racing_II           = share_instance.GetTextValue ("Text_description_racing_II");
        Text_description_go_go               = share_instance.GetTextValue ("Text_description_go_go");
        Text_description_tutorial_merge      = share_instance.GetTextValue ("Text_description_tutorial_merge");
        Text_description_tutorial_earning    = share_instance.GetTextValue ("Text_description_tutorial_earning");
        Text_description_tutorial_upgrade    = share_instance.GetTextValue ("Text_description_tutorial_upgrade");
        Text_description_tutorial_start_game = share_instance.GetTextValue ("Text_description_tutorial_start_game");
        Text_description_tutorial_speed_up   = share_instance.GetTextValue ("Text_description_tutorial_speed_up");

        Text_description_you_earned = share_instance.GetTextValue ("Text_description_you_earned");
        Text_label_offline_earning  = share_instance.GetTextValue ("Text_label_offline_earning");

        Text_label_free         = share_instance.GetTextValue ("Text_label_free");
        Text_label_tap_to_close = share_instance.GetTextValue ("Text_label_tap_to_close");
        Text_label_wheel_lucky  = share_instance.GetTextValue ("Text_label_wheel_lucky");

        Text_label_speed        = share_instance.GetTextValue ("Text_label_speed");
        Text_label_unlocked     = share_instance.GetTextValue ("Text_label_unlocked");
        Text_label_earning      = share_instance.GetTextValue ("Text_label_earning");
        Text_label_bonus_reward = share_instance.GetTextValue ("Text_label_bonus_reward");

        Text_description_add_seconds = share_instance.GetTextValue ("Text_description_add_seconds");
        Text_description_speed_in    = share_instance.GetTextValue ("Text_description_speed_in");
        Text_label_speed_up          = share_instance.GetTextValue ("Text_label_speed_up");

        Text_label_shop  = share_instance.GetTextValue ("Text_label_shop");
        Text_label_item  = share_instance.GetTextValue ("Text_label_item");
        Text_label_other = share_instance.GetTextValue ("Text_label_other");

        Text_label_exchange          = share_instance.GetTextValue ("Text_label_exchange");
        Text_label_currency_exchange = share_instance.GetTextValue ("Text_label_currency_exchange");
        Text_label_min               = share_instance.GetTextValue ("Text_label_min");
        Text_label_max               = share_instance.GetTextValue ("Text_label_max");

        Text_label_diamond_shop = share_instance.GetTextValue ("Text_label_diamond_shop");

        Text_label_owned = share_instance.GetTextValue ("Text_label_owned");

        Text_description_mission_merge        = share_instance.GetTextValue ("Text_description_mission_merge");
        Text_description_mission_get_bonus    = share_instance.GetTextValue ("Text_description_mission_get_bonus");
        Text_description_mission_own_item     = share_instance.GetTextValue ("Text_description_mission_own_item");
        Text_description_mission_upgrade_item = share_instance.GetTextValue ("Text_description_mission_upgrade_item");
        Text_description_mission_buy_item     = share_instance.GetTextValue ("Text_description_mission_buy_item");

        Text_description_mission_not_complete = share_instance.GetTextValue ("Text_description_mission_not_complete");

        Text_description_not_enough_diamonds = share_instance.GetTextValue ("Text_description_not_enough_diamonds");
        Text_description_not_enough_cash     = share_instance.GetTextValue ("Text_description_not_enough_cash");
        Text_description_not_enough_coca     = share_instance.GetTextValue ("Text_description_not_enough_coca");
        Text_description_locked              = share_instance.GetTextValue ("Text_description_locked");

        Text_completed_mission = share_instance.GetTextValue ("Text_completed_mission");

        Text_daily_quest_will_back = share_instance.GetTextValue ("Text_daily_quest_will_back");
        Text_out_of_mission        = share_instance.GetTextValue ("Text_out_of_mission");

        Text_label_daily_quest  = share_instance.GetTextValue ("Text_label_daily_quest");
        Text_label_normal_quest = share_instance.GetTextValue ("Text_label_normal_quest");

        Text_description_revenue_from_ads      = share_instance.GetTextValue ("Text_description_revenue_from_ads");
        Text_description_revenue_from_diamonds = share_instance.GetTextValue ("Text_description_revenue_from_diamonds");

        Text_description_get_revenue_for_times = share_instance.GetTextValue ("Text_description_get_revenue_for_times");

        Text_description_will_available_in              = share_instance.GetTextValue ("Text_description_will_available_in");
        Text_description_thank_you                      = share_instance.GetTextValue ("Text_description_thank_you");
        Text_label_back                                 = share_instance.GetTextValue ("Text_label_back");
        Text_label_claim                                = share_instance.GetTextValue ("Text_label_claim");
        Text_label_buy                                  = share_instance.GetTextValue ("Text_label_buy");
        Text_description_reach_level                    = share_instance.GetTextValue ("Text_description_reach_level");
        Text_description_rate_me                        = share_instance.GetTextValue ("Text_description_rate_me");
        Text_description_require_level                  = share_instance.GetTextValue ("Text_description_require_level");
        Text_description_tap_to_open                    = share_instance.GetTextValue ("Text_description_tap_to_open");
        Text_description_tap_open_chest                 = share_instance.GetTextValue ("Text_description_tap_open_chest");
        Text_label_follow                               = share_instance.GetTextValue ("Text_label_follow");
        Text_label_share                                = share_instance.GetTextValue ("Text_label_share");
        Text_label_rate_now                             = share_instance.GetTextValue ("Text_label_rate_now");
        Text_label_update_now                           = share_instance.GetTextValue ("Text_label_update_now");
        Text_label_your_income                          = share_instance.GetTextValue ("Text_label_your_income");
        Text_label_upgrade                              = share_instance.GetTextValue ("Text_label_upgrade");
        Text_label_invite_friends                       = share_instance.GetTextValue ("Text_label_invite_friends");
        Text_label_handful_of_coins                     = share_instance.GetTextValue ("Text_label_handful_of_coins");
        Text_label_level_up                             = share_instance.GetTextValue ("Text_label_level_up");
        Text_label_pile_of_coins                        = share_instance.GetTextValue ("Text_label_pile_of_coins");
        Text_label_next_in                              = share_instance.GetTextValue ("Text_label_next_in");
        Text_description_more_come_soon                 = share_instance.GetTextValue ("Text_description_more_come_soon");
        Text_description_connect_facebook_failed        = share_instance.GetTextValue ("Text_description_connect_facebook_failed");
        Text_description_failed_to_sync_cloud           = share_instance.GetTextValue ("Text_description_failed_to_sync_cloud");
        Text_description_follow_on_twitter              = share_instance.GetTextValue ("Text_description_follow_on_twitter");
        Text_description_increase_exp                   = share_instance.GetTextValue ("Text_description_increase_exp");
        Text_description_here_your_reward               = share_instance.GetTextValue ("Text_description_here_your_reward");
        Text_description_invite_friend_get_profit_boost = share_instance.GetTextValue ("Text_description_invite_friend_get_profit_boost");
        Text_description_mission_like_facebook          = share_instance.GetTextValue ("Text_description_mission_like_facebook");
       
        Text_label_maxed      = share_instance.GetTextValue ("Text_label_maxed");
        Text_label_statistics = share_instance.GetTextValue ("Text_label_statistics");

        Text_label_event_end_in             = share_instance.GetTextValue ("Text_label_event_end_in");
        Text_label_give_us_five_stars       = share_instance.GetTextValue ("Text_label_give_us_five_stars");
        Text_label_discount_value           = share_instance.GetTextValue ("Text_label_discount_value");
        Text_label_daily_event_available    = share_instance.GetTextValue ("Text_label_daily_event_available");
        Text_label_discount                 = share_instance.GetTextValue ("Text_label_discount");
        Text_label_profit_speed             = share_instance.GetTextValue ("Text_label_profit_speed");
        Text_label_sale_off                 = share_instance.GetTextValue ("Text_label_sale_off");
        Text_label_notification             = share_instance.GetTextValue ("Text_label_notification");
        Text_label_later                    = share_instance.GetTextValue ("Text_label_later");
        Text_label_let_spin                 = share_instance.GetTextValue ("Text_label_let_spin");
        Text_description_you_are_claimed    = share_instance.GetTextValue ("Text_description_you_are_claimed");
        Text_description_we_are_collected   = share_instance.GetTextValue ("Text_description_we_are_collected");
        Text_descriptuon_profit_speed_x     = share_instance.GetTextValue ("Text_descriptuon_profit_speed_x");
        Text_label_booster                  = share_instance.GetTextValue ("Text_label_booster");
        Text_label_battery_saver            = share_instance.GetTextValue ("Text_label_battery_saver");
        Text_label_come_back_later_for_spin = share_instance.GetTextValue ("Text_label_come_back_later_for_spin");
        Text_label_completed                = share_instance.GetTextValue ("Text_label_completed");
        Text_description_spin_more_and_win  = share_instance.GetTextValue ("Text_description_spin_more_and_win");
        Text_description_ufo_gift           = share_instance.GetTextValue ("Text_description_ufo_gift");
        Text_description_time_jump          = share_instance.GetTextValue ("Text_description_time_jump");
        Text_description_follow_and_talk_us = share_instance.GetTextValue ("Text_description_follow_and_talk_us");
        Text_label_spin_and_win             = share_instance.GetTextValue ("Text_label_spin_and_win");
        Text_label_watch_to_spin            = share_instance.GetTextValue ("Text_label_watch_to_spin");
        Text_label_watch_now                = share_instance.GetTextValue ("Text_label_watch_now");
        Text_description_lv                 = share_instance.GetTextValue ("Text_description_lv");
        Text_label_yeah                     = share_instance.GetTextValue ("Text_label_yeah");

        Text_description_time_jump_1_day  = share_instance.GetTextValue ("Text_description_time_jump_1_day");
        Text_description_time_jump_14_day = share_instance.GetTextValue ("Text_description_time_jump_14_day");
        Text_description_time_jump_30_day = share_instance.GetTextValue ("Text_description_time_jump_30_day");
        Text_description_time_jump_7_day  = share_instance.GetTextValue ("Text_description_time_jump_7_day");

        Text_label_get_x_profit = share_instance.GetTextValue ("Text_label_get_x_profit");

        Text_description_watch_to_get_free = share_instance.GetTextValue ("Text_description_watch_to_get_free");

        Text_label_get = share_instance.GetTextValue ("Text_label_get");

        Text_label_gimme_that = share_instance.GetTextValue ("Text_label_gimme_that");

        Text_description_equip_discount_buy     = share_instance.GetTextValue ("Text_description_equip_discount_buy");
        Text_description_equip_discount_upgrade = share_instance.GetTextValue ("Text_description_equip_discount_upgrade");
        Text_description_equip_earning          = share_instance.GetTextValue ("Text_description_equip_earning");

        Text_description_mission_tap_item = share_instance.GetTextValue ("Text_description_mission_tap_item");
        Text_description_mission_tap_box  = share_instance.GetTextValue ("Text_description_mission_tap_box");

        Text_label_level_up_upgrade = share_instance.GetTextValue ("Text_label_level_up_upgrade");

        Text_label_rate_me = share_instance.GetTextValue ("Text_label_rate_me");

        Text_description_update_ios     = share_instance.GetTextValue ("Text_description_update_ios");
        Text_description_update_android = share_instance.GetTextValue ("Text_description_update_android");

        // start ---------------------------- Asker Game ----------------------------- //

        Text_inp_window_button_ok                       =       share_instance.GetTextValue ("Text_inp_window_button_ok");
        Text_inp_window_error                           =       share_instance.GetTextValue ("Text_inp_window_error");
        Text_inp_window_forb                            =       share_instance.GetTextValue ("Text_inp_window_forb");
        Text_inp_window_header                          =       share_instance.GetTextValue ("Text_inp_window_header");
                                                                                                                                       
        Text_lb_panel_loadinglb                         =       share_instance.GetTextValue ("Text_lb_panel_loadinglb");
        Text_lb_panel_show_around_players               =       share_instance.GetTextValue ("Text_lb_panel_show_around_players");
        Text_lb_panel_show_all_players                  =       share_instance.GetTextValue ("Text_lb_panel_show_all_players");
        Text_lb_panel_header                            =       share_instance.GetTextValue ("Text_lb_panel_header");
                                                                                                                                       
        Text_lb_access_panel_loadinglb                  =       share_instance.GetTextValue ("Text_lb_access_panel_loadinglb");
        Text_lb_access_panel_show_around_players        =       share_instance.GetTextValue ("Text_lb_panel_show_around_players");
        Text_lb_access_panel_header                     =       share_instance.GetTextValue ("Text_lb_panel_header");

        Text_bonus_gems                                 =       share_instance.GetTextValue ("Text_bonus_gems");
        // end ---------------------------- Asker Game ----------------------------- //

        var size = GameConfig.TotalItem;

        Text_label_item_name = new string[size];

        for (int i = 1; i <= size; i++)
        {
            Text_label_item_name[i - 1] = share_instance.GetTextValue (string.Format ("Text_label_item_name_{0}", i));
        }

        Text_title_notification = new string[size_title_notification];

        for (int i = 1; i <= size_title_notification; i++)
        {
            Text_title_notification[i - 1] = share_instance.GetTextValue (string.Format ("Text_title_notification_{0}", i));
        }

        Text_description_notification = new string[size_description_notification];

        for (int i = 1; i <= size_description_notification; i++)
        {
            Text_description_notification[i - 1] = share_instance.GetTextValue (string.Format ("Text_description_notification_{0}", i));
        }

        this.PostActionEvent (ActionEnums.ActionID.RefreshLanguage);
    }

    #endregion

    #region Helper

    public int GetSizeLanguage ()
    {
        return share_instance.NumberOfSupportedLanguages;
    }
  
    public List<SmartCultureInfo> GetSupportLanguage ()
    {
        return share_instance.GetSupportedLanguages ();
    }

    public static string GetItemName (int level)
    {
        return Text_label_item_name[level - 1];
    }

    public static string GetRandomTitleNotification ()
    {
        return Text_title_notification.Length > 0 ? Text_title_notification[Random.Range (0, Text_title_notification.Length)] : string.Empty;
    }

    public static string GetRandomDescriptionNotification ()
    {
        return Text_description_notification.Length > 0 ? Text_description_notification[Random.Range (0, Text_description_notification.Length)] : string.Empty;
    }

    #endregion
}