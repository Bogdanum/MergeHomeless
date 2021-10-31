using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIItemView : MonoBehaviour
{
    [Header ("Items Shop")] [SerializeField]
    private Text label_shop;

    [SerializeField] private Text label_upgrade,
                                  label_buy,
                                  label_speed,
                                  label_earning;

    [Header ("Items")] [SerializeField] private Button interact_upgrade_items,
                                                       interact_buy_items;

    [SerializeField] private Image currency_icon,
                                   earning_process,
                                   speed_process;

    [SerializeField] private Text label_item_name,
                                  item_level,
                                  label_earning_items,
                                  label_buy_discount,
                                  label_upgrade_discount,
                                  label_upgrade_level;

    [SerializeField] private Text label_price_buy,
                                  label_price_upgrade;

    [SerializeField] private Transform banner_discount_buy,
                                       banner_discount_upgrade;

    [SerializeField] private Text label_total_items;

    [Header ("Renderer")] [SerializeField] private Image sprite_renderer;

    private CurrencyEnums.CurrencyId currency_id;

    private double price;
    private int    price_unit;

    private double price_upgrade;
    private int    price_upgrade_unit;

    private double price_earning;
    private int    price_earning_unit;

    private double price_data;
    private int    price_unit_data;

    private double price_upgrade_data;
    private int    price_unit_upgrade_data;

    private float profit_per_sec;
    private int   profit_per_sec_unit;

    private float profit_per_upgrade_coefficient;

    private float price_buy_coefficient;
    private float price_upgrade_coefficient;

    private double discount_prices;
    private double discount_upgrade_prices;

    private float earning_value_percents;
    private float speed_value_percents;

    private double last_price_notice;
    private int    last_price_unit_notice;

    private int level;

    private string item_name;

    private bool is_unlock,
                 is_can_buy,
                 is_use_diamond;

    private int price_diamonds;

    private int buy_from_level;

    private int number_buy_times,
                number_upgrade_times;

    private int total_items;

    private bool is_enabled;
    private bool is_enabled_notice;

    private int last_level_buying;

    #region Action

    public UIItemView InitParameters (CurrencyEnums.CurrencyId id, int _level)
    {
        currency_id          = id;
        currency_icon.sprite = GameData.Instance.CurrencyIcon.GetIconCurrency (id);

        level = _level;

        total_items = GameConfig.TotalItem;

        var data = GameData.Instance.ItemDataGroup.GetDataItemWithLevel (level);

        number_upgrade_times = PlayerData.GetNumberUpgradeItemProfitCoefficient (level);

        price_data      = data.Prices;
        price_unit_data = data.PricesUnit;

        price_upgrade_data      = data.PricesUpgrade;
        price_unit_upgrade_data = data.PricesUnitUpgrade;

        profit_per_sec      = data.ProfitPerSec;
        profit_per_sec_unit = data.ProfitPerSecUnit;

        price_buy_coefficient     = data.PricesCoefficient;
        price_upgrade_coefficient = data.PriceUpgradeCoefficient;

        profit_per_upgrade_coefficient = data.ProfitPerUpgradeCoefficient;

        buy_from_level = data.BuyFromLevel;

        var shop = GameData.Instance.ItemShopDetail.GetItem (level);

        earning_value_percents = shop.Earning;
        speed_value_percents   = shop.Speed;


        price_diamonds = GameData.Instance.ItemDiamondPrices.GetPrice (level);

        RefreshData ();

        return this;
    }

    private void RefreshData ()
    {
        number_buy_times = PlayerData.GetNumberBuyItemProfitCoefficient (level);

        var UpgradeData    = GameData.Instance.EquipmentData.GetData (EquipmentEnums.AbilityId.DiscountBuy);
        var discount_data  = GameData.Instance.EquipmentData.GetData (EquipmentEnums.AbilityId.DiscountUpgradeItems);
        var level_upgrade  = PlayerData.GetEquipmentUpgrade (EquipmentEnums.AbilityId.DiscountBuy);
        var level_discount = PlayerData.GetEquipmentUpgrade (EquipmentEnums.AbilityId.DiscountUpgradeItems);

        discount_prices         = Math.Pow (UpgradeData.UpgradeCoefficient, level_upgrade) / 100f;
        discount_upgrade_prices = Math.Pow (discount_data.UpgradeCoefficient, level_discount) / 100f;

        if (level_upgrade == 0)
        {
            discount_prices = 0;
        }

        if (level_discount == 0)
        {
            discount_upgrade_prices = 0;
        }

        RefreshPriceBuy ();
        RefreshPriceUpgrade ();
        RefreshEarning ();
    }

    public void RefreshEnabled ()
    {
        is_enabled = true;

        RefreshData ();
        RefreshUI (level <= PlayerData.LastLevelUnlocked, level <= PlayerData.LastLevelUnlocked, PlayerData.LastLevelUnlocked - 2 < level);
    }

    public UIItemView SetId (Sprite icon)
    {
        item_name              = ApplicationLanguage.GetItemName (level);
        sprite_renderer.sprite = icon;

        return this;
    }

    public void RefreshUI (bool _is_unlock, bool _is_can_buy, bool _is_use_diamond)
    {
        is_unlock      = _is_unlock;
        is_can_buy     = _is_can_buy;
        is_use_diamond = _is_use_diamond;

        item_level.text = level.ToString ();

        if (!_is_unlock)
        {
            earning_process.fillAmount = 0;
            speed_process.fillAmount   = 0;

            sprite_renderer.color = Color.black;

            label_item_name.text     = "????";
            label_earning_items.text = "????";
            label_upgrade_level.text = "????";

            label_price_buy.text     = "????";
            label_price_upgrade.text = "????";

            interact_buy_items.interactable     = false;
            interact_upgrade_items.interactable = false;

            if (banner_discount_upgrade.gameObject.activeSelf) banner_discount_upgrade.gameObject.SetActive (false);
            if (banner_discount_buy.gameObject.activeSelf) banner_discount_buy.gameObject.SetActive (false);
        }
        else
        {
            earning_process.fillAmount = earning_value_percents;
            speed_process.fillAmount   = speed_value_percents;

            label_item_name.text = item_name;

            if (discount_prices > 0)
            {
                label_buy_discount.text = string.Format ("-{0}%", (discount_prices * 100).ToString ("0.00"));
                if (banner_discount_buy.gameObject.activeSelf == false) banner_discount_buy.gameObject.SetActive (true);
            }
            else
            {
                if (banner_discount_buy.gameObject.activeSelf == true) banner_discount_buy.gameObject.SetActive (false);
            }

            if (discount_upgrade_prices > 0)
            {
                label_upgrade_discount.text = string.Format ("-{0}%", (discount_upgrade_prices * 100).ToString ("0.00"));
                if (banner_discount_upgrade.gameObject.activeSelf == false) banner_discount_upgrade.gameObject.SetActive (true);
            }
            else
            {
                if (banner_discount_upgrade.gameObject.activeSelf == true) banner_discount_upgrade.gameObject.SetActive (false);
            }

            interact_buy_items.interactable     = _is_can_buy;
            interact_upgrade_items.interactable = _is_can_buy;


            if (is_use_diamond)
            {
                label_price_buy.text = price_diamonds.ToString ();
                currency_icon.sprite = GameData.Instance.CurrencyIcon.GetIconCurrency (CurrencyEnums.CurrencyId.Diamonds);
            }
            else
            {
                label_price_buy.text = ApplicationManager.Instance.AppendFromCashUnit (price, price_unit);
                currency_icon.sprite = GameData.Instance.CurrencyIcon.GetIconCurrency (currency_id);
            }

            label_price_upgrade.text = ApplicationManager.Instance.AppendFromCashUnit (price_upgrade, price_upgrade_unit);

            label_item_name.text     = item_name;
            label_earning_items.text = string.Format (ApplicationLanguage.Text_description_earning_each_sec, ApplicationManager.Instance.AppendFromCashUnit (price_earning, price_earning_unit, 2));
            label_upgrade_level.text = string.Format (ApplicationLanguage.Text_description_lv, number_upgrade_times.ToString ());

            RefreshInteract ();

            sprite_renderer.color = Color.white;
        }

        label_total_items.text = string.Format ("{0}/{1}", level.ToString (), total_items.ToString ());
    }

    private void RefreshPriceBuy ()
    {
        price = price_data - price_data * discount_prices;
        price = price * Math.Pow (price_buy_coefficient, number_buy_times);

        price_unit = price_unit_data;

        Helper.FixUnit (ref price, ref price_unit);
    }

    private void RefreshPriceUpgrade ()
    {
        price_upgrade      = price_upgrade_data - price_upgrade_data * discount_upgrade_prices;
        price_upgrade      = price_upgrade * Math.Pow (price_upgrade_coefficient, number_upgrade_times);
        price_upgrade_unit = price_unit_upgrade_data;

        Helper.FixUnit (ref price_upgrade, ref price_upgrade_unit);
    }

    private void RefreshEarning ()
    {
        price_earning      = profit_per_sec * Math.Pow (profit_per_upgrade_coefficient, number_upgrade_times);
        price_earning_unit = profit_per_sec_unit;

        Helper.FixUnit (ref price_earning, ref price_earning_unit);
    }

    public void RefreshNotice ()
    {
        last_price_notice      = price_upgrade;
        last_price_unit_notice = price_upgrade_unit;

        if (Helper.IsSmaller (PlayerData.Coins, PlayerData.CoinUnit, last_price_notice, last_price_unit_notice))
        {
            is_enabled_notice = true;
        }
        else
        {
            if (Random.Range (0.0f, 1f) < 0.5f)
            {
                is_enabled_notice = true;
            }
        }
    }

    public void RefreshInteract ()
    {
        if (!is_unlock)
        {
            interact_buy_items.interactable     = false;
            interact_upgrade_items.interactable = false;

            return;
        }

        if (is_use_diamond)
        {
            interact_buy_items.interactable = PlayerData.Diamonds >= price_diamonds;
        }
        else
        {
            switch (currency_id)
            {
                case CurrencyEnums.CurrencyId.Coca:

                    interact_buy_items.interactable = Helper.IsEqualOrSmaller (PlayerData.Coca, PlayerData.CocaUnit, price, price_unit);

                    break;
                case CurrencyEnums.CurrencyId.Cash:

                    interact_buy_items.interactable = Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, price, price_unit);

                    break;
                case CurrencyEnums.CurrencyId.Diamonds:

                    interact_buy_items.interactable = PlayerData.Diamonds >= price_diamonds;

                    break;
            }
        }

        interact_upgrade_items.interactable = Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, price_upgrade, price_upgrade_unit);
    }

    public void RefreshLanguage ()
    {
        label_shop.text    = ApplicationLanguage.Text_label_shop;
        label_upgrade.text = ApplicationLanguage.Text_label_upgrade;
        label_buy.text     = ApplicationLanguage.Text_label_buy;
        label_speed.text   = ApplicationLanguage.Text_label_speed;
        label_earning.text = ApplicationLanguage.Text_label_earning;
        if (level <= PlayerData.LastLevelUnlocked) label_item_name.text = ApplicationLanguage.GetItemName(level);
        else label_item_name.text = "????";
    }

    public void RegisterRefreshUi ()
    {
        // =============================== Register ui fresh coins ================================ //

        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUICoins, OnRefreshUiCoins);
    }

    public void UnRegisterRefreshUi ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUICoins, OnRefreshUiCoins);
    }

    public void SetState (bool is_enable)
    {
        is_enabled = is_enable;
    }

    #endregion

    #region Callbacks

    private void OnRefreshUiCoins (object obj)
    {
        if (is_enabled)
        {
            RefreshInteract ();
        }

        if (is_enabled_notice)
        {
            if (!Helper.IsSmaller (PlayerData.Coins, PlayerData.CoinUnit, last_price_notice, last_price_unit_notice))
            {
                is_enabled_notice = false;

                UIGameManager.Instance.SetStateNoticeShop (true);
            }
        }
    }

    private void OnUpgradeCompleted ()
    {
        OnIncreaseNumberTimeUpgrade ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUpgradeItems);

        this.PostMissionEvent (MissionEnums.MissionId.UpgradeItem);

        if (Random.Range (0.00f, 1.00f) < 0.3f)
        {
            this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);
        }

        GameManager.Instance.FxShakeScale (sprite_renderer.transform);
        GameActionManager.Instance.InstanceFxLevelUp (sprite_renderer.transform.position);

        this.PlayAudioSound (AudioEnums.SoundId.UpgradeLevelUp);
    }

    private void OnIncreaseNumberTimeUpgrade ()
    {
        number_upgrade_times++;

        price_upgrade = price_upgrade * price_upgrade_coefficient;

        Helper.FixUnit (ref price_upgrade, ref price_upgrade_unit);

        PlayerData.SaveItemUpgradeProfitCoefficient (level, number_upgrade_times);

        RefreshEarning ();
        RefreshUI (level <= PlayerData.LastLevelUnlocked, level <= PlayerData.LastLevelUnlocked, PlayerData.LastLevelUnlocked - 2 < level);
    }

    private void OnBuyCompleted ()
    {
        OnIncreaseNumberTimeBuy ();

        GameManager.Instance.SetBoxReward (level);

        this.PostMissionEvent (MissionEnums.MissionId.BuyItem);

        if (currency_id == CurrencyEnums.CurrencyId.Cash)
            last_level_buying = level;
    }

    private void OnIncreaseNumberTimeBuy ()
    {
        number_buy_times++;

        price = price * price_buy_coefficient;

        Helper.FixUnit (ref price, ref price_unit);

        PlayerData.SaveItemProfitCoefficient (level, number_buy_times);

        RefreshUI (level <= PlayerData.LastLevelUnlocked, level <= PlayerData.LastLevelUnlocked, PlayerData.LastLevelUnlocked - 2 < level);
    }

    #endregion

    #region Interact

    public void InteractUpgradeItems ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!is_can_buy)
            return;

        if (!ApplicationManager.Instance.IsMinusCurrency (price_upgrade, price_upgrade_unit, currency_id))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (currency_id);

            return;
        }

        switch (currency_id)
        {
            case CurrencyEnums.CurrencyId.Ads:

                if (this.IsRewardVideoAvailable ())
                {
                    this.ExecuteRewardAds (OnUpgradeCompleted, null);
                    return;
                }
                else
                {
                    this.RefreshRewardVideo ();
                }

                ApplicationManager.Instance.AlertNoAdsAvailable ();

                return;
            case CurrencyEnums.CurrencyId.Cash:


                OnUpgradeCompleted ();


                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);


                break;
            case CurrencyEnums.CurrencyId.Coca:

                OnUpgradeCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoca);

                break;
            case CurrencyEnums.CurrencyId.Diamonds:


                OnUpgradeCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

                break;
        }

        GameActionManager.Instance.InstanceFxTap (label_price_upgrade.transform.position);
    }

    public void InteractBuyItems ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!is_can_buy)
            return;

        if (GameManager.Instance.IsFreeIndexGrid () == false)
        {
            ApplicationManager.Instance.AlertNoMorePark ();

            return;
        }

        if (is_use_diamond)
        {
            if (!ApplicationManager.Instance.IsMinusCurrency (price_diamonds, 0, CurrencyEnums.CurrencyId.Diamonds))
            {
                ApplicationManager.Instance.AlertNotEnoughCurrency (currency_id);

                return;
            }

            OnBuyCompleted ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

            GameActionManager.Instance.InstanceFxTapDiamonds (currency_icon.transform.position);

            return;
        }

        switch (currency_id)
        {
            case CurrencyEnums.CurrencyId.Diamonds:
                if (!ApplicationManager.Instance.IsMinusCurrency (price_diamonds, 0, currency_id))
                {
                    ApplicationManager.Instance.AlertNotEnoughCurrency (currency_id);

                    return;
                }

                break;
            default:
                if (!ApplicationManager.Instance.IsMinusCurrency (price, price_unit, currency_id))
                {
                    ApplicationManager.Instance.AlertNotEnoughCurrency (currency_id);

                    return;
                }

                break;
        }

        switch (currency_id)
        {
            case CurrencyEnums.CurrencyId.Ads:

                if (this.IsRewardVideoAvailable ())
                {
                    this.ExecuteRewardAds (OnBuyCompleted, null);
                    return;
                }
                else
                {
                    this.RefreshRewardVideo ();
                }


                ApplicationManager.Instance.AlertNoAdsAvailable ();

                break;
            case CurrencyEnums.CurrencyId.Cash:

                OnBuyCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);

                GameActionManager.Instance.InstanceFxTap (currency_icon.transform.position);

                break;
            case CurrencyEnums.CurrencyId.Coca:

                OnBuyCompleted ();


                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoca);

                break;
            case CurrencyEnums.CurrencyId.Diamonds:

                OnBuyCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

                break;
        }
    }

    public void InteractClose ()
    {
        // =============================== Close the shop ================================ //

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        ShopManager.Instance.DisableHud ();

        UIGameManager.Instance.InitDataBuy (last_level_buying);
    }

    public void InteractNextItems ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        level = level + 1;

        if (level > GameConfig.TotalItem)
            level = GameConfig.TotalItem;

        InitParameters (CurrencyEnums.CurrencyId.Cash, level);
        SetId (GameData.Instance.ItemNodeIdleImage.GetIcon (level));
        RefreshUI (level <= PlayerData.LastLevelUnlocked, level <= PlayerData.LastLevelUnlocked, PlayerData.LastLevelUnlocked - 2 < level);
    }


    public void InteractPreviousItems ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        level = level - 1;

        if (level < 1)
            level = 1;

        InitParameters (CurrencyEnums.CurrencyId.Cash, level);
        SetId (GameData.Instance.ItemNodeIdleImage.GetIcon (level));
        RefreshUI (level <= PlayerData.LastLevelUnlocked, level <= PlayerData.LastLevelUnlocked, PlayerData.LastLevelUnlocked - 2 < level);
    }

    #endregion

    #region Helper

    public Vector3 GetPositionUpgradeDiamonds ()
    {
        return label_price_upgrade.transform.position;
    }

    #endregion
}