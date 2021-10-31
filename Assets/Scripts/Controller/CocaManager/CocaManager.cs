using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CocaManager : Singleton<CocaManager>, IDialog
{
    [Header ("UI")] [SerializeField]
    private UICocaManager _UiCocaManager;

    #region Variables

    private double _ExchangeValue;
    private int    _ExchangeUnit;

    private double _Coin;
    private int    _CoinUnit;

    private double _CoinExchange;

    #endregion

    protected override void Awake ()
    {
        base.Awake ();

        _UiCocaManager.Init ();

        _CoinExchange = 10000.0;
    }

    #region Action

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _UiCocaManager.EnableHud ();

        GameManager.Instance.DisableTouch ();
     
        MinValue ();
    }

    public void DisableHud ()
    {
        ApplicationManager.Instance.UnSetDialog (this);

        _UiCocaManager.Close ();

        GameManager.Instance.EnableTouch ();
    }

    public void MinusValue ()
    {
        Helper.MinusValue (_ExchangeValue, _ExchangeUnit, 1, 0, out _ExchangeValue, out _ExchangeUnit);

        Helper.GetRealExchangeCoins (out _Coin, out _CoinUnit, _ExchangeValue, _ExchangeUnit, _CoinExchange, 0);

        if (Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit))
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.blue);
        }
        else
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.red);
        }
    }

    public void PlusValue ()
    {
        Helper.AddValue (ref _ExchangeValue, ref _ExchangeUnit, 1, 0);

        Helper.GetRealExchangeCoins (out _Coin, out _CoinUnit, _ExchangeValue, _ExchangeUnit, _CoinExchange, 0);

        if (Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit))
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.blue);
        }
        else
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.red);
        }
    }

    public void MinValue ()
    {
        if (Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _CoinExchange, 0))
        {
            _ExchangeValue = 1;
            _ExchangeUnit  = 0;
        }
        else
        {
            _ExchangeValue = 0;
            _ExchangeUnit  = 0;
        }

        Helper.GetRealExchangeCoins (out _Coin, out _CoinUnit, _ExchangeValue, _ExchangeUnit, _CoinExchange, 0);

        if (Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit))
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.blue);
        }
        else
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.red);
        }
    }

    public void MaxValue ()
    {
        Helper.GetTimesValue (PlayerData.Coins, PlayerData.CoinUnit, _CoinExchange, 0, out _ExchangeValue, out _ExchangeUnit);

        Helper.GetRealExchangeCoins (out _Coin, out _CoinUnit, _ExchangeValue, _ExchangeUnit, _CoinExchange, 0);

        if (Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit))
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.blue);
        }
        else
        {
            _UiCocaManager.UpdateTextExchangeValue (ApplicationManager.Instance.AppendFromUnit (_ExchangeValue, _ExchangeUnit), Color.red);
        }
    }

    public void ExchangeValue ()
    {
        if (!Helper.IsEqualOrSmaller (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit))
        {
            return;
        }

        Helper.MinusValue (PlayerData.Coins, PlayerData.CoinUnit, _Coin, _CoinUnit, out PlayerData.Coins, out PlayerData.CoinUnit);
        Helper.AddValue (ref PlayerData.Coca, ref PlayerData.CocaUnit, _ExchangeValue, _ExchangeUnit);

        MinValue ();

        PlayerData.SaveCoca ();
        PlayerData.SaveCoins ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoca);
    }

    #endregion
}