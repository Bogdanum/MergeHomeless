using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoxBehaviour : NodeComponent
{
    private bool IsReady;

    [SerializeField] private SpriteRenderer _SpriteBox;
    [SerializeField] private BoxData        _BoxData;

    public override NodeComponent TouchBusy ()
    {
        UnBox ();

        return this;
    }

    public override NodeComponent SetPosition (Vector3 position)
    {
        IsReady = false;

        transform.position = new Vector3 (position.x, position.y + 3, 0);
        transform.DOComplete (true);
        transform.DOMoveY (position.y, Durations.DurationDrop).SetEase (Ease.OutBack).OnComplete (() =>
        {
            IsReady = true;
            this.PlayAudioSound (AudioEnums.SoundId.BoxAppear);
        });

        return this;
    }

    public void SetIcon (BoxEnums.BoxId id)
    {
        _SpriteBox.sprite = _BoxData.GetIcon (id);
    }

    private void UnBox ()
    {
        if (!IsReady) return;

        this.PlayAudioSound (AudioEnums.SoundId.BoxOpen);

        GameManager.Instance.SetBaseItemGrid (_ItemNodeData,
                                              GetIndexX (),
                                              GetIndexY ());

        PoolExtension.SetPool (GetPoolId (), transform);

        GameActionManager.Instance.InstanceFxTapBox (transform.position);
        GameActionManager.Instance.InstanceFxFireWork (transform.position);

        this.PostMissionEvent(MissionEnums.MissionId.TapOnBox);
    }
}