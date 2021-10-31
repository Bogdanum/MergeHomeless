using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlaneMoving : MonoBehaviour, IMoving
{
    private ItemNodeData _ItemNodeData;

    [SerializeField] private PoolEnums.PoolId _PoolId;

    [Header ("Fx")] [SerializeField] private ParticleSystem[] _FxRunLine;

    [Header ("Data")] [SerializeField] private ColorData _ColorData;

    [SerializeField] private SpriteRenderer sprite_renderer;

    private int InstanceId;

    private new Transform transform;

    #region System

    private void Awake ()
    {
        InitConfig ();
    }

    #endregion

    #region Controller

    private void InitConfig ()
    {
        transform  = gameObject.transform;
        InstanceId = GetInstanceID ();
    }

    #endregion


    #region Action

    public void SetItemData (ItemNodeData _itemNodeData)
    {
        _ItemNodeData          = _itemNodeData;
        sprite_renderer.sprite = GameData.Instance.ItemMoving.GetIcon (_itemNodeData.Level);
    }

    public void Register (int indexPoint)
    {
        GameMovingManager.Instance.RegisterMoving (this, _ItemNodeData, indexPoint, _ItemNodeData.PerCircleTime);
        EarningManager.Instance.RegisterData (_ItemNodeData);
        UIGameManager.Instance.UpdateTextProfitPerSec ();
    }

    public void Stop ()
    {
        UnRegister ();
    }

    public void UnRegister ()
    {
        GameMovingManager.Instance.UnRegisterMoving (this);
        EarningManager.Instance.UnRegisterData (_ItemNodeData);
        UIGameManager.Instance.UpdateTextProfitPerSec ();
    }

    public void EnableFx ()
    {
        for (int i = 0; i < _FxRunLine.Length; i++)
        {
            _FxRunLine[i].Play ();
        }
    }

    public void DisableFx ()
    {
        for (int i = 0; i < _FxRunLine.Length; i++)
        {
            _FxRunLine[i].Stop ();
        }
    }

    public Transform GetTransform ()
    {
        return transform;
    }

    public void ReturnToPools ()
    {
        PoolExtension.SetPool (_PoolId, transform);
    }

    #endregion
}