using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>, IDialog
{
    [Header ("UI")] [SerializeField] private UIUnlockManager _UiUnlockManager;

    [Header ("Data")] [SerializeField] private ItemNodeGroupData _ItemNodeGroup;

    [Header ("Data")] [SerializeField] private ItemNodeImage  _ItemNodeImages;
    [Header ("Data")] [SerializeField] private ItemShopDetail _ItemShopDetail;
    [SerializeField]                   private ShareData      _ShareData;

    private int _Level;

    public void UnlockItemLevel (int item_level)
    {
        var id      = _ItemNodeImages.GetIcon (item_level);
        var speed   = _ItemShopDetail.GetSpeed (item_level);
        var earning = _ItemShopDetail.GetEarning (item_level);

        _Level = item_level;

        _UiUnlockManager.Init (id, speed, earning, ApplicationLanguage.GetItemName (item_level), GameData.Instance.ItemDataGroup.GetDataItemWithLevel (item_level));
        _UiUnlockManager.SetShareFacebook (ApplicationManager.Instance.AppendFromCashUnit (_ShareData.GetRewardValue (_Level), _ShareData.GetRewardUnit (_Level)));

        this.PostMissionEvent (MissionEnums.MissionId.ReachCats, item_level);

        EnableHud ();
    }

    #region Action

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _UiUnlockManager.EnableHud ();
        GameManager.Instance.DisableTouch ();
    }

    public void DisableHud ()
    {
        ApplicationManager.Instance.UnSetDialog (this);

        _UiUnlockManager.Disable ();
        GameManager.Instance.EnableTouch ();
    }

    public void DoubleReward ()
    {
        if (this.IsRewardVideoAvailable ())
        {
            this.ExecuteRewardAds (()=>OnGetReward(2), null);
            
            return;
        }
        
        ApplicationManager.Instance.AlertNoAdsAvailable ();
    }

    public void OnGetReward (int time)
    {
        var reward_value = _ShareData.GetRewardValue (_Level) * time;
        var reward_unit  = _ShareData.GetRewardUnit (_Level);

        Helper.FixUnit (ref reward_value, ref reward_unit);

        Instance.DisableHud ();

        GameManager.Instance.FxEarnCoin (reward_value, reward_unit, Vector.Vector3Zero);

    }

    #endregion
}