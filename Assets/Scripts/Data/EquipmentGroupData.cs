using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EquipmentGroupData : ScriptableObject {

	[Header ("Data")]
	[SerializeField] private EquipmentData[] _EquipmentData;

	public EquipmentData GetData (EquipmentEnums.AbilityId id)
	{
		for (int i = 0; i < _EquipmentData.Length; i++)
		{
			if (_EquipmentData[i].AbilityId == id)
			{
				return _EquipmentData[i];
			}
		}

		return null;
	}

	public EquipmentData GetData (int index)
	{
		return _EquipmentData.Length > index ? _EquipmentData[index] : null;
	}

	public int GetSize ()
	{
		return _EquipmentData.Length;
	}
}
