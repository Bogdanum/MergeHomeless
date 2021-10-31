using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiamondItemView : MonoBehaviour
{
    [SerializeField] private IAPData _IapData;

    [SerializeField] private Text _TextPrices;
    [SerializeField] private Text _TextValue;

    [SerializeField] private Text      _TextSalePercents;
    [SerializeField] private Transform _TransformSale;

    private bool IsLocked;

    public void Init ()
    {
        RefreshPrice ();

        _TextValue.text        = _IapData.Value.ToString ();
        _TextSalePercents.text = _IapData.DescriptionSalePercent;

        _TransformSale.gameObject.SetActive (!string.IsNullOrEmpty (_IapData.DescriptionSalePercent));
    }

    public void RefreshPrice ()
    {
        if (_IapData.id == IapEnums.IapId.FreePack)
        {
            _TextPrices.text = _IapData.PriceOffline;
        }
        else
        {
            _TextPrices.text = IapManager.Instance.ReturnThePrice (_IapData.id);
        }
    }

    public void SetStateUnLock (bool state)
    {
        IsLocked = !state;
     
    }

    public void DoBuy ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (IsLocked)
            return;
        
        switch (_IapData.TypeIap)
        {
            case IapEnums.TypeIap.FreeAds:

                if (this.IsRewardVideoAvailable ())
                {
                    this.ExecuteRewardAds (() =>
                    {
                        if (GameActionManager.Instance != null)
                        {
                            GameActionManager.Instance.InstanceFxDiamonds (Vector.Vector3Zero,
                                                                           UIGameManager.Instance.GetPositionHubDiamonds (),
                                                                           _IapData.Value);
                        }
                        else
                        {
                            PlayerData.Diamonds += _IapData.Value;
                            PlayerData.SaveDiamonds ();

                            this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
                        }

                        PlayerData._LastTimeWatchAdsForFreeDiamonds = Helper.GetUtcTimeString ();
                        PlayerData.SaveLastTimeWatchAdsForFreeDiamonds ();

                        if (DiamondManager.Instance != null)
                        {
                            DiamondManager.Instance.RefreshTime ();
                        }

                    }, null);
                }
                else
                {
                    this.RefreshRewardVideo ();
                }

                break;
            case IapEnums.TypeIap.Consumable:
            case IapEnums.TypeIap.NonConsumable:
                IapManager.Instance.BuyProductWithID (_IapData.IapId);
                break;
        }
    }
}