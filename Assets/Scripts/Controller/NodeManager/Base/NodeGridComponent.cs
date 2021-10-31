using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGridComponent : NodeComponent, INodeGrid
{
    [Header ("Data")] [SerializeField] private ItemNodeImage _ItemNodeImage;

    [Header ("Renderer")] [SerializeField] private SpriteRenderer sprite_renderer;
    
    [Header ("Transform Icon")] [SerializeField]
    private Transform _TransformIconBack, _TransformOutlineSelect;

    [Header ("Lock")] [SerializeField] private Transform transform_lock;
    [SerializeField]                   private TextMesh  label_unlock_price;

    [Header ("Level")] [SerializeField] private SpriteRenderer transform_level_group;

    private int _InstanceId;
    private int _Level;

    private bool _IsBusy;
    private bool _IsActive;

    public void EnableIconItem (int level, string key)
    {
        sprite_renderer.sprite = _ItemNodeImage.GetIcon (level);
        sprite_renderer.color = new Color (1, 1, 1, 0.4f);

        sprite_renderer.enabled = true;
    }

    public void DisableIconItem ()
    {
        sprite_renderer.enabled = false;
    }

    public void EnableIconBack ()
    {
        if (_TransformIconBack != null && _TransformIconBack.gameObject.activeSelf == false) _TransformIconBack.gameObject.SetActive (true);

        _IsBusy = true;
    }

    public void DisableIconBack ()
    {
        if (_TransformIconBack != null && _TransformIconBack.gameObject.activeSelf == true) _TransformIconBack.gameObject.SetActive (false);

        _IsBusy = false;
    }

    public void EnableLevel (int level)
    {
        if (transform_level_group.enabled == false)
            transform_level_group.enabled = true;

        transform_level_group.sprite = GameData.Instance.NumberIcon.GetIcon (level);
    }

    public void DisableLevel ()
    {
        if (transform_level_group.enabled == true)
            transform_level_group.enabled = false;
    }

    public void EnableOutline (int instanceId, int level)
    {
        if (_Level == level && _InstanceId != instanceId && _IsBusy == false && _IsActive == true)
        {
            if (_TransformOutlineSelect != null && _TransformOutlineSelect.gameObject.activeSelf == false)
                _TransformOutlineSelect.gameObject.SetActive (true);
        }
    }

    public void DisableOutline ()
    {
        if (_TransformOutlineSelect != null && _TransformOutlineSelect.gameObject.activeSelf == true)
            _TransformOutlineSelect.gameObject.SetActive (false);
    }

    public void SetInfo (int instanceId, int level)
    {
        _InstanceId = instanceId;
        _Level      = level;
    }

    public void SetState (bool isActive)
    {
        _IsActive = isActive;
    }

    public void Enable ()
    {
        if (gameObject.activeSelf == false) gameObject.SetActive (true);
    }

    public void Disable ()
    {
        if (gameObject.activeSelf) gameObject.SetActive (false);
    }

    public void UpdateLockPrice (string price)
    {
        label_unlock_price.text = price;
    }

    public void EnableLockState ()
    {
        transform_lock.gameObject.SetActive (true);
    }

    public void DisableLockState ()
    {
        transform_lock.gameObject.SetActive (false);
    }
}