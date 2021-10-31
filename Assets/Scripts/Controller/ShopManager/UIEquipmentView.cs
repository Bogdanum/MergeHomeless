using System;
using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentView : MonoBehaviour
{
    [SerializeField] private Image icon_equipment, icon_currency;

    [SerializeField] private Text description_equipment,
                                  level_equipment,
                                  price_upgrade_equipment;

    [SerializeField] private Button interact_upgrade;

    private EquipmentEnums.AbilityId equipment_id;

    private int level_upgrade;

    private double price_upgrade;
    private int    price_unit_upgrade;

    private double price_real_upgrade;
    private int    price_unit_real_upgrade;

    private float price_coefficient_per_buy;

    private float profit_coefficient_per_upgrade;

    private double profit_upgrade;
    private int    profit_upgrade_unit;

    private int upgrade_from_level;
    private int upgrade_max_level;

    private CurrencyEnums.CurrencyId currency_id;

    #region Action

    public void InitUpgradeItems (EquipmentEnums.AbilityId id)
    {
        equipment_id = id;

        level_upgrade = PlayerData.GetEquipmentUpgrade (id);

        var data = GameData.Instance.EquipmentData.GetData (equipment_id);

        upgrade_max_level = data.MaxLevel;

        profit_coefficient_per_upgrade = data.UpgradeCoefficient;

        price_upgrade      = data.Price;
        price_unit_upgrade = data.PriceUnit;

        price_coefficient_per_buy = data.PriceCoefficient;

        upgrade_from_level = data.UpgradeFromLevel;

        currency_id = data.CurrencyId;
    }

    public void RefreshUi ()
    {
        level_equipment.text = string.Format (ApplicationLanguage.Text_description_lv, level_upgrade.ToString ());

        if (upgrade_max_level > 0 && upgrade_max_level <= level_upgrade)
        {
            price_upgrade_equipment.text = ApplicationLanguage.Text_label_max;
        }
        else
        {
            price_upgrade_equipment.text = ApplicationManager.Instance.AppendFromCashUnit (price_real_upgrade, price_unit_real_upgrade);
        }

        icon_currency.sprite  = GameData.Instance.CurrencyIcon.GetIconCurrency (currency_id);
        icon_equipment.sprite = GameData.Instance.EquipmentImage.GetIcon (equipment_id);


        RefreshLanguage ();
        RefreshInteract ();
    }

    public void RefreshInteract ()
    {
        if (upgrade_from_level > PlayerData.Level || upgrade_max_level > 0 && upgrade_max_level <= level_upgrade)
        {
            interact_upgrade.interactable = false;

            return;
        }

        switch (currency_id)
        {
            case CurrencyEnums.CurrencyId.Coca:
                interact_upgrade.interactable = Helper.IsEqualOrSmaller (PlayerData.Coca, PlayerData.CocaUnit, price_real_upgrade, price_unit_real_upgrade);
                break;
            case CurrencyEnums.CurrencyId.Cash:
                interact_upgrade.interactable = Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, price_real_upgrade, price_unit_real_upgrade);
                break;
            case CurrencyEnums.CurrencyId.Diamonds:
                interact_upgrade.interactable = PlayerData.Diamonds >= (int) price_upgrade;
                break;
        }
    }

    public void RefreshUpgrade ()
    {
        price_real_upgrade      = price_upgrade * Math.Pow (price_coefficient_per_buy, level_upgrade);
        price_unit_real_upgrade = price_unit_upgrade;

        Helper.FixUnit (ref price_real_upgrade, ref price_unit_real_upgrade);

        profit_upgrade      = Math.Pow (profit_coefficient_per_upgrade, level_upgrade);
        profit_upgrade_unit = 0;

        Helper.FixUnit (ref profit_upgrade, ref profit_upgrade_unit);
    }

    public void RefreshLanguage ()
    {
        switch (equipment_id)
        {
            case EquipmentEnums.AbilityId.DiscountBuy:
                description_equipment.text = string.Format (ApplicationLanguage.Text_description_equip_discount_buy, ApplicationManager.Instance.AppendFromUnit (profit_upgrade, profit_upgrade_unit, 2));
                break;
            case EquipmentEnums.AbilityId.IncreaseEarnCoins:
                description_equipment.text = string.Format (ApplicationLanguage.Text_description_equip_earning, ApplicationManager.Instance.AppendFromUnit (profit_upgrade, profit_upgrade_unit, 2));
                break;
            case EquipmentEnums.AbilityId.DiscountUpgradeItems:
                description_equipment.text = string.Format (ApplicationLanguage.Text_description_equip_discount_upgrade, ApplicationManager.Instance.AppendFromUnit (profit_upgrade, profit_upgrade_unit, 2));
                break;
        }
        level_equipment.text = string.Format(ApplicationLanguage.Text_description_lv, level_upgrade.ToString());
    }

    #endregion

    #region Callback

    private void OnUpgradeCompleted ()
    {
        IncreaseUpgradeValue ();

        switch (equipment_id)
        {
            case EquipmentEnums.AbilityId.DiscountBuy:
                this.PostActionEvent (ActionEnums.ActionID.UpdateDiscountBuy);
                break;
            case EquipmentEnums.AbilityId.IncreaseEarnCoins:
                this.PostActionEvent (ActionEnums.ActionID.UpdateEarningCoins);
                break;
            case EquipmentEnums.AbilityId.MultiRewardCoins:
                break;
        }
    }

    private void IncreaseUpgradeValue ()
    {
        level_upgrade++;

        PlayerData.SaveEquipmentUpgrade (equipment_id, level_upgrade);

        RefreshUpgrade ();
        RefreshUi ();
    }

    #endregion

    #region Interact

    public void InteractUpgrade ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!ApplicationManager.Instance.IsMinusCurrency (price_real_upgrade, price_unit_real_upgrade, currency_id))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (currency_id);

            return;
        }


        switch (currency_id)
        {
            case CurrencyEnums.CurrencyId.Cash:


                OnUpgradeCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);

                break;
            case CurrencyEnums.CurrencyId.Diamonds:


                OnUpgradeCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

                break;
            case CurrencyEnums.CurrencyId.Coca:


                OnUpgradeCompleted ();

                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoca);

                break;
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
        }

        GameManager.Instance.FxShakeScale (icon_equipment.transform);

        GameActionManager.Instance.InstanceFxLevelUp (icon_equipment.transform.position);
        GameActionManager.Instance.InstanceFxTap (interact_upgrade.transform.position);

        this.PlayAudioSound (AudioEnums.SoundId.UpgradeLevelUp);
    }

    #endregion

    #region Helper

    public void GetPrice (out double price_upgraded, out int price_unit_upgraded)
    {
        price_upgraded      = price_real_upgrade;
        price_unit_upgraded = price_unit_real_upgrade;
    }

    #endregion
}