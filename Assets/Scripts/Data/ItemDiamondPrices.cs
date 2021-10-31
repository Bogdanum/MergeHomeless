using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDiamondPrices : ScriptableObject {

	[System.Serializable]
	public struct ItemDiamondPrice
	{
		public int Level;
		public int Price;
	}

	[SerializeField] private ItemDiamondPrice[] _Prices;

	public int GetPrice (int level)
	{
		for (int i = 0; i < _Prices.Length; i++)
		{
			if (_Prices[i].Level == level)
			{
				return _Prices[i].Price;
			}
		}

		return -1;
	}
}
