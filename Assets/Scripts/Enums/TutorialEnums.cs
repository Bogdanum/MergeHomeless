using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialEnums {

	public enum TutorialId
	{
		None,
		HowToPlayGame,
		HowToTouchBox,
		HowToMerge,
		HowToEarningCoins,
		HowToTouchItemBack,
		HowToSpeedUp,
		HowToUpgradeEquipments,
		HowToUpgradeItems,
		HowToSpin,
	}

	public static readonly string[] TutorialString =
	{
		"none",
		"how_to_play_game",
		"how_to_touch_box",
		"how_to_merge",
		"how_to_earning_coins",
		"how_to_touch_item_back",
		"how_to_speed_up",
		"how_to_upgrade_equipments",
		"how_to_upgrade_items",
		"how_to_spin"
	};

	public static string GetTutorialKey (TutorialEnums.TutorialId id)
	{
		return TutorialString[(int) id];
	}
	
	public static int GetSizeTutorial ()
	{
		return TutorialString.Length;
	}
}
