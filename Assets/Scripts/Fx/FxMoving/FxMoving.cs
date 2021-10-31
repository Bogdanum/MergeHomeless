using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxMoving : MonoBehaviour
{
    [Header ("Config")] [SerializeField] private Vector3 _PositionStart;

    [SerializeField] private Vector3 _PositionEnd,
                                     _OffsetRandomUp,
                                     _OffsetRandomDown;

    private new Transform transform;
    private     Vector3   _TransformPosition;

    [Header ("Direction")] [SerializeField]
    private bool _Direction;

    private float _Speed;

    public event System.Action OnMovingEnd;

    public void Awake ()
    {
        transform = gameObject.transform;
    }

    private void OnEnable ()
    {
        _TransformPosition = transform.localPosition;


        if (Random.Range (0, 2) == 0)
        {
            _TransformPosition.x = _PositionStart.x;

            _Direction = true;
        }
        else
        {
            _TransformPosition.x = _PositionEnd.x;

            _Direction = false;
        }

        _TransformPosition.z = 0;

        RefreshSpeed ();
        RefreshY ();

        SetFlip (_Direction);
        SetPosition (_TransformPosition);
    }

    private void SetPosition (Vector3 position)
    {
        transform.localPosition = position;
    }

    private void RefreshSpeed ()
    {
        _Speed = Random.Range (0.8f, 1f);
    }

    private void RefreshY ()
    {
        _TransformPosition.y = _PositionStart.y + Random.Range (-_OffsetRandomDown.y, _OffsetRandomUp.y);
    }

    private void SetFlip (bool Direction)
    {
        if (Direction)
        {
            transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);
        }
        else
        {
            transform.eulerAngles = new Vector3 (0, 180, transform.eulerAngles.z);
        }
    }


    private void Update ()
    {
        if (_Direction)
        {
            _TransformPosition.x += Time.deltaTime * _Speed;

            if (_TransformPosition.x < _PositionEnd.x) return;

            _Direction = !_Direction;

            RefreshY ();

            SetFlip (_Direction);
            RefreshSpeed ();

            if (OnMovingEnd != null)
            {
                OnMovingEnd ();
            }
        }
        else
        {
            _TransformPosition.x -= Time.deltaTime * _Speed;

            if (_TransformPosition.x > _PositionStart.x) return;

            _Direction = !_Direction;

            RefreshY ();

            SetFlip (_Direction);
            RefreshSpeed ();

            if (OnMovingEnd != null)
            {
                OnMovingEnd ();
            }
        }
    }

    private void LateUpdate ()
    {
        SetPosition (_TransformPosition);
    }
}