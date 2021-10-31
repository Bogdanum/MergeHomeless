using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionReward : MonoBehaviour
{
    [SerializeField] private Image _RewardIcon;
    [SerializeField] private Text  _RewardValue;

    public void SetIcon (Sprite icon)
    {
        _RewardIcon.sprite = icon;
    }

    public void SetValue (string value)
    {
        _RewardValue.text = value;
    }

    public void Disable ()
    {
        gameObject.SetActive (false);
    }

    public void Enable ()
    {
        gameObject.SetActive (true);
    }
}