using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPData : ScriptableObject
{
    public IapEnums.IapId   id;
    public IapEnums.TypeIap TypeIap;
    public string           IapId;
    public int              Value;
    public string           DescriptionSalePercent;
    public string           PriceOffline;
}