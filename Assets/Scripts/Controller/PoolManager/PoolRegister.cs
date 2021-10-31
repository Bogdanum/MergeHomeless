using UnityEngine;

public class PoolRegister : MonoBehaviour
{
    [System.Serializable]
    public struct PoolData
    {
        public PoolEnums.PoolId poolID;
        public Transform        poolPrefab;
        public int              quantity;
        public bool             IsExpand;
    }

    [SerializeField] private PoolData[] poolData = null;

    private void Awake ()
    {
        InstancePool ();
        PoolExtension.RegisterPoolExtension (this);
    }

    private void OnDestroy ()
    {
        PoolExtension.RemoverPoolExtension (this);
        RemovePoolKey ();
    }

    private void RemovePoolKey ()
    {
        for (int i = 0; i < poolData.Length; i++)
        {
            PoolExtension.RemovePoolKeyExtension (poolData[i].poolID, false);
        }
    }

    public Transform InstancePool (PoolEnums.PoolId poolID)
    {
        for (int i = 0; i < poolData.Length; i++)
        {
            var item = poolData[i];

            if (item.poolID == poolID)
            {
                if (item.IsExpand == false)
                    return null;

                var poolInstance = Instantiate (item.poolPrefab.gameObject, transform);

                return poolInstance.transform;
            }
        }

        return null;
    }

    /// <summary>
    /// Instances the pools.
    /// </summary>
    private void InstancePool ()
    {
        for (int i = 0; i < poolData.Length; i++)
        {
            var item = poolData[i];

            for (int j = 0; j < item.quantity; j++)
            {
                var poolInstance = Instantiate (item.poolPrefab.gameObject, transform);
                PoolExtension.SetPool (item.poolID, poolInstance.transform);
            }
        }
    }
}