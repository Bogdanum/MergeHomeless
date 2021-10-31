using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UISettingManager : MonoBehaviour
{
    [SerializeField] private Transform _TransformSettingHud;

    [Header ("Fanpage")] [SerializeField] private string _TwitterFanpage;
    [SerializeField]                      private string _FacebookFanpage;
    [SerializeField]                      private string _SiteURL;

    [Header ("Stores")] [SerializeField] private string _GooglePlayURL;
                        [SerializeField] private string _iosAppID;

    [Header ("UI")] [SerializeField] private Button _RestorePurchaseBtn;

    [SerializeField] private RectTransform _TransformSound,
                                           _TransformMusic,
                                           _TransformNotification,
                                           _TransformBatterySaver,
                                           _TransformPanelSound,
                                           _TransformPanelMusic,
                                           _TransformPanelNotification,
                                           _TransformPanelBatterySaver;

    [SerializeField] private Image image_sound_button,
                                   image_music_button,
                                   image_battery_button,
                                   image_notification_button;

    [SerializeField] private Sprite _SpriteActiveButton,
                                    _SpriteDeActiveButton;

    [Header ("Language")] [SerializeField] private Text _LabelSettings;

    [SerializeField] private Text _LabelLanguage,
                                  _LabelRestorePurchase,
                                  _LabelMusic,
                                  _LabelSound;

    [SerializeField] private Text label_battery_saver,
                                  label_notification;

    [SerializeField] private Text label_description_follow;
    [SerializeField] private Text label_version;

    [SerializeField] private Text label_rate_us;
    
    [Header ("Data")] [SerializeField] private LanguageIcon _LanguageIcon;

    [SerializeField] private Image _IconLanguage;

    public static string GooglePlayURL;
    public static string iosAppID;

    private void Awake()
    {
        GooglePlayURL = _GooglePlayURL;
        iosAppID = _iosAppID;
    }

    public void EnableHud ()
    {
        _TransformSettingHud.gameObject.SetActive (true);

        if (PlayerData.IsRemoveAds)
        {
            _RestorePurchaseBtn.interactable = false;
        }
        else
        {
            _RestorePurchaseBtn.interactable = true;
        }

        #if UNITY_ANDROID

        _RestorePurchaseBtn.interactable = false;

        #endif

        RefreshMusicUI (false);
        RefreshSoundUI (false);
        RefreshBatterySaver (false);
        RefreshNotificationUI (false);

        _IconLanguage.sprite = _LanguageIcon.GetIcon (LanguageEnums.GetLanguageId (PlayerData.DefaultLanguage));

        RefreshLanguage ();
    }

    public void DisableHud ()
    {
        _TransformSettingHud.gameObject.SetActive (false);
    }

    private void RefreshLanguage ()
    {
        _LabelLanguage.text        = ApplicationLanguage.Text_label_language;
        _LabelMusic.text           = ApplicationLanguage.Text_label_music;
        _LabelSettings.text        = ApplicationLanguage.Text_label_settings;
        _LabelSound.text           = ApplicationLanguage.Text_label_sound;
        _LabelRestorePurchase.text = ApplicationLanguage.Text_label_restore_purchase;

        label_description_follow.text = ApplicationLanguage.Text_description_follow_and_talk_us;

        label_battery_saver.text = ApplicationLanguage.Text_label_battery_saver;
        label_notification.text  = ApplicationLanguage.Text_label_notification;

        label_version.text = string.Format (GameConfig.VersionLabel, Version.version);

        label_rate_us.text = ApplicationLanguage.Text_label_rate_me;
    }

    private void RefreshSoundUI (bool isAnimation = true)
    {
        _TransformSound.DOComplete ();

        if (Contains.IsSoundOn)
        {
            if (isAnimation)
            {
                _TransformSound.DOLocalMoveX (_TransformPanelSound.sizeDelta.x / 2 - _TransformSound.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformSound.localPosition = new Vector3 (_TransformPanelSound.sizeDelta.x / 2 - _TransformSound.sizeDelta.x / 2, 0, 0);
            }

            image_sound_button.sprite = _SpriteActiveButton;
        }
        else
        {
            if (isAnimation)
            {
                _TransformSound.DOLocalMoveX (-_TransformPanelSound.sizeDelta.x / 2 + _TransformSound.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformSound.localPosition = new Vector3 (-_TransformPanelSound.sizeDelta.x / 2 + _TransformSound.sizeDelta.x / 2, 0, 0);
            }

            image_sound_button.sprite = _SpriteDeActiveButton;
        }
    }

    private void RefreshMusicUI (bool isAnimation = true)
    {
        _TransformMusic.DOComplete ();

        if (Contains.IsMusicOn)
        {
            if (isAnimation)
            {
                _TransformMusic.DOLocalMoveX (_TransformPanelMusic.sizeDelta.x / 2 - _TransformMusic.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformMusic.localPosition = new Vector3 (_TransformPanelMusic.sizeDelta.x / 2 - _TransformMusic.sizeDelta.x / 2, 0, 0);
            }

            image_music_button.sprite = _SpriteActiveButton;
        }
        else
        {
            if (isAnimation)
            {
                _TransformMusic.DOLocalMoveX (-_TransformPanelMusic.sizeDelta.x / 2 + _TransformMusic.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformMusic.localPosition = new Vector3 (-_TransformPanelMusic.sizeDelta.x / 2 + _TransformMusic.sizeDelta.x / 2, 0, 0);
            }

            image_music_button.sprite = _SpriteDeActiveButton;
        }
    }

    public void RefreshNotificationUI (bool isAnimation = true)
    {
        _TransformNotification.DOComplete ();

        if (Contains.IsNotificationOn)
        {
            if (isAnimation)
            {
                _TransformNotification.DOLocalMoveX (_TransformPanelNotification.sizeDelta.x / 2 - _TransformNotification.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformNotification.localPosition = new Vector3 (_TransformPanelNotification.sizeDelta.x / 2 - _TransformNotification.sizeDelta.x / 2, 0, 0);
            }

            image_notification_button.sprite = _SpriteActiveButton;
        }
        else
        {
            if (isAnimation)
            {
                _TransformNotification.DOLocalMoveX (-_TransformPanelNotification.sizeDelta.x / 2 + _TransformNotification.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformNotification.localPosition = new Vector3 (-_TransformPanelNotification.sizeDelta.x / 2 + _TransformNotification.sizeDelta.x / 2, 0, 0);
            }

            image_notification_button.sprite = _SpriteDeActiveButton;
        }
    }

    public void RefreshBatterySaver (bool isAnimation = true)
    {
        _TransformBatterySaver.DOComplete ();

        if (Contains.IsBatteryOn)
        {
            if (isAnimation)
            {
                _TransformBatterySaver.DOLocalMoveX (_TransformPanelBatterySaver.sizeDelta.x / 2 - _TransformBatterySaver.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformBatterySaver.localPosition = new Vector3 (_TransformPanelBatterySaver.sizeDelta.x / 2 - _TransformBatterySaver.sizeDelta.x / 2, 0, 0);
            }

            image_battery_button.sprite = _SpriteActiveButton;
        }
        else
        {
            if (isAnimation)
            {
                _TransformBatterySaver.DOLocalMoveX (-_TransformPanelBatterySaver.sizeDelta.x / 2 + _TransformBatterySaver.sizeDelta.x / 2, Durations.DurationMoving);
            }
            else
            {
                _TransformBatterySaver.localPosition = new Vector3 (-_TransformPanelBatterySaver.sizeDelta.x / 2 + _TransformBatterySaver.sizeDelta.x / 2, 0, 0);
            }

            image_battery_button.sprite = _SpriteDeActiveButton;
        }
    }

    #region Interact

    public void OpenLanguage ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== Open the language system ================================ //

        if (UILanguageManager.Instance != null) UILanguageManager.Instance.EnableHud ();
    }

    public void CloseHud ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        SettingManager.Instance.DisableHud ();
    }

    public void SetSound ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        Contains.IsSoundOn = !Contains.IsSoundOn;

        RefreshSoundUI ();

        SoundManagerExtension.SetStateSound (Contains.IsSoundOn);
    }

    public void SetMusic ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        Contains.IsMusicOn = !Contains.IsMusicOn;

        RefreshMusicUI ();

        SoundManagerExtension.SetStateMusic (Contains.IsMusicOn);
    }

    public void OpenTwitterFanpage ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        Application.OpenURL (_TwitterFanpage);
    }

    public void OpenFacebookFanpage ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        Application.OpenURL (_FacebookFanpage);
    }

    public void OpenDeliciousGames()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        Application.OpenURL (_SiteURL);
    }

    public void RestorePurchase ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        IapManager.Instance.RestorePurchases ();
    }

    public void InteractSaveBattery ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== Save the battery ================================ //

        Contains.IsBatteryOn = !Contains.IsBatteryOn;

        if (Contains.IsBatteryOn)
        {
            Application.targetFrameRate = 30;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        RefreshBatterySaver (true);
    }

    public void InteractNotification ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        // =============================== Enable the notification ================================ //

        Contains.IsNotificationOn = !Contains.IsNotificationOn;

        RefreshNotificationUI (true);
    }

    public void InteractRateUs ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);
        
        // =============================== Rate Us ================================ //
        
        ApplicationManager.Instance.OpenUrlStore ();
    }
    #endregion
}