using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    [Header ("Currency")] public CurrencyImage CurrencyIcon;

    [Header ("Node")] public ItemNodeGroupData ItemDataGroup;
    public                   GridNodeData      GridNodeData;

    public ItemNodeImage ItemMoving;
    public ItemNodeImage ItemNodeIdleImage;

    [Header ("Equipments")] public EquipmentGroupData EquipmentData;
    public                         EquipmentIconData  EquipmentIconData;
    public                         EquipmentImage     EquipmentImage;
    public                         EquipmentData      MultiRewardData;

    [Header ("Shop")] public ItemShopDetail    ItemShopDetail;
    public                   ItemDiamondPrices ItemDiamondPrices;

    [Header ("Boost")] public BoostData BoostData;

    [Header ("Number")] public NumberIcon NumberIcon;

    [Header ("Level")] public RewardLevelup RewardLevelup;

    [Header ("Items")] public ItemsData ItemHeadData;
    public                    ItemsData ItemFaceData;

    #region Helper

    public void GetProfit (EquipmentEnums.AbilityId id, out double profit_data, out int profit_unit)
    {
        var level = PlayerData.GetEquipmentUpgrade (id);
        var data  = EquipmentData.GetData (id);

        profit_data = Math.Pow (data.UpgradeCoefficient, level);
        profit_unit = 0;

        Helper.FixUnit (ref profit_data, ref profit_unit);
    }

    public double GetEquipmentPercents (EquipmentEnums.AbilityId id)
    {
        var upgrade_times = PlayerData.GetEquipmentUpgrade (id);

        var data = EquipmentData.GetData (id);

        return Math.Pow (data.UpgradeCoefficient, upgrade_times);
    }

    #endregion
}