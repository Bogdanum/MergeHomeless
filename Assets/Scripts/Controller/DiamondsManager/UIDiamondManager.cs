using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDiamondManager : MonoBehaviour
{
    [SerializeField] private Transform _TransformHud;

    [SerializeField] private DiamondItemView[] _ItemViews;
    [SerializeField] private DiamondItemView   _ItemViewFree;
    [SerializeField] private Text              _TextNextTimeViewFree;

    [Header ("Language")] [SerializeField] private Text _LabelDiamondShop;
    

    #region Action

    public void Init ()
    {
        for (int i = 0; i < _ItemViews.Length; i++)
        {
            _ItemViews[i].Init ();
            _ItemViews[i].SetStateUnLock (true);
        }

        _ItemViewFree.Init ();
        _ItemViewFree.SetStateUnLock (true);
    }

    public void UpdateTime (string time)
    {
        _TextNextTimeViewFree.text = time;
    }

    public void UpdateState (bool is_unlocked)
    {
        _ItemViewFree.SetStateUnLock (is_unlocked);

        if (is_unlocked)
        {
            _TextNextTimeViewFree.text = ApplicationLanguage.Text_label_watch_now;
        }
    }

    public void UpdateRemoveAds ()
    {
      
    }

    public void EnableHud ()
    {
        if (_TransformHud != null) _TransformHud.gameObject.SetActive (true);

        UpdateRemoveAds ();
        RefreshLanguage ();
        RefreshPriceText ();
    }

    public void DisableHud ()
    {
        if (_TransformHud != null) _TransformHud.gameObject.SetActive (false);
    }

    public void RefreshLanguage ()
    {
        _LabelDiamondShop.text = ApplicationLanguage.Text_label_diamond_shop;
    }

    public void RefreshPriceText ()
    {
        for (int i = 0; i < _ItemViews.Length; i++)
        {
            _ItemViews[i].RefreshPrice ();
        }
    }
    #endregion
}