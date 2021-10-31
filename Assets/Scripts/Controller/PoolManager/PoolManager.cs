using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string, List<Transform>> poolSaver    = new Dictionary<string, List<Transform>> ();
    private readonly List<PoolRegister>                  poolRegister = new List<PoolRegister> ();

    private int m_Length;

    public PoolManager RegisterPoolExtension (PoolRegister register)
    {
        if (poolRegister.Contains (register)) return this;

        poolRegister.Add (register);
        m_Length++;

        return this;
    }

    public PoolManager RemovePoolExtension (PoolRegister remover)
    {
        if (!poolRegister.Contains (remover)) return this;

        poolRegister.Remove (remover);
        m_Length--;

        return this;
    }

    public PoolManager RemovePoolKeys (PoolEnums.PoolId poolID, bool IsDestroy = true)
    {
        var poolKey = PoolEnums.GetPoolKey (poolID);

        if (IsDestroy)
        {
            List<Transform> poolExport = null;

            if (poolSaver.TryGetValue (poolKey, out poolExport))
            {
                for (int i = 0; i < poolExport.Count; i++)
                {
                    Destroy (poolExport[i].gameObject);
                }
            }
        }

        if (poolSaver.ContainsKey (poolKey))
        {
            poolSaver.Remove (poolKey);
        }

        return this;
    }

    private Transform InstancePool (PoolEnums.PoolId poolID)
    {
        for (int i = 0; i < poolRegister.Count; i++)
        {
            var poolReturn = poolRegister[i].InstancePool (poolID);

            if (!ReferenceEquals (poolReturn, null))
            {
                return poolReturn;
            }
        }

        return null;
    }

    public Transform GetPools (PoolEnums.PoolId poolID, bool IsEnable = true)
    {
        var poolKey = PoolEnums.GetPoolKey (poolID);

        Transform       poolReturn = null;
        List<Transform> poolExport = null;

        if (poolSaver.TryGetValue (poolKey, out poolExport))
        {
            if (poolExport.Count > 0)
            {
                poolReturn = poolExport[0];

                poolExport.RemoveAt (0);
            }
        }

        if (ReferenceEquals (poolReturn, null))
        {
            poolReturn = InstancePool (poolID);
        }

        if (!ReferenceEquals (poolReturn, null) && poolReturn.gameObject.activeSelf != IsEnable)
        {
            poolReturn.gameObject.SetActive (IsEnable);
        }

        return poolReturn;
    }

    public PoolManager SetPools (PoolEnums.PoolId poolID, Transform poolComponent)
    {
        var poolKey = PoolEnums.GetPoolKey (poolID);

        List<Transform> poolImport = null;

        if (poolSaver.TryGetValue (poolKey, out poolImport))
        {
            if (!poolImport.Contains (poolComponent))
            {
                poolImport.Add (poolComponent);
            }
        }
        else
        {
            poolImport = new List<Transform> {poolComponent};
            poolSaver.Add (poolKey, poolImport);
        }

        if (poolComponent.gameObject.activeSelf == true)
        {
            poolComponent.gameObject.SetActive (false);
        }

        return this;
    }
}

public static class PoolExtension
{
    public static PoolManager SetPool (PoolEnums.PoolId poolID, Transform poolComponent)
    {
        return PoolManager.InstanceAwake () == null ? null : PoolManager.Instance.SetPools (poolID, poolComponent);
    }

    public static Transform GetPool (PoolEnums.PoolId poolID, bool IsEnable = true)
    {
        return PoolManager.InstanceAwake ().GetPools (poolID, IsEnable);
    }

    public static PoolManager RegisterPoolExtension (PoolRegister register)
    {
        return PoolManager.InstanceAwake () == null ? null : PoolManager.Instance.RegisterPoolExtension (register);
    }

    public static PoolManager RemoverPoolExtension (PoolRegister remover)
    {
        return PoolManager.InstanceAwake () == null ? null : PoolManager.Instance.RemovePoolExtension (remover);
    }

    public static PoolManager RemovePoolKeyExtension (PoolEnums.PoolId poolID, bool IsDestroy = true)
    {
        return PoolManager.InstanceAwake () == null ? null : PoolManager.Instance.RemovePoolKeys (poolID,IsDestroy);
    }
}