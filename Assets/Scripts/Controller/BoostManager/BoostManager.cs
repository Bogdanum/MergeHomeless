using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostManager : Singleton<BoostManager>, IDialog
{
    [Header ("Hub")] [SerializeField] private Transform transform_hub;
    [SerializeField]                  private Transform transform_grid_content;

    #region Variables

    private List<BoostProperty> boost_properties;

    #endregion
    
    #region System

    protected override void Awake ()
    {
        base.Awake ();

        Init ();
    }

    #endregion

    #region Controller

    private void Init ()
    {
        boost_properties = new List<BoostProperty> ();
        
        var size = GameData.Instance.BoostData.GetSize ();

        for (int i = 0; i < size; i++)
        {
            var data = GameData.Instance.BoostData.GetBoost (i);

            var item = PoolExtension.GetPool (PoolEnums.PoolId.InteractBoostView, false);

            if (item == null)
                continue;
            
            var script = item.GetComponent<BoostProperty> ();
            
            script.Init (data);

            item.SetParent (transform_grid_content);
            item.localScale    = Vector3.one;
            item.localPosition = Vector3.zero;

            item.gameObject.SetActive (true);
            
            boost_properties.Add (script);
        }
    }

    #endregion

    #region Action

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);
        transform_hub.gameObject.SetActive (true);
        GameManager.Instance.DisableTouch ();

        RefreshBoost ();
        RefreshLanguage ();
    }

    public void DisableHud ()
    {
        GameManager.Instance.EnableTouch ();

        transform_hub.gameObject.SetActive (false);
        ApplicationManager.Instance.UnSetDialog (this);
    }

    public void RefreshBoost ()
    {
        for (int i = 0; i < boost_properties.Count; i++)
        {
            boost_properties[i].RefreshCash ();
        }
    }

    public void RefreshLanguage ()
    {
        for (int i = 0; i < boost_properties.Count; i++)
        {
            boost_properties[i].RefreshLanguage ();
        }
    }

    #endregion

    #region Interact

    public void InteractClose ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
        
        DisableHud ();
    }

    #endregion
}