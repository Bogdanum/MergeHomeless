using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNodeGroupData : ScriptableObject
{
    [Header ("Data")] [SerializeField] private ItemNodeData[] _ItemNodeDatas;

    public int GetMaxLevel ()
    {
        var max = 0;

        for (int i = 0; i < _ItemNodeDatas.Length; i++)
        {
            if (_ItemNodeDatas[i].Level > max)
            {
                max = _ItemNodeDatas[i].Level;
            }
        }

        return max;
    }

    public ItemNodeData GetDataItemWithLevel (int level)
    {
        for (int i = 0; i < _ItemNodeDatas.Length; i++)
        {
            if (_ItemNodeDatas[i].Level == level)
            {
                return _ItemNodeDatas[i];
            }
        }

        return _ItemNodeDatas.Length > 0 ? _ItemNodeDatas[0] : null;
    }

    public int GetSize ()
    {
        return _ItemNodeDatas.Length;
    }

    public ItemNodeData GetDataWithIndex (int index)
    {
        return _ItemNodeDatas.Length > index ? _ItemNodeDatas[index] : null;
    }
}