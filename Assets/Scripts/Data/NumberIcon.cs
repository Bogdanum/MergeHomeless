using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberIcon : ScriptableObject {
	
	[System.Serializable]
	public struct NumberProperty
	{
		public int id;
		public Sprite icon;
	}

	[SerializeField] private NumberProperty[] number_properties;

	#region Helper

	public Sprite GetIcon (int id)
	{
		for (int i = 0; i < number_properties.Length; i++)
		{
			if (id == number_properties[i].id)
				return number_properties[i].icon;
		}

		return null;
	}

	#endregion

}
