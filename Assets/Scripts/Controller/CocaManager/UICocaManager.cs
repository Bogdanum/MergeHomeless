using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICocaManager : MonoBehaviour
{
    [Header ("UI")] [SerializeField] private Transform _TransformHud;

    [Header ("UI")] [SerializeField] private InputField _IPValueExchange;

    [Header ("Language")] [SerializeField] private Text _LabelCurrencyExchange;

    [SerializeField] private Text _LabelExchange,
                                  _LabelMax,
                                  _LabelMin;

    #region Action

    public void Init () { }

    public void EnableHud ()
    {
        if (_TransformHud != null) _TransformHud.gameObject.SetActive (true);

        RefreshLanguage ();
    }

    public void UpdateTextExchangeValue (string text, Color color)
    {
        if (_IPValueExchange != null)
        {
            _IPValueExchange.text                = text;
            _IPValueExchange.textComponent.color = color;
        }
    }

    public void RefreshLanguage ()
    {
        _LabelCurrencyExchange.text = ApplicationLanguage.Text_label_currency_exchange;
        _LabelExchange.text         = ApplicationLanguage.Text_label_exchange;
        _LabelMax.text              = ApplicationLanguage.Text_label_max;
        _LabelMin.text              = ApplicationLanguage.Text_label_min;
    }

    #endregion

    #region Interface Interact

    public void Close ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (_TransformHud != null) _TransformHud.gameObject.SetActive (false);
    }

    public void Minus ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (CocaManager.Instance != null) CocaManager.Instance.MinusValue ();
    }

    public void Plus ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (CocaManager.Instance != null) CocaManager.Instance.PlusValue ();
    }

    public void Max ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (CocaManager.Instance != null) CocaManager.Instance.MaxValue ();
    }

    public void Min ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (CocaManager.Instance != null) CocaManager.Instance.MinValue ();
    }

    public void Exchange ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (CocaManager.Instance != null) CocaManager.Instance.ExchangeValue ();
    }

    #endregion
}