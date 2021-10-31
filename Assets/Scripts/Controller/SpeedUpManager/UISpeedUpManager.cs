using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedUpManager : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformSpeedUpHud;

    [SerializeField] private Text _TextFree,
                                  _TextDiamonds,
                                  _TextDescription;

    [Header("Button")] [SerializeField] private Button _SpeedUpButton;

    [Header ("Time Process")] [SerializeField]
    private Image _ImageProcessTime;

    [SerializeField] private Text _TextProcessTime;

    [Header ("Language")] [SerializeField] private Text _DescriptionAddSeconds;

    [SerializeField] private Text _LabelSpeedUp,
                                  _LabelFree,
                                  _DescriptionSpeedIn;


    #region Variables

    public event System.Action<bool>   HandleStateSpeedUpHub;
    public event System.Action<string> HandleStateSpeedUpTime;

    private bool IsEnable;

    #endregion
    #region System
    public void Update()
    {
        if (PlayerData.TotalTimeSpeedUp <= 330)
        {
            _SpeedUpButton.interactable = true;
        }
        else
        {
            _SpeedUpButton.interactable = false;
        }
    }
    #endregion
    #region Helper

    public Vector3 GetPositionDiamonds ()
    {
        return _TextDiamonds.transform.position;
    }

    #endregion
    #region Action

    public void Init ()
    {
        if (_TextFree != null) _TextFree.text               = "Free";
        if (_TextDiamonds != null) _TextDiamonds.text       = GameConfig._DiamondBuySpeedUp.ToString ();
        if (_TextDescription != null) _TextDescription.text = string.Format ("Add another {0} seconds", GameConfig.TimeSpeedUp.ToString ());
    }

    public void EnableHub ()
    {
        if (_TransformSpeedUpHud != null) _TransformSpeedUpHud.gameObject.SetActive (true);

        IsEnable = true;

        RefreshLanguage ();
    }

    public void DisableHub ()
    {
        if (_TransformSpeedUpHud != null) _TransformSpeedUpHud.gameObject.SetActive (false);

        IsEnable = false;
    }

    public void UpdateTimeProcess ()
    {
        var stringSpeedTime = Helper.ConvertToTime (PlayerData.TotalTimeSpeedUp);
        
        if (IsEnable)
        {
            if (_ImageProcessTime != null) _ImageProcessTime.fillAmount = PlayerData.TotalTimeSpeedUp / GameConfig.MaxTimeSpeedUp;
            if (_TextProcessTime != null) _TextProcessTime.text         = stringSpeedTime;
        }

        if (!ReferenceEquals (HandleStateSpeedUpTime, null)) HandleStateSpeedUpTime (stringSpeedTime);
    }

    public void EnableTimeProcess ()
    {
        if (!ReferenceEquals (HandleStateSpeedUpHub, null))
        {
            HandleStateSpeedUpHub (true);
        }
    }

    public void DisableTimeProcess ()
    {
        if (!ReferenceEquals (HandleStateSpeedUpHub, null))
        {
            HandleStateSpeedUpHub (false);
        }
    }

    public void RefreshLanguage ()
    {
        _DescriptionAddSeconds.text = string.Format (ApplicationLanguage.Text_description_add_seconds, GameConfig.TimeSpeedUp.ToString ());
        _LabelFree.text             = ApplicationLanguage.Text_label_free;
        _LabelSpeedUp.text          = ApplicationLanguage.Text_label_speed_up;
        _DescriptionSpeedIn.text    = string.Format (ApplicationLanguage.Text_description_speed_in, 2);
    }
    #endregion

    #region Interface Interact

    public void Close ()
    {
        SpeedUpManager.Instance.DisableHud ();
    }

    public void FreeSpeedUp ()
    {
        SpeedUpManager.Instance.FreeSpeedUp ();
    }

    public void BuySpeedUpDiamonds ()
    {
        SpeedUpManager.Instance.BuySpeedUpDiamonds();
    }

    #endregion
}