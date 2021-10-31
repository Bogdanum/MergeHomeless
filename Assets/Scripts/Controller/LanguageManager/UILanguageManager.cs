using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILanguageManager : Singleton<UILanguageManager>, IDialog
{
    [Header ("Data")] [SerializeField] private LanguageIcon _LanguageIcon;

    [Header ("References")] [SerializeField]
    private Transform _TransformHub;

    [SerializeField] private Transform _GroupButtonLanguage;
    [SerializeField] private Button    _TransformButtonLanguage;

    [Header ("Language")] [SerializeField] private Text _LabelLanguage;

    private List<Button> _ListLanguages;

    private int InstanceId;

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitButton ();

    }

    #endregion

    #region Callback

    private void RefreshLanguage ()
    {
        _LabelLanguage.text = ApplicationLanguage.Text_label_language;
    }

    #endregion
    
    #region Controller

    public void InitButton ()
    {
        _ListLanguages = new List<Button> ();

        var support_language = ApplicationLanguage.Instance.GetSupportLanguage ();

        for (int i = 0; i < support_language.Count; i++)
        {
            var prefab = Instantiate (_TransformButtonLanguage.gameObject, _GroupButtonLanguage);

            var item = prefab.GetComponent<Button> ();

            if (item == null)
                continue;

            string language_code = support_language[i].languageCode;
            int    instanceId    = item.GetInstanceID ();

            item.image.sprite = _LanguageIcon.GetIcon (LanguageEnums.GetLanguageId (language_code));

            item.onClick.AddListener (() =>
            {
                ApplicationLanguage.Instance.ChangeLanguage (language_code);
                ApplicationLanguage.Instance.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

                Instance.InstanceId = instanceId;
                Instance.RefreshItem ();
                Instance.RefreshLanguage ();
                LboardTranslateElements.Instance.RefreshLanguage();
            });

            _ListLanguages.Add (item);

            if (string.CompareOrdinal (PlayerData.DefaultLanguage, language_code) == 0)
            {
                InstanceId = instanceId;
            }
        }
        
        _TransformButtonLanguage.gameObject.SetActive (false);
         
        RefreshItem ();
    }

    public void RefreshItem ()
    {
        for (int i = 0; i < _ListLanguages.Count; i++)
        {
            var item = _ListLanguages[i];

            if (item.GetInstanceID () == InstanceId)
            {
                item.interactable = false;
            }
            else
            {
                item.interactable = true;
            }
        }
    }
    #endregion

    #region Action

    public void DisableHud ()
    {
        _TransformHub.gameObject.SetActive (false);

        GameManager.Instance.EnableTouch ();
     
        ApplicationManager.Instance.UnSetDialog (this);
    }

    public void EnableHud ()
    {
        ApplicationManager.Instance.SetDialog (this);

        _TransformHub.gameObject.SetActive (true);

        GameManager.Instance.DisableTouch ();
         
        RefreshLanguage ();
    }

    #endregion

    #region Interaction

    public void Close ()
    {
        DisableHud ();

        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
    }

    #endregion
}