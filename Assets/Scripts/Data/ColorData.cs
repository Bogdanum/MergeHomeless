using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorData : ScriptableObject {

	[System.Serializable]
	public struct ColorProperty
	{
		public int Level;
		public Color Color;
	}

	[SerializeField] private ColorProperty[] _ColorProperties;

	public Color GetColor (int level)
	{
		for (int i = 0; i < _ColorProperties.Length; i++)
		{
			if (_ColorProperties[i].Level == level)
			{
				return _ColorProperties[i].Color;
			}
		}
		
		return Color.white;
	}
}
