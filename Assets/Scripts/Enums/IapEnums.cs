using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IapEnums : ScriptableObject
{
    public enum IapId
    {
        SmallDiamondsPack,
        MediumDiamondsPack,
        BigDiamondsPack,
        RemoveAdsPack,
        FreePack,
    }

    public enum TypeIap
    {
        Consumable,
        NonConsumable,
        FreeAds,
    }

    public readonly static string[] IapName =
    {
        "small_diamonds_packs",
        "medium_diamonds_packs",
        "big_diamonds_pack",
        "remove_ads_pack",
        "free_pack"
    };

    public static string GetName (IapId id)
    {
        return IapName[(int) id];
    }
}