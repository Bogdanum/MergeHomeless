using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostProperty : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Text  description_jump_time;
    [SerializeField]                 private Text  description_value_coins;
    [SerializeField]                 private Image icon;

    [SerializeField] private Button button_buy;

    [SerializeField] private Text label_price_buy;

    #region Variables

    private double profit;
    private int    profit_unit;

    private BoostData.TimeJumpId time_jump_id;

    private int price_buy;

    private BoostData.BoostProperty boost_property;

    #endregion

    #region Action

    public void Init (BoostData.BoostProperty data)
    {
        boost_property = data;

        RefreshLanguage ();

        time_jump_id         = data.time_jump_id;
        icon.sprite          = data.icon;
        label_price_buy.text = data.price_diamonds.ToString ();

        price_buy = data.price_diamonds;
    }

    public void RefreshCash ()
    {
        double time = 0.0;

        switch (time_jump_id)
        {
            case BoostData.TimeJumpId.FourteenDay:
                time = 1209600;
                break;
            case BoostData.TimeJumpId.OneDay:
                time = 86400;
                break;
            case BoostData.TimeJumpId.SevenDay:
                time = 604800;
                break;
            case BoostData.TimeJumpId.ThirtyDay:
                time = 2592000;
                break;
        }

        var real_profit = EarningManager.Instance.ProfitPerSec;
        var real_unit   = EarningManager.Instance.ProfitUnit;

        if ((int) real_profit == 0 && real_unit == 0)
        {
            real_profit = 1;
            real_unit   = 0;
        }

        Helper.GetCashWithTime (out profit, out profit_unit, real_profit, real_unit, time);

        description_value_coins.text = string.Format (ApplicationLanguage.Text_label_get, ApplicationManager.Instance.AppendFromCashUnit (profit, profit_unit));
    }


    public void RefreshLanguage ()
    {
        switch (boost_property.time_jump_id)
        {
            case BoostData.TimeJumpId.FourteenDay:
                description_jump_time.text = ApplicationLanguage.Text_description_time_jump_14_day;
                break;
            case BoostData.TimeJumpId.OneDay:
                description_jump_time.text = ApplicationLanguage.Text_description_time_jump_1_day;
                break;
            case BoostData.TimeJumpId.SevenDay:
                description_jump_time.text = ApplicationLanguage.Text_description_time_jump_7_day;
                break;
            case BoostData.TimeJumpId.ThirtyDay:
                description_jump_time.text = ApplicationLanguage.Text_description_time_jump_30_day;
                break;
        }
    }
    #endregion


    #region Interact

    public void InteractBuy ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!ApplicationManager.Instance.IsMinusCurrency (price_buy, 0, CurrencyEnums.CurrencyId.Diamonds))
        {
            ApplicationManager.Instance.AlertNotEnoughCurrency (CurrencyEnums.CurrencyId.Diamonds);

            return;
        }

        GameManager.Instance.FxEarnCoin (profit, profit_unit, Vector.Vector3Zero);

        this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
    }

    #endregion
}