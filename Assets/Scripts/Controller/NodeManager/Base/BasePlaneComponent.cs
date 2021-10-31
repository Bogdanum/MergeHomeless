using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BasePlaneComponent : NodeComponent, IIdle
{
    [Header ("Data")] [SerializeField] private ItemNodeImage _ItemNodeImage;

    [SerializeField] private SpriteRenderer sprite_renderer;

    [SerializeField] private Transform transform_renderer;

    private new Transform transform;

    private BasePlaneMoving _BasePlaneMoving;

    private bool IsMinerGold;
    private bool IsIdleGold;

    private ItemNodeData item_node_data;


    public override NodeComponent Init (ItemNodeData _item_node_data)
    {
        if (ReferenceEquals (transform, null))
        {
            transform = gameObject.transform;
        }

        item_node_data = _item_node_data;

        sprite_renderer.sprite  = _ItemNodeImage.GetIcon (_item_node_data.Level);
        sprite_renderer.enabled = true;

        return base.Init (item_node_data);
    }

    public override NodeComponent SetBusy (bool IsBusy)
    {
        IsMinerGold = IsBusy;

        return base.SetBusy (IsBusy);
    }

    public override NodeComponent SetEnable ()
    {
        IdleRegister ();

        sprite_renderer.enabled = true;

        return base.SetEnable ();
    }

    public override NodeComponent SetDisable ()
    {
        IdleUnRegister ();

        sprite_renderer.enabled = false;

        return base.SetDisable ();
    }

    public void SetPlaneMoving (BasePlaneMoving basePlaneMoving)
    {
        _BasePlaneMoving = basePlaneMoving;
    }

    #region Action

    public override NodeComponent TouchBusy ()
    {
        if (_BasePlaneMoving != null)
        {
            _BasePlaneMoving.Stop ();

            GameManager.Instance.SetItemBackToNode (GetIndexX (), GetIndexY (), _BasePlaneMoving, this);
            GameManager.Instance.MinusBaseActive ();

            PlayerData.SaveItemMoving (-1, GetIndexX (), GetIndexY ());
        }

        _BasePlaneMoving = null;

        return this;
    }

    public override NodeComponent TouchHit ()
    {
        double profit      = item_node_data.ProfitPerSec * item_node_data.PerCircleTime * GameConfig.PercentCoinEarnFromHitItem;
        int    profit_unit = item_node_data.ProfitPerSecUnit;

        Helper.FixNumber (ref profit, ref profit_unit);

        profit = profit * Mathf.Pow (item_node_data.ProfitPerUpgradeCoefficient, PlayerData.GetNumberUpgradeItemProfitCoefficient (item_node_data.Level));

        Helper.FixUnit (ref profit, ref profit_unit);

        if (profit < 1 && profit_unit == 0)
        {
            profit      = GameConfig.DefaultCoinEarn;
            profit_unit = 0;
        }

        GameManager.Instance.FxEarnCoin (profit, profit_unit, GetPosition ());
        GameManager.Instance.FxTapNode (transform, null);

        GameActionManager.Instance.InstanceFxTapCoins (GetPosition ());

        this.PlayAudioSound (AudioEnums.SoundId.TapOnItem);

        this.PostMissionEvent (MissionEnums.MissionId.TapOnItem);

        if (Random.Range (0.00f, 1.00f) < 0.25f)
        {
            this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);
        }

        return this;
    }

    public void EarnCoins ()
    {
        // 21.10.2021 ------- add profit * 7 
        double profit      = item_node_data.ProfitPerSec * item_node_data.PerCircleTime * GameConfig.PercentCoinEarnFromIdle * 7;
        int    profit_unit = item_node_data.ProfitPerSecUnit;

        Helper.FixNumber (ref profit, ref profit_unit);

        profit = profit * Mathf.Pow (item_node_data.ProfitPerUpgradeCoefficient, PlayerData.GetNumberUpgradeItemProfitCoefficient (item_node_data.Level));

        profit += profit * Contains.MultiRewardFromCoins;

        Helper.FixUnit (ref profit, ref profit_unit);

        if (profit < 1 && profit_unit == 0)
        {
            profit      = GameConfig.DefaultCoinEarn;
            profit_unit = 0;
        }

        GameManager.Instance.FxDisplayEarnCoin (profit, profit_unit, GetPosition ());
        GameManager.Instance.FxTapNode (transform, null);
    }

    public void IdleRegister ()
    {
        if (IsIdleGold == true)
            return;

        IsIdleGold = true;
        // 21.10.2021 ------- old /2  new * 5
        GameIdleAction.Instance.RegisterIdle (this, item_node_data.PerCircleTime * 5);
    }

    public void IdleUnRegister ()
    {
        if (IsIdleGold == false)
            return;

        IsIdleGold = false;

        GameIdleAction.Instance.UnRegisterIdle (this);
    }

    #endregion

    #region Helper

    public override bool IsBusy ()
    {
        return IsMinerGold;
    }

    public bool IsStop ()
    {
        return !IsIdleGold;
    }

    #endregion
}