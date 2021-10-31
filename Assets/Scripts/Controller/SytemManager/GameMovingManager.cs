using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameMovingManager : Singleton<GameMovingManager>
{
    [Header ("Point")] [SerializeField] private Transform[] _TransformsPathMoving;

    [SerializeField] private int       _StartIndexPoint;
    [SerializeField] private Transform transform_earth;
    [SerializeField] private Transform transform_planet;
    [SerializeField] private Transform transform_moon;

    #region Variables

    private readonly List<Tweener>      _TweenObject   = new List<Tweener> ();
    private readonly List<IMoving>      _IMovingObject = new List<IMoving> ();
    private readonly List<ItemNodeData> _INodeData     = new List<ItemNodeData> ();
    private          Vector3[]          _PositionPath;

    private Vector3 _Direction;

    private float _Speed;

    private bool _IsSpeedUp;

    private Vector3 euler_angle_earth;
    private Vector3 euler_angle_moon;
    private Vector3 euler_angle_planet;

    private float delta_time;

    #endregion

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitPath ();
        InitFx ();
        InitConfig ();

        OnSpeedUp (SpeedUpManager.InstanceAwake ().IsEnableSpeedUp ());
    }

    private void Update ()
    {
        delta_time = Time.deltaTime;

        euler_angle_earth.z  += delta_time * 3;
        euler_angle_planet.z -= delta_time;
        euler_angle_moon.z   -= delta_time * 2f;
    }

    private void LateUpdate ()
    {
        transform_earth.localEulerAngles  = euler_angle_earth;
        transform_moon.localEulerAngles   = euler_angle_moon;
        transform_planet.localEulerAngles = euler_angle_planet;
    }

    protected override void OnDestroy ()
    {
        UnInitFx ();

        base.OnDestroy ();
    }

    private void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus)
        {
            euler_angle_earth.z  = euler_angle_earth.z % 720;
            euler_angle_moon.z   = euler_angle_moon.z % 720;
            euler_angle_planet.z = euler_angle_planet.z % 720;
        }
    }

    #endregion

    #region Controller

    private void InitConfig ()
    {
        _Speed = 1f;

        euler_angle_earth  = transform_earth.localEulerAngles;
        euler_angle_moon   = transform_moon.localEulerAngles;
        euler_angle_planet = transform_planet.localEulerAngles;
    }

    private void InitFx ()
    {
        this.RegisterActionEvent (ActionEnums.ActionID.SpeedUp, param => OnSpeedUp ((bool) param));
    }

    private void UnInitFx ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.SpeedUp, param => OnSpeedUp ((bool) param));
    }

    private void InitPath ()
    {
        _PositionPath = new Vector3[_TransformsPathMoving.Length];

        for (int i = 0; i < _PositionPath.Length; i++)
        {
            _PositionPath[i] = _TransformsPathMoving[i].localPosition;
        }

        Contains.StartIndexPoint = _StartIndexPoint;
    }

    #endregion

    #region Callback

    private void OnSpeedUp (bool obj)
    {
        if (obj == true)
        {
            _Speed = 1.5f;

            for (int i = 0; i < _IMovingObject.Count; i++)
            {
                _IMovingObject[i].EnableFx ();
            }

            _IsSpeedUp = true;
        }
        else
        {
            _Speed = 1f;

            for (int i = 0; i < _IMovingObject.Count; i++)
            {
                _IMovingObject[i].DisableFx ();
            }

            _IsSpeedUp = false;
        }

        for (int i = 0; i < _TweenObject.Count; i++)
        {
            _TweenObject[i].DOTimeScale (_Speed, Durations.DurationTimeScale);
        }
    }

    #endregion

    #region Action

    public void RegisterMoving (IMoving iMoving, ItemNodeData itemNodeData, int indexPoint, float time)
    {
        if (_IMovingObject.Contains (iMoving))
        {
            return;
        }

        _IMovingObject.Add (iMoving);
        _INodeData.Add (itemNodeData);

        var obj = iMoving.GetTransform ();

        obj.SetParent (transform_earth);

        obj.localPosition = _PositionPath[0];

        var _Tween = obj.DOLocalPath (_PositionPath, time, PathType.CatmullRom, PathMode.TopDown2D, 20, Color.green)
                        .SetOptions (true)
                        .SetLookAt (0)
                        .SetLoops (-1, LoopType.Restart)
                        .SetEase (Ease.Linear)
                        .OnStepComplete (() =>
                             {
                                 double value    = itemNodeData.ProfitPerSec * itemNodeData.PerCircleTime;
                                 var    profit   = itemNodeData.ProfitPerSecUnit;
                                 var    position = GameManager.Instance.GetPositionEndLine ();

                                 value = value * Mathf.Pow (itemNodeData.ProfitPerUpgradeCoefficient, PlayerData.GetNumberUpgradeItemProfitCoefficient (itemNodeData.Level));

                                 position.x -= Random.Range (0.25f, 0.5f);

                                 EarningManager.Instance.GetRealEarning (ref value, ref profit);

                                 GameManager.Instance.ShakeEndLine ();
                                 GameManager.Instance.ShakeCoins ();
                                 GameManager.Instance.FxExploseGold ();

                                 GameManager.Instance.FxDisplayGold (position,
                                                                     ApplicationManager.Instance.AppendFromCashUnit (value, profit));

                                 EarningManager.Instance.UpdateEarning (value, profit);
                                 SoundManager.Instance.PlaySound (AudioEnums.SoundId.ExploseGold);
                             }
                         );


        obj.localScale = new Vector3 (1.3f, 1.3f, 1.3f);

        obj.DOScale (1, Durations.DurationTimeScale).SetEase (Ease.OutBack);

        _Tween.timeScale = 0;

        _Tween.DOTimeScale (_Speed, Durations.DurationTimeScale).SetEase (Ease.Linear);

        if (indexPoint > -1)
        {
            _Tween.GotoWaypoint (indexPoint, true);
        }
        else
        {
            _Tween.GotoWaypoint (Random.Range (0, _PositionPath.Length), true);
            _Tween.SetDelay (Random.Range (0f, 0.5f));
        }

        _TweenObject.Add (_Tween);

        if (_IsSpeedUp)
        {
            iMoving.EnableFx ();
        }
        else
        {
            iMoving.DisableFx ();
        }
    }

    public void UnRegisterMoving (IMoving iMoving)
    {
        var indexOf = _IMovingObject.IndexOf (iMoving);

        if (indexOf < 0)
        {
            return;
        }

        _TweenObject[indexOf].Kill (false);

        _IMovingObject.RemoveAt (indexOf);
        _TweenObject.RemoveAt (indexOf);
        _INodeData.RemoveAt (indexOf);

        iMoving.DisableFx ();
    }

    public void RefreshEarning ()
    {
        for (int i = 0; i < _INodeData.Count; i++) { }
    }

    #endregion

    #region Helper

    public bool IsMovingCars ()
    {
        return _IMovingObject.Count > 0;
    }

    #endregion
}