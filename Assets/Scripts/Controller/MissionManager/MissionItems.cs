using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionItems : MonoBehaviour
{
    [SerializeField] private Image _ProcessMission;
    [SerializeField] private Text  _ValueMission;

    [SerializeField] private Image _MissionIcon;
    [SerializeField] private Text  _DescriptionMission;

    [SerializeField] private Transform _TransformReward;

    private List<MissionReward> _MissionRewards;

    private MissionEnums.MissionId _MissionId;

    private int _Quantity;
    private int _QuantityTarget;

    private bool IsCompletedMission;

    #region System

    public void Awake ()
    {
        _MissionRewards = new List<MissionReward> ();
    }

    #endregion

    #region Action

    public MissionItems SetQuantityTarget (int quantity)
    {
        _QuantityTarget = quantity;
        return this;
    }

    public MissionItems SetDescription (string description)
    {
        _DescriptionMission.text = description;
        return this;
    }

    public MissionItems SetIcon (Sprite icon)
    {
        _MissionIcon.sprite = icon;
        return this;
    }

    public MissionItems SetProcess (int current)
    {
        _Quantity = current;

        _ProcessMission.fillAmount = (float) current / _QuantityTarget;
        _ValueMission.text         = string.Format ("{0}/{1}", current.ToString (), _QuantityTarget.ToString ());

        return this;
    }

    public MissionItems SetReward (MissionData.MissionProperty data)
    {
        for (int i = _MissionRewards.Count; i < data._MissionReward.Length; i++)
        {
            var reward = PoolExtension.GetPool (PoolEnums.PoolId.Reward_Layout, false);

            reward.SetParent (_TransformReward);

            reward.localPosition = Vector.Vector3Zero;
            reward.localScale    = Vector.Vector3One;

            _MissionRewards.Add (reward.GetComponent<MissionReward> ());
        }

        for (int i = 0; i < _MissionRewards.Count; i++)
        {
            _MissionRewards[i].Disable ();
        }

        for (int i = 0; i < data._MissionReward.Length; i++)
        {
            var reward      = data._MissionReward[i];
            var reward_item = _MissionRewards[i];

            reward_item.SetValue (ApplicationManager.Instance.AppendFromUnit (reward.Value, 0));
            reward_item.SetIcon (ApplicationManager.Instance.GetIconReward (reward.RewardId));
            reward_item.Enable ();
        }

        return this;
    }

    public MissionItems SetId (MissionEnums.MissionId missionId)
    {
        _MissionId = missionId;

        return this;
    }

    public MissionItems RefreshStatus ()
    {
        if (_Quantity == _QuantityTarget && !IsCompletedMission)
        {
            IsCompletedMission = true;

            this.PostActionEvent (ActionEnums.ActionID.RefreshUICompleteMission);

            if (MessageManager.InstanceAwake () != null)
            {
                MessageManager.Instance.ShowNotice (ApplicationLanguage.Text_completed_mission, _MissionIcon.sprite);
            }

            transform.SetAsFirstSibling ();
        }
        else if (_Quantity != _QuantityTarget)
        {
            IsCompletedMission = false;
        }

        return this;
    }

    public void Lock () { }

    public void Unlock () { }

    public void Disable ()
    {
        gameObject.SetActive (false);
    }

    public void Enable ()
    {
        gameObject.SetActive (true);
    }

    #endregion

    #region Helper

    public MissionEnums.MissionId GetId ()
    {
        return _MissionId;
    }

    public int GetIntId ()
    {
        return (int) _MissionId;
    }

    #endregion

    #region Interaction

    public void OnTouchReceiveReward ()
    {
        this.PlayAudioSound (AudioEnums.SoundId.TapOnButton);

        if (!IsCompletedMission)
        {
            ApplicationManager.Instance.AlertCompletedMission ();

            return;
        }


        this.PlayAudioSound (AudioEnums.SoundId.ClaimReward);

        MissionManager.Instance.OnGetReward (_MissionId);

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICompleteMission);
    }

    #endregion
}