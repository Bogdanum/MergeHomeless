using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class UIWheelLucky : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformWheelLuckyHud;

    [SerializeField] private Transform _TransformLuckyWheel;

    [SerializeField] private SpriteRenderer[] _ImageIcons;
    [SerializeField] private TextMesh[]       _TextRenderer;

    [SerializeField] private Text _TextNextTime;
    [SerializeField] private Text _TextNumberSpin;

    [SerializeField] private Button spin_button;

    [Header ("Reward")] [SerializeField] private Transform      _TransformReward;
    [SerializeField]                     private SpriteRenderer _SpriteReward;
    [SerializeField]                     private Transform      transform_sprite_reward;
    [SerializeField]                     private Image          _PanelImageReward;
    [SerializeField]                     private Transform      _FxTransformReward;
    [SerializeField]                     private Transform      _TransformTapCloseReward;

    [Header ("Language")] [SerializeField] private Text _LabelWheelLucky;
    [SerializeField]                       private Text _LabelFree, _LabelTapClose;
    [SerializeField]                       private Text _RewerdDescription;    

    [Header ("Fx")] [SerializeField] private ParticleSystem _FxSpin;

    #region Variables

    private bool IsReadyRotation;
    private bool IsReadyRenderer;
    private bool IsReadyToClose;
    private bool is_enable_box_chat;

    private Vector3 _AngleRotation;
    private Vector3 _TargetAngleRotation;

    private float _TargetSpeedRotation;

    #endregion

    #region System

    private void Update ()
    {
        if (!IsReadyRotation)
            return;

        if (_AngleRotation.z < _TargetAngleRotation.z)
        {
            _AngleRotation.z += _TargetSpeedRotation;
        }
        else
        {
            _AngleRotation.z = _TargetAngleRotation.z;
            IsReadyRotation  = false;
        }

        Debug.Log("Попыток - "+ PlayerData._LastNumberTurnSpin);

        IsReadyRenderer = true;
    }

    private void LateUpdate ()
    {
        if (!IsReadyRenderer)
            return;

        _TransformLuckyWheel.localEulerAngles = _AngleRotation;

        IsReadyRenderer = false;

        if (IsReadyRotation == false)
        {
            OnCompletedSpinReward ();
        }
    }

    #endregion

    #region Callback

    private void OnCompletedSpinReward ()
    {
        if (WheelLuckyManager.Instance != null) WheelLuckyManager.Instance.OnCompletedRotation ();
        if (PlayerData._LastNumberTurnSpin == 0)
        {
            spin_button.interactable = false;
        }
        else
        {
            spin_button.interactable = true;
        }
        _FxSpin.Stop ();
    }

    #endregion

    #region Action

    public void InitIcons (WheelLuckyData data)
    {
        for (int i = 0; i < data.GetSize (); i++)
        {
            if (_ImageIcons.Length > i)
            {
                _ImageIcons[i].sprite = data.GetIcon (i);
                _TextRenderer[i].text = ApplicationManager.Instance.AppendFromUnit (data.GetQuantity (i), 0);
            }
            else
            {
                Debug.Log ("The Icons size Is smaller than data size");

                break;
            }
        }
    }

    public void SetRotation (float angle, float speed)
    {
        _TargetAngleRotation.z = angle;
        _TargetSpeedRotation   = speed;
        _AngleRotation.z       = GetAngleRotation ();

        IsReadyRotation = true;

        spin_button.interactable = false;

        _FxSpin.Play ();
    }

    public void SetInteractLuckyButton ()
    {
        spin_button.transform.DOComplete ();
        spin_button.transform.DOPunchScale (new Vector3 (0.1f, 0.1f, 0.1f), Durations.DurationScale);
    }

    public void EnableHud ()
    {
        _AngleRotation = _TransformLuckyWheel.localEulerAngles;
        if (_TransformWheelLuckyHud != null) _TransformWheelLuckyHud.gameObject.SetActive (true);

        RefreshLanguage ();
        RefreshSpinButton ();

        UIGameManager.isSpeenWheel = true;
    }

    private void RefreshSpinButton()
    {
        if (PlayerData._LastNumberTurnSpin > 0) spin_button.interactable = true;
        else spin_button.interactable = false;
    }

    public void RefreshUINumberSpin ()
    {
        _TextNumberSpin.text = string.Format ("{0}/{1}", PlayerData._LastNumberTurnSpin.ToString (), GameConfig.MaxNumberTurnForSpin.ToString ());
    }

    public void RefreshTimeSpin (bool stateEnable)
    {
        if (_TextNextTime.gameObject.activeSelf == !stateEnable) _TextNextTime.gameObject.SetActive (stateEnable);
    }

    public void UpdateTimeNextSpin (string value)
    {
        _TextNextTime.text = value;
    }

    public void EnableAnimationReward (Sprite id)
    {
        if (_TransformReward.gameObject.activeSelf == false) _TransformReward.gameObject.SetActive (true);

        _SpriteReward.sprite = id;

        Timing.RunCoroutine (_AnimationReward ());
    }

    public void GetDescriptionReward (string description)
    {
        _RewerdDescription.text = description;
    }

    public void RefreshLanguage ()
    {
        RefreshLanguageSpin ();

        _LabelWheelLucky.text = ApplicationLanguage.Text_label_spin_and_win;
        _LabelTapClose.text   = ApplicationLanguage.Text_label_tap_to_close;
    }

    public void RefreshLanguageSpin ()
    {
        if (PlayerData.IsWatchAdsForSpin)
        {
            _LabelFree.text = ApplicationLanguage.Text_label_let_spin;
        }
        else
        {
            _LabelFree.text = ApplicationLanguage.Text_label_watch_to_spin;
        }
    }

    #endregion

    #region IEnumerator

    private IEnumerator<float> _AnimationReward ()
    {
        _PanelImageReward.DOComplete ();
        transform_sprite_reward.DOComplete ();
        _FxTransformReward.DOComplete ();
        _TransformTapCloseReward.DOComplete ();

        var panel_color = _PanelImageReward.color;
        var fx_scale    = _FxTransformReward.localScale;

        // =============================== Prepare animation ================================ //

        _PanelImageReward.color                       = new Color (panel_color.r, panel_color.g, panel_color.b, 0);
        transform_sprite_reward.localScale            = Vector.Vector3Zero;
        _TransformTapCloseReward.transform.localScale = Vector.Vector3Zero;
        _FxTransformReward.localScale                 = Vector.Vector3Zero;

        // =============================== Do animation ================================ //

        _PanelImageReward.DOFade (panel_color.a, Durations.DurationFade);

        yield return Timing.WaitForSeconds (Durations.DurationFade);

        this.PlayAudioSound (AudioEnums.SoundId.LuckyRewardAppear);

        transform_sprite_reward.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);
        _FxTransformReward.transform.DOScale (fx_scale, Durations.DurationScale).SetEase (Ease.OutBack);

        yield return Timing.WaitForSeconds (1f);

        _TransformTapCloseReward.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        WheelLuckyManager.Instance.IsReadyToGetReward = true;

        yield break;
    }

    #endregion

    #region Interface Interact

    public void Close ()
    {
        if (_TransformWheelLuckyHud != null) _TransformWheelLuckyHud.gameObject.SetActive (false);
        UIGameManager.isSpeenWheel = false;
    }

    public void CloseReward ()
    {
        if (_TransformReward.gameObject.activeSelf == true) _TransformReward.gameObject.SetActive (false);
    }

    #endregion

    #region Helper

    public float GetAngleRotation ()
    {
        return _AngleRotation.z % 360;
    }

    #endregion
}