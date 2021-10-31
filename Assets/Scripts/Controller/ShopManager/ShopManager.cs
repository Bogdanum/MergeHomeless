using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>, IDialog
{
    [SerializeField] private Transform shop_transform;

    [SerializeField] private Transform item_transform;
    [SerializeField] private Transform upgrade_transform, grid_upgrade;

    [Header ("Cats Shop")] [SerializeField]
    private UIItemView cat_shop_view;

    private List<UIEquipmentView> equipment_view_list;

    #region Variables

    public static bool IsEnabledItemShop;
    public static bool IsEnableEquipmentShop;

    private double last_price_notice;
    private int    last_price_unit_notice;

    private bool is_enabled_notice;

    #endregion

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitParameters ();
        InitShop ();

        RegisterRefreshUiCoinsEquipments ();
        cat_shop_view.RegisterRefreshUi ();

        RefreshNoticeEquipments ();
        cat_shop_view.RefreshNotice ();
    }

    protected override void OnDestroy ()
    {
        UnRegisterRefreshUiCoinsEquipments ();
        cat_shop_view.UnRegisterRefreshUi ();

        base.OnDestroy ();
    }

    #endregion

    #region Controller

    private void InitParameters ()
    {
        equipment_view_list = new List<UIEquipmentView> ();
    }

    public void InitShop ()
    {
        DisableItemsView ();
        EnableUpgradeView ();

        InitCatsShop ();
        InitUpgradeShop ();
    }

    private void InitCatsShop ()
    {
        cat_shop_view.InitParameters (CurrencyEnums.CurrencyId.Cash, 1)
                     .SetId (GameData.Instance.ItemNodeIdleImage.GetIcon (1))
                     .RefreshUI (true, true, false);
    }

    private void InitUpgradeShop ()
    {
        var size = GameData.Instance.EquipmentData.GetSize ();

        for (int i = 0; i < size; i++)
        {
            var data = GameData.Instance.EquipmentData.GetData (i);

            if (data == null)
                continue;

            var item = PoolExtension.GetPool (PoolEnums.PoolId.InteractEquipmentView, false);

            item.SetParent (grid_upgrade);

            item.localPosition = Vector.Vector3Zero;
            item.localScale    = Vector.Vector3One;

            item.gameObject.SetActive (true);

            var script = item.GetComponent<UIEquipmentView> ();

            script.InitUpgradeItems (data.AbilityId);
            script.RefreshUpgrade ();
            script.RefreshUi ();

            equipment_view_list.Add (script);
        }
    }

    #endregion

    #region Helper

    public Vector3 GetPositionUpgradeItems ()
    {
        return cat_shop_view.GetPositionUpgradeDiamonds ();
    }

    #endregion

    public void DisableHud ()
    {
        shop_transform.gameObject.SetActive (false);

        GameManager.Instance.EnableTouch ();

        ApplicationManager.Instance.UnSetDialog (this);

        cat_shop_view.SetState (false);
        cat_shop_view.RefreshNotice ();

        RefreshNoticeEquipments ();

        IsEnabledItemShop     = false;
        IsEnableEquipmentShop = false;
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        GameManager.Instance.DisableTouch ();

        shop_transform.gameObject.SetActive (true);
    }

    public void EnableItemShop ()
    {
        EnableHud ();

        EnableItemsView ();
        DisableUpgradeView ();

        IsEnabledItemShop = true;
    }

    public void EnableEquipmentShop ()
    {
        IsEnableEquipmentShop = true;

        EnableHud ();

        DisableItemsView ();
        EnableUpgradeView ();
        RefreshLanguageEquipments ();

        OnRefreshUiCoins (null);
    }

    private void RefreshInteractEquipments ()
    {
        for (int i = 0; i < equipment_view_list.Count; i++)
        {
            equipment_view_list[i].RefreshInteract ();
        }
    }

    private void RefreshLanguageEquipments ()
    {
        for (int i = 0; i < equipment_view_list.Count; i++)
        {
            equipment_view_list[i].RefreshLanguage ();
        }
    }

    private void RefreshNoticeEquipments ()
    {
        var price      = 0.0;
        var price_unit = 0;

        equipment_view_list[0].GetPrice (out last_price_notice, out last_price_unit_notice);

        for (int i = 1; i < equipment_view_list.Count; i++)
        {
            equipment_view_list[i].GetPrice (out price, out price_unit);

            if (!Helper.IsSmaller (last_price_notice, last_price_unit_notice, price, price_unit))
            {
                last_price_notice      = price;
                last_price_unit_notice = price_unit;
            }
        }

        if (Helper.IsSmaller (PlayerData.Coins, PlayerData.CoinUnit, last_price_notice, last_price_unit_notice))
        {
            is_enabled_notice = true;
        }
        else
        {
            if (Random.Range (0.0f, 1.0f) < 0.5f)
            {
                is_enabled_notice = true;
            }
        }
    }

    #region Action

    public void EnableItemsView ()
    {
        item_transform.gameObject.SetActive (true);

        cat_shop_view.SetState (true);
        cat_shop_view.RefreshEnabled ();
        cat_shop_view.RefreshLanguage ();
    }

    public void DisableItemsView ()
    {
        item_transform.gameObject.SetActive (false);
    }

    public void EnableUpgradeView ()
    {
        upgrade_transform.gameObject.SetActive (true);
    }

    public void DisableUpgradeView ()
    {
        upgrade_transform.gameObject.SetActive (false);
    }

    public void RegisterRefreshUiCoinsEquipments ()
    {
        this.RegisterActionEvent (ActionEnums.ActionID.RefreshUICoins, OnRefreshUiCoins);
    }

    public void UnRegisterRefreshUiCoinsEquipments ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.RefreshUICoins, OnRefreshUiCoins);
    }

    #endregion

    #region Callbacks

    private void OnRefreshUiCoins (object obj)
    {
        if (IsEnableEquipmentShop)
        {
            RefreshInteractEquipments ();
        }

        if (is_enabled_notice)
        {
            if (!Helper.IsSmaller (PlayerData.Coins, PlayerData.CoinUnit, last_price_notice, last_price_unit_notice))
            {
                is_enabled_notice = false;

                UIGameManager.Instance.SetStateNoticeEquipments (true);
            }
        }
    }

    #endregion

    #region Interact

    public void InteractCloseUpgradeShop ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        DisableHud ();
    }

    #endregion
}