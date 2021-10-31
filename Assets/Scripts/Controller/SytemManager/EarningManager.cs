using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarningManager : Singleton<EarningManager>
{
    private double profit_data;
    private int    profit_unit;

    private List<ItemNodeData> _DataInMining;

    #region Variables

    private double _ProfitPerSec;
    private int    _ProfitUnit;

    private double _RealProfitPerSec;
    private int    _RealProfitUnit;

    #endregion

    protected override void Awake ()
    {
        base.Awake ();

        InitConfig ();
        InitEvent ();
    }

    protected override void OnDestroy ()
    {
        UnInitEvent ();

        base.OnDestroy ();
    }

    private void InitEvent ()
    {
        this.RegisterActionEvent (ActionEnums.ActionID.UpdateEarningCoins, OnUpdateEarningCoins);

        OnUpdateEarningCoins (null);
    }

    private void UnInitEvent ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.UpdateEarningCoins, OnUpdateEarningCoins);
    }

    private void InitConfig ()
    {
        _DataInMining = new List<ItemNodeData> ();
    }

    #region Action

    public void RegisterData (ItemNodeData data)
    {
        _DataInMining.Add (data);

        double profit = data.ProfitPerSec;
        var    unit   = data.ProfitPerSecUnit;

        profit = profit * Mathf.Pow ( data.ProfitPerUpgradeCoefficient , PlayerData.GetNumberUpgradeItemProfitCoefficient (data.Level));

        Helper.FixUnit (ref profit, ref unit);

        Helper.AddValue (ref _ProfitPerSec, ref _ProfitUnit, profit, unit);

        _RealProfitUnit   = _ProfitUnit;
        _RealProfitPerSec = _ProfitPerSec;

        GetRealEarning (ref _RealProfitPerSec, ref _RealProfitUnit);
    }

    public void UnRegisterData (ItemNodeData data)
    {
        for (int i = 0; i < _DataInMining.Count; i++)
        {
            if (_DataInMining[i].Level != data.Level) continue;

            _DataInMining.RemoveAt (i);

            double profit = data.ProfitPerSec;
            var    unit   = data.ProfitPerSecUnit;

            profit = profit * Mathf.Pow ( data.ProfitPerUpgradeCoefficient , PlayerData.GetNumberUpgradeItemProfitCoefficient (data.Level));

            Helper.FixUnit (ref profit, ref unit);
            
            Helper.MinusValue (_ProfitPerSec, _ProfitUnit, profit, unit, out _ProfitPerSec, out _ProfitUnit);

            _RealProfitUnit   = _ProfitUnit;
            _RealProfitPerSec = _ProfitPerSec;

            GetRealEarning (ref _RealProfitPerSec, ref _RealProfitUnit);

            break;
        }
    }

    #endregion

    #region Helper

    public string GetProfitPerSec ()
    {
        return ApplicationManager.Instance.AppendFromCashUnit (_RealProfitPerSec, _RealProfitUnit, 2);
    }

    public double ProfitPerSec
    {
        get { return _RealProfitPerSec; }
    }

    public int ProfitUnit
    {
        get { return _RealProfitUnit; }
    }

  
    #endregion

    private void OnUpdateEarningCoins (object obj)
    {
        profit_data = 0;
        profit_unit = 0;

        GameData.Instance.GetProfit (EquipmentEnums.AbilityId.IncreaseEarnCoins, out profit_data, out profit_unit);

        _RealProfitUnit   = _ProfitUnit;
        _RealProfitPerSec = _ProfitPerSec;

        GetRealEarning (ref _RealProfitPerSec, ref _RealProfitUnit);

        if (UIGameManager.Instance != null)
            UIGameManager.Instance.UpdateTextProfitPerSec ();
    }

    public void UpdateEarning (double rawCoins, int unit)
    {
        SetCoins (rawCoins, unit);
    }

    public void RefreshEarning () { }

    public void SetCoins (double quantity, int unit)
    {
        Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, quantity, unit);

        PlayerData.SaveCoins ();

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
    }

    public void GetRealEarning (ref double value, ref int unit)
    {
        Helper.AddPercents (ref value, ref unit, profit_data, profit_unit);

        value = value + value * Contains.MultiRewardFromCoins + value * Contains.MultiRewardFromSpeed;

        Helper.FixUnit (ref value, ref unit);
    }
}