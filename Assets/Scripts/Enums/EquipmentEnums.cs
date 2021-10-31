using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquipmentEnums  {

	public enum AbilityId
	{
		None,
		IncreaseEarnCoins,
		DiscountBuy,
		MultiRewardCoins,
		DiscountUpgradeItems,
		DiscountUpgradeFood,
		IncreaseEarnFood,
	}

	private static readonly string[] EquipmentKey =
	{
		"none",
		"increase_earn_coins",
		"discount_buy",
		"multi_reward_coins",
		"discount_upgrade_items",
		"discount_upgrade_food",
		"increase_earn_food"
	};

	public static string GetKey (AbilityId id)
	{
		return EquipmentKey[(int) id];
	}

	public static int GetLength ()
	{
		return EquipmentKey.Length;
	}

	public static AbilityId GetId (string key)
	{
		for (int i = 0; i < EquipmentKey.Length; i++)
		{
			if (string.CompareOrdinal (EquipmentKey[i], key) == 0)
				return (AbilityId) i;
		}

		return AbilityId.None;
	}
}
