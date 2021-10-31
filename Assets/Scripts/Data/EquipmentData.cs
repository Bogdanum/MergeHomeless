using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ScriptableObject
{
    public string                   Name;
    public EquipmentEnums.AbilityId AbilityId;
    public CurrencyEnums.CurrencyId CurrencyId;
    public float                    UpgradeCoefficient;
    public float                    Price;
    public int                      PriceUnit;
    public float                    PriceCoefficient;
    public int                      UpgradeFromLevel;
    public int                      MaxLevel;
}