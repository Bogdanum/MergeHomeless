using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxData : ScriptableObject {

	[System.Serializable]
	public struct BoxProperty
	{
		public BoxEnums.BoxId BoxId;
		public Sprite Icon;
	}

	[SerializeField] private BoxProperty[] _BoxProperties;

	public Sprite GetIcon (BoxEnums.BoxId id)
	{
		for (int i = 0; i < _BoxProperties.Length; i++)
		{

			if (_BoxProperties[i].BoxId == id)
				return _BoxProperties[i].Icon;
		}

		return null;
	}

}
