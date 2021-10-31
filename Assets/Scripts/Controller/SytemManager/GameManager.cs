using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] [Header ("Config")] private Transform _TransformNode;

    [SerializeField] private Transform _TransformRewardItem,
                                       _TransformStartPosition;

    [SerializeField] [Header ("Grid")] private int _MaxRow,
                                                   _MaxColumn;

    [SerializeField] [Header ("Cell")] private float _DistanceOffset;

    [SerializeField] private float _WidthOffset,
                                   _HeightOffset;

    [Header ("Data")] [SerializeField] private ItemNodeGroupData _ItemNodeGroupData;

    [SerializeField] private LevelData _LevelData;

    [Header ("Line")] [SerializeField] private Transform _TransformEndLine;
    [SerializeField]                   private Transform transform_earth;

    [SerializeField] private TextMeshPro _TextBaseActive;
    [SerializeField] private float       _OffsetHeightSpriteBaseActive;
    [SerializeField] private float       _OffsetDistanceEachBase;

    [SerializeField] private float Height_Earth,
                                   Width_Earth;


    [Header ("Animation")] [SerializeField]
    private Animation _AnimationEndLine;

    [SerializeField] private Animation _AnimationCoin;

    [Header ("Fx")] [SerializeField] private ParticleSystem _FxExploseGold;

    [Header ("Renderer")] [SerializeField] private SpriteRenderer _EndLineRenderer;

    [Header ("Delete House")] [SerializeField]
    private SpriteRenderer transform_delete_house;

    [SerializeField] private Animation animation_delete_house;


    #region Variables

    private Vector3 _DefaultPosition;
    private Vector3 _SetPosition;
    private Vector3 _DragPosition;
    private Vector3 _OffsetPosition;

    private Camera _MainCamera;

    private bool _IsDrag;
    private bool _IsBeginDrag;
    private bool _IsReady;
    private bool _IsRenderDrag;
    private bool _IsTouchReady;
    private bool _IsNodeInBusy;
    private bool _IsEnableOutline;
    private bool _IsInteractGame;

    private Vector2Int _PreviousIndex;
    private Vector2Int _LastIndex;
    private Vector2Int _FreeIndex;

    private NodeComponent[][] _ItemNode;
    private NodeComponent[][] _GridNode;
    private NodeComponent     _NodeComponentInDrag;
    private NodeComponent     _NodeComponentMerge;

    private List<int> _FreeIndexXColumn;
    private List<int> _FreeIndexYColumn;

    private INodeGrid[][] _INodeGrids;

    private Transform _TransformInDrag;

    private Vector3 position_earth;
    private Vector3 _PositionEndLine;

    private int _NumberBaseActive;
    private int _MaxBaseActive;

    private int _Row,
                _Column,
                _MaxNodeActive;

    private System.Action<object> HandleStateInteractGame;

    private float   height_delete_house;
    private float   width_delete_house;
    private Vector3 position_delete_house;

    private bool is_enable_delete_house;

    private Transform transform_delete;
    private Transform transform_base_active;

    #endregion

    #region Systems

    private void Start ()
    {
        InitConfig ();
        InitNodes ();
        InitGrids ();
        InitParameters ();

        InstanceBase ();

        InitFreeList ();
        InitBaseActive ();

        LoadBase ();
        LoadBaseMoving ();
        LoadRevenueOffline ();

        LoadBackgroundMusic ();

        RegisterAction ();

        if (!this.IsTutorialCompleted (TutorialEnums.TutorialId.HowToPlayGame))
        {
            this.ExecuteTutorial (TutorialEnums.TutorialId.HowToPlayGame);
        }

        ApplicationManager.IsGameReady = true;
    }

    private void Update ()
    {
        if (!_IsReady || !_IsTouchReady)
        {
            if (Input.GetMouseButtonDown (0))
            {
                this.PostTapAnyWhere ();
            }

            return;
        }

        if (!_IsBeginDrag && Input.GetMouseButtonDown (0))
        {
            // =============================== START DRAG ================================ //

            this.PostTapAnyWhere ();

            _SetPosition = ConvertScreenTouchToPosition ();

            _PreviousIndex.x = GetIndexXWithPositionX (_SetPosition.x);
            _PreviousIndex.y = GetIndexYWithPositionY (_SetPosition.y);

            if (BonusManager.Instance.PostPositionBonusCoins (_SetPosition))
            {
                return;
            }

            _NodeComponentInDrag = GetNodeInGrid (_PreviousIndex.x, _PreviousIndex.y);

            LogGame.Log (string.Format ("[Game Manager] Get Input Touch Index {0} : {1}", _PreviousIndex.x.ToString (), _PreviousIndex.y.ToString ()));

            if (ReferenceEquals (_NodeComponentInDrag, null))
            {
                return;
            }

            _IsBeginDrag  = true;
            _IsNodeInBusy = _NodeComponentInDrag.IsBusy ();

            _TransformInDrag = _NodeComponentInDrag.transform;
            _OffsetPosition  = _TransformInDrag.position;

            _OffsetPosition.x = _SetPosition.x - _OffsetPosition.x;
            _OffsetPosition.y = _SetPosition.y - _OffsetPosition.y;
            _OffsetPosition.z = 0;

            return;
        }

        if (_IsBeginDrag && Input.GetMouseButtonUp (0))
        {
            LogGame.Log (string.Format ("[Game Manager] Get Input Touch Index {0} : {1}", _LastIndex.x.ToString (), _LastIndex.y.ToString ()));

            // =============================== DRAG COMPLETED ================================ //


            _IsBeginDrag = false;

            if (_IsEnableOutline)
            {
                DisableOutlineSelected ();
            }

            if (_IsDrag == false && _NodeComponentInDrag != null)
            {
                if (_IsNodeInBusy)
                {
                    _NodeComponentInDrag.TouchBusy ();
                }
                else
                {
                    _NodeComponentInDrag.TouchHit ();
                }

                Instance.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

                return;
            }
            else if (_IsDrag == true && _IsNodeInBusy)
            {
                _IsDrag = false;

                return;
            }

            _IsDrag = false;

            _SetPosition = ConvertScreenTouchToPosition ();

            _SetPosition.x = _SetPosition.x - _OffsetPosition.x;
            _SetPosition.y = _SetPosition.y - _OffsetPosition.y;
            _SetPosition.z = _SetPosition.z - _OffsetPosition.z;

            _LastIndex.x = GetIndexXWithPositionX (_SetPosition.x);
            _LastIndex.y = GetIndexYWithPositionY (_SetPosition.y);

            _NodeComponentMerge = GetNodeInGrid (_LastIndex.x, _LastIndex.y);

            if (!ReferenceEquals (_NodeComponentMerge, null) && _NodeComponentMerge.IsBusy () == false && _IsInteractGame)
            {
                if (_NodeComponentMerge.GetLevel () < GameConfig.TotalItem && _NodeComponentMerge.GetLevel () == _NodeComponentInDrag.GetLevel () && _NodeComponentMerge.GetId () != _NodeComponentInDrag.GetId ())
                {
                    SetNodeInGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY (), null);
                    SetFreeIndexGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY ());

                    _IsReady = false;

                    FxMergeTwo (GetPositionWithIndex (_LastIndex.x, _LastIndex.y), _NodeComponentMerge.transform, _NodeComponentInDrag.transform, () =>
                    {
                        if (Instance == null) return;

                        PoolExtension.SetPool (Instance._NodeComponentInDrag.GetPoolId (),
                                               Instance._NodeComponentInDrag.transform);

                        Instance._NodeComponentInDrag.SetDisable ();

                        Instance._NodeComponentInDrag = null;

                        Instance._NodeComponentMerge.transform.localScale = Vector3.zero;

                        Instance._NodeComponentMerge.transform
                                .DOScale (1, Durations.DurationScale)
                                .SetEase (Ease.OutBack)
                                .OnComplete (() => { Instance._IsReady = true; });

                        Instance.PlayAudioSound (AudioEnums.SoundId.MergeCompleted);

                        Instance._NodeComponentMerge.Init (Instance._ItemNodeGroupData.GetDataItemWithLevel (Instance._NodeComponentMerge.GetLevel () + 1));


                        var Exp = Instance._NodeComponentMerge.GetExp ();

                        FxStarsExp (Instance._NodeComponentMerge.GetPosition (), UIGameManager.Instance.GetPositionHubExp (), () =>
                        {
                            if (GameActionManager.Instance != null)
                            {
                                GameActionManager.Instance.SetExp (Exp);
                            }

                            if (Instance != null) Instance.PlayAudioSound (AudioEnums.SoundId.Stars);
                        });

                        Instance.SetNodeInGrid (_NodeComponentMerge.GetIndexX (),
                                                _NodeComponentMerge.GetIndexY (),
                                                _NodeComponentMerge);

                        if (Instance._NodeComponentMerge.GetLevel () > PlayerData.LastLevelUnlocked)
                        {
                            SetUnlockHighItem (Instance._NodeComponentMerge.GetLevel ());
                        }

                        Instance.FXSunshine (Instance._NodeComponentMerge.GetPosition ());

                        Instance.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

                        GameActionManager.Instance.InstanceFxTapFlower (Instance._NodeComponentMerge.GetPosition ());
                        GameActionManager.Instance.InstanceFxLevelUpItems (Instance._NodeComponentMerge.GetPosition ());


                        Instance._NodeComponentMerge.SetIndex (_NodeComponentMerge.GetIndexX (), _NodeComponentMerge.GetIndexY ());

                        if (Random.Range (0.00f, 1.00f) < 0.4f)
                        {
                            this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);
                        }

                        UIGameManager.Instance.RefreshRandomRewardCondition ();
                    });

                    SetDisableNodeIconItem (_PreviousIndex.x, _PreviousIndex.y);

                    this.PostMissionEvent (MissionEnums.MissionId.Merge);

                    DisableDeleteHouse ();

                    return;
                }
                else if (_NodeComponentMerge.GetId () != _NodeComponentInDrag.GetId ())
                {
                    _IsReady = false;

                    SetNodeInGrid (_PreviousIndex.x, _PreviousIndex.y, _NodeComponentMerge);
                    SetUnFreeIndexGrid (_PreviousIndex.x, _PreviousIndex.y);

                    SetNodeInGrid (_LastIndex.x, _LastIndex.y, _NodeComponentInDrag);
                    SetUnFreeIndexGrid (_LastIndex.x, _LastIndex.y);

                    _NodeComponentInDrag
                       .SetIndex (_LastIndex.x,
                                  _LastIndex.y)
                       .SetPosition (Instance.GetPositionWithIndex (_LastIndex.x,
                                                                    _LastIndex.y));

                    FxMoveNode (GetPositionWithIndex (_PreviousIndex.x, _PreviousIndex.y),
                                Instance._NodeComponentMerge.transform, () =>
                                {
                                    if (Instance != null)
                                    {
                                        Instance._NodeComponentMerge
                                                .SetIndex (Instance._PreviousIndex.x,
                                                           Instance._PreviousIndex.y)
                                                .SetPosition (Instance.GetPositionWithIndex (Instance._PreviousIndex.x,
                                                                                             Instance._PreviousIndex.y));

                                        Instance.SetNodeInGrid (Instance._PreviousIndex.x,
                                                                Instance._PreviousIndex.y,
                                                                Instance._NodeComponentMerge);

                                        Instance._IsReady = true;
                                    }
                                });

                    SetDisableNodeIconItem (_PreviousIndex.x, _PreviousIndex.y);
                    SetDisableNodeIconItem (_LastIndex.x, _LastIndex.y);
                }
            }

            if (_NodeComponentInDrag != null)
            {
                if (is_enable_delete_house && IsDeleteItems (_NodeComponentInDrag.GetPosition ()) && this.IsTutorialCompleted (TutorialEnums.TutorialId.HowToPlayGame))
                {
                    SetNodeInGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY (), null);
                    SetFreeIndexGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY ());

                    PoolExtension.SetPool (_NodeComponentInDrag.GetPoolId (),
                                           _NodeComponentInDrag.transform);

                    _NodeComponentInDrag.SetDisable ();

                    _NodeComponentInDrag = null;

                    SetDisableNodeIconItem (_PreviousIndex.x, _PreviousIndex.y);

                    GameActionManager.Instance.InstanceFxTapFlower (position_delete_house);

                    this.PlayAudioSound (AudioEnums.SoundId.Backward);

                    UIGameManager.Instance.RefreshRandomRewardCondition ();
                }
                else if (!IsOutOfMaxBase (_LastIndex.x, _LastIndex.y) && IsOutOfGrid (_LastIndex.x, _LastIndex.y) == false && !IsExistsNodeInGrid (_LastIndex.x, _LastIndex.y))
                {
                    SetNodeInGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY (), null);
                    SetFreeIndexGrid (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY ());

                    _NodeComponentInDrag
                       .SetIndex (_LastIndex.x,
                                  _LastIndex.y)
                       .SetPosition (Instance.GetPositionWithIndex (_LastIndex.x,
                                                                    _LastIndex.y));

                    FxPutNode (_NodeComponentInDrag.transform, null);

                    SetNodeInGrid (_LastIndex.x, _LastIndex.y, _NodeComponentInDrag);
                    SetUnFreeIndexGrid (_LastIndex.x, _LastIndex.y);

                    SetDisableNodeIconItem (_PreviousIndex.x, _PreviousIndex.y);

                    this.PlayAudioSound (AudioEnums.SoundId.Backward);
                }
                else
                {
                    if (IsSetItemForMinerGold (_SetPosition) && _IsInteractGame)
                    {
                        var item = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemMoving);

                        if (item != null)
                        {
                            var basePlaneMoving    = item.GetComponent<BasePlaneMoving> ();
                            var basePlaneComponent = _NodeComponentInDrag.GetComponent<BasePlaneComponent> ();

                            if (basePlaneMoving != null && basePlaneComponent != null)
                            {
                                _NumberBaseActive++;

                                UpdateBaseActiveForEarning ();

                                SetEnableNodeIconBack (_PreviousIndex.x, _PreviousIndex.y);

                                _NodeComponentInDrag.SetDisable ();
                                _NodeComponentInDrag.SetBusy (true);

                                basePlaneComponent.SetPlaneMoving (basePlaneMoving);

                                basePlaneMoving.SetItemData (_ItemNodeGroupData.GetDataItemWithLevel (_NodeComponentInDrag.GetLevel ()));

                                basePlaneMoving.Register (Contains.StartIndexPoint);

                                GameActionManager.Instance.InstanceFxFireWork (transform_base_active.position);

                                PlayerData.SaveItemMoving (_NodeComponentInDrag.GetLevel (), _PreviousIndex.x, _PreviousIndex.y);

                                Instance.PostCompletedTutorial (TutorialEnums.TutorialId.HowToPlayGame);

                                Instance.DisableDeleteHouse ();

                                Instance.PlayAudioSound (AudioEnums.SoundId.PutItemsEarning);

                                return;
                            }
                        }

                        this.PlayAudioSound (AudioEnums.SoundId.Backward);
                    }

                    _IsReady = false;

                    SetDisableNodeIconItem (_PreviousIndex.x, _PreviousIndex.y);

                    FxMoveNode (GetPositionWithIndex (_NodeComponentInDrag.GetIndexX (), _NodeComponentInDrag.GetIndexY ()),
                                _NodeComponentInDrag.transform,
                                () =>
                                {
                                    Instance._IsReady = true;
                                    Instance._NodeComponentInDrag.SetPosition (Instance.GetPositionWithIndex (Instance._NodeComponentInDrag.GetIndexX (), Instance._NodeComponentInDrag.GetIndexY ()));
                                });

                    this.PlayAudioSound (AudioEnums.SoundId.Backward);
                }
            }


            DisableDeleteHouse ();

            // =============================== DOING COMPLETED DRAG ================================ //

            return;
        }

        if (!_IsBeginDrag) return;

        // =============================== DOING SOMETHING WHEN DRAG ================================ //

        _DragPosition   = ConvertScreenTouchToPosition ();
        _DragPosition.z = 0;

        if (_IsDrag == false)
        {
            if (Vector2.Distance (_SetPosition, _DragPosition) < 0.1f)
            {
                return;
            }

            _IsDrag = true;

            if (!ReferenceEquals (_NodeComponentInDrag, null) && !_NodeComponentInDrag.IsBusy ())
            {
                SetEnableNodeIconItem (_NodeComponentInDrag.GetLevel (), _PreviousIndex.x, _PreviousIndex.y, _NodeComponentInDrag.GetKey ());
            }

            if (_IsNodeInBusy)
            {
                return;
            }

            if (!ReferenceEquals (_NodeComponentInDrag, null))
            {
                EnableOutlineSelected (_NodeComponentInDrag.GetId (),
                                       _NodeComponentInDrag.GetLevel ());

                EnableDeleteHouse ();
            }
        }

        _IsRenderDrag = true;
    }

    private void LateUpdate ()
    {
        if (!_IsReady)
            return;

        if (!_IsRenderDrag)
            return;

        if (_IsNodeInBusy)
            return;

        _SetPosition.x = _DragPosition.x - _OffsetPosition.x;
        _SetPosition.y = _DragPosition.y - _OffsetPosition.y;
        _SetPosition.z = _DragPosition.z - _OffsetPosition.z;

        _TransformInDrag.position = _SetPosition;
        _IsRenderDrag             = false;
    }

    protected override void OnDestroy ()
    {
        UnRegisterAction ();

        PlayerData.SaveTotalTimeMultiRewardCoins ();

        base.OnDestroy ();
    }

    #endregion

    #region Controller

    private void RegisterAction ()
    {
        HandleStateInteractGame = param => Instance.OnInteractGame ((bool) param);

        this.RegisterActionEvent (ActionEnums.ActionID.SetStateInteractGame, HandleStateInteractGame);
    }

    private void UnRegisterAction ()
    {
        this.RemoveActionEvent (ActionEnums.ActionID.SetStateInteractGame, HandleStateInteractGame);
    }

    private void LoadBase ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                var level = PlayerData.LoadLevelItemStatic (i, j);

                if (level == -1)
                {
                    continue;
                }

                var node = _ItemNodeGroupData.GetDataItemWithLevel (level);


                SetBaseItemGrid (node, i, j);
                SetUnFreeIndexGrid (i, j);
            }
        }
    }

    private void LoadBaseMoving ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                var level = PlayerData.LoadItemMoving (i, j);

                if (level == -1)
                {
                    continue;
                }

                var node = GetNodeInGrid (i, j);

                if (node == null)
                    continue;

                var item = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemMoving);

                if (item == null)
                    continue;

                var basePlaneMoving    = item.GetComponent<BasePlaneMoving> ();
                var basePlaneComponent = node.GetComponent<BasePlaneComponent> ();

                _NumberBaseActive++;

                UpdateBaseActiveForEarning ();

                SetEnableNodeIconBack (i, j);
                SetEnableNodeIconItem (node.GetLevel (), i, j, node.GetKey ());

                node.SetDisable ();
                node.SetBusy (true);

                basePlaneComponent.SetPlaneMoving (basePlaneMoving);

                basePlaneMoving.SetItemData (_ItemNodeGroupData.GetDataItemWithLevel (node.GetLevel ()));

                basePlaneMoving.Register (-1);
            }
        }
    }

    private void LoadRevenueOffline ()
    {
        if (OfflineManager.InstanceAwake () != null &&
            EarningManager.InstanceAwake () != null)
        {
            OfflineManager.Instance.EnableOfflineProfit (EarningManager.Instance.ProfitPerSec,
                                                         EarningManager.Instance.ProfitUnit);
        }
    }

    private void LoadBackgroundMusic ()
    {
        this.PlayAudioMusic (AudioEnums.MusicId.Background, true);
    }

    public void InstanceBase ()
    {
        _MaxNodeActive = _LevelData.GetMaxItemWithLevel (PlayerData.Level);

        if (_MaxNodeActive < 4)
            _MaxNodeActive = 4;
        
        if (_MaxNodeActive < _MaxColumn + 2)
        {
            _Row    = Mathf.Clamp (_Row, 2, _MaxRow);
            _Column = Mathf.Clamp ((_MaxNodeActive - _MaxNodeActive % _Row) / _Row + _MaxNodeActive % _Row, 2, _MaxColumn);
        }
        else
        {
            _Column = _MaxColumn;
            _Row    = Mathf.Clamp ((_MaxNodeActive - _MaxNodeActive % _Column) / _Column, 2, _MaxRow);

            if (_MaxNodeActive - _Row * _Column > 0)
            {
                _Row = Mathf.Clamp (_Row + 1, 0, _MaxRow);
            }
        }

        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                _INodeGrids[i][j].Enable ();

                var position = GetPositionWithIndex (i, j);

                if (_ItemNode[i][j] != null)
                {
                    _ItemNode[i][j].SetPosition (position);
                }

                _GridNode[i][j].SetPosition (position);
            }
        }

        _MaxBaseActive = _LevelData.GetMaxItemInPlayWithLevel (PlayerData.Level);
    }

    private void InitConfig ()
    {
        _DefaultPosition = _TransformStartPosition.position;
        _MainCamera      = Camera.main;

        if (transform_earth != null) position_earth     = transform_earth.position;
        if (_TransformEndLine != null) _PositionEndLine = _TransformEndLine.position;

        _IsReady        = true;
        _IsTouchReady   = true;
        _IsInteractGame = true;
    }

    private void InitParameters ()
    {
        var size = transform_delete_house.size;

        height_delete_house = size.y * transform_delete_house.transform.localScale.y;
        width_delete_house  = size.x * transform_delete_house.transform.localScale.x;

        position_delete_house = transform_delete_house.transform.position;

        transform_delete = transform_delete_house.transform;

        transform_base_active = _TextBaseActive.transform;
    }

    private void InitGrids ()
    {
        for (int i = 0; i < _MaxColumn; i++)
        {
            for (int j = 0; j < _MaxRow; j++)
            {
                var node      = Instantiate (_TransformNode.gameObject, _TransformStartPosition);
                var component = node.GetComponent<NodeComponent> ();

                component
                   .Init (null)
                   .SetIndex (i, j)
                   .SetPosition (GetPositionWithIndex (i, j))
                   .SetEnable ();

                var iNodeGrid = node.GetComponent<INodeGrid> ();

                _INodeGrids[i][j] = iNodeGrid;
                _GridNode[i][j]   = component;

                iNodeGrid.DisableLevel ();
                iNodeGrid.DisableIconBack ();
                iNodeGrid.Disable ();
            }
        }
    }

    private void InitNodes ()
    {
        _ItemNode   = new NodeComponent[_MaxColumn][];
        _GridNode   = new NodeComponent[_MaxColumn][];
        _INodeGrids = new INodeGrid[_MaxColumn][];

        for (int i = 0; i < _MaxColumn; i++)
        {
            _ItemNode[i]   = new NodeComponent[_MaxRow];
            _GridNode[i]   = new NodeComponent[_MaxRow];
            _INodeGrids[i] = new INodeGrid[_MaxRow];
        }
    }

    private void InitFreeList ()
    {
        _FreeIndexXColumn = new List<int> ();
        _FreeIndexYColumn = new List<int> ();

        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                if (ReferenceEquals (_ItemNode[i][j], null))
                {
                    _FreeIndexXColumn.Add (i);
                    _FreeIndexYColumn.Add (j);
                }
            }
        }
    }

    private void InitBaseActive ()
    {
        // =============================== INIT BASE ================================ //

        _NumberBaseActive = 0;

        // =============================== LOAD BASE ================================ //

        UpdateBaseActiveForEarning ();
    }

    #endregion

    #region Action

    public void UpdateFreeList ()
    {
        _FreeIndexXColumn.Clear ();
        _FreeIndexYColumn.Clear ();

        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                if (ReferenceEquals (_ItemNode[i][j], null))
                {
                    _FreeIndexXColumn.Add (i);
                    _FreeIndexYColumn.Add (j);
                }
            }
        }
    }

    public void SetUnlockHighItem (int level)
    {
        PlayerData.LastLevelUnlocked = level;
        PlayerData.SaveUnlockItemLevel ();

        if (UnlockManager.Instance != null)
        {
            UnlockManager.Instance.UnlockItemLevel (level);
        }

        ApplicationManager.Instance.CheckUnlockLevelRate ();
    }

    public void EnableOutlineSelected (int instanceId, int level)
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                _INodeGrids[i][j].EnableOutline (instanceId, level);
            }
        }

        _IsEnableOutline = true;
    }

    public void DisableOutlineSelected ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                if (IsOutOfMaxBase (i, j))
                    break;

                _INodeGrids[i][j].DisableOutline ();
            }
        }

        _IsEnableOutline = false;
    }

    public void EnableTouch ()
    {
        _IsTouchReady = true;
    }

    public void DisableTouch ()
    {
        _IsTouchReady = false;
    }

    public void SetRandomReward ()
    {
        // =============================== RANDOM THE REWARD ================================ //

        _FreeIndex = GetFreeIndexGrid ();

        if (IsOutOfGrid (_FreeIndex.x, _FreeIndex.y))
        {
            return;
        }

        var reward          = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemBox);
        var rewardComponent = reward.GetComponent<NodeComponent> ();

        rewardComponent
           .SetBusy (false)
           .SetIndex (_FreeIndex.x, _FreeIndex.y)
           .SetPosition (GetPositionWithIndex (_FreeIndex.x, _FreeIndex.y))
           .Init (_ItemNodeGroupData.GetDataItemWithLevel (ApplicationManager.Instance.GetRandomItemReward ()))
           .SetEnable ();

        rewardComponent.SetUnbox (false);

        reward.GetComponent<BoxBehaviour> ()
              .SetIcon (BoxEnums.BoxId.RewardBox);

        SetNodeInGrid (_FreeIndex.x, _FreeIndex.y, rewardComponent);
    }

    public void SetRandomTouchBoxReward ()
    {
        // =============================== Random the reward from the touch box ================================ //

        _FreeIndex = GetFreeIndexGrid ();

        if (IsOutOfGrid (_FreeIndex.x, _FreeIndex.y))
        {
            return;
        }

        var reward          = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemBox);
        var rewardComponent = reward.GetComponent<NodeComponent> ();

        rewardComponent
           .SetBusy (false)
           .SetIndex (_FreeIndex.x, _FreeIndex.y)
           .SetPosition (GetPositionWithIndex (_FreeIndex.x, _FreeIndex.y))
           .Init (_ItemNodeGroupData.GetDataItemWithLevel (ApplicationManager.Instance.GetRandomItemTouchBox ()))
           .SetEnable ();

        rewardComponent.SetUnbox (false);

        reward.GetComponent<BoxBehaviour> ()
              .SetIcon (BoxEnums.BoxId.RandomBox);

        SetNodeInGrid (_FreeIndex.x, _FreeIndex.y, rewardComponent);
    }

    public void SetBoxReward (int item_level)
    {
        _FreeIndex = GetFreeIndexGrid ();

        if (IsOutOfGrid (_FreeIndex.x, _FreeIndex.y))
            return;

        var reward          = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemBox);
        var rewardComponent = reward.GetComponent<NodeComponent> ();

        rewardComponent
           .SetBusy (false)
           .SetIndex (_FreeIndex.x, _FreeIndex.y)
           .SetPosition (GetPositionWithIndex (_FreeIndex.x, _FreeIndex.y))
           .Init (_ItemNodeGroupData.GetDataItemWithLevel (item_level))
           .SetEnable ();

        rewardComponent.SetUnbox (false);

        reward.GetComponent<BoxBehaviour> ()
              .SetIcon (BoxEnums.BoxId.ShopBox);

        SetNodeInGrid (_FreeIndex.x, _FreeIndex.y, rewardComponent);
        SetStateInGrid (_FreeIndex.x, _FreeIndex.y, false);
    }

    public void SetBaseItemGrid (ItemNodeData itemNodeData, int xColumn, int yRow)
    {
        var item          = PoolExtension.GetPool (PoolEnums.PoolId.BaseItemGrid);
        var itemComponent = item.GetComponent<NodeComponent> ();

        itemComponent
           .SetIndex (xColumn, yRow)
           .SetPosition (GetPositionWithIndex (xColumn, yRow))
           .SetBusy (false)
           .Init (itemNodeData)
           .SetEnable ();

        itemComponent.SetUnbox (true);

        SetNodeInGrid (xColumn, yRow, itemComponent);
        SetStateInGrid (xColumn, yRow, true);
        FxAppear (item, null);
    }

    public void SetEnableNodeIconItem (int level, int xColumn, int yRow, string key)
    {
        if (IsOutOfGrid (xColumn, yRow))
            return;

        _INodeGrids[xColumn][yRow].EnableIconItem (level, key);
    }

    public void SetDisableNodeIconItem (int xColumn, int yRow)
    {
        if (IsOutOfGrid (xColumn, yRow))
            return;

        _INodeGrids[xColumn][yRow].DisableIconItem ();
    }

    public void SetEnableNodeIconBack (int xColumn, int yRow)
    {
        if (IsOutOfGrid (xColumn, yRow))
            return;

        _INodeGrids[xColumn][yRow].EnableIconBack ();
    }

    public void SetDisableNodeIconBack (int xColumn, int yRow)
    {
        if (IsOutOfGrid (xColumn, yRow))
            return;

        _INodeGrids[xColumn][yRow].DisableIconBack ();
    }

    public void SetItemBackToNode (int indexX, int indexY, BasePlaneMoving basePlaneMoving, NodeComponent nodeComponent)
    {
        var tf = basePlaneMoving.transform;

        _IsReady = false;

        tf.DOMove (GetPositionWithIndex (indexX, indexY), Durations.DurationMovingMerge).OnComplete (() =>
        {
            Instance.SetDisableNodeIconBack (indexX, indexY);
            Instance.SetDisableNodeIconItem (indexX, indexY);

            Instance._IsReady = true;

            nodeComponent.SetEnable ();
            nodeComponent.SetBusy (false);
            nodeComponent.SetPosition (Instance.GetPositionWithIndex (indexX, indexY));

            basePlaneMoving.ReturnToPools ();

            Instance.PlayAudioSound (AudioEnums.SoundId.PutItemsBack);
        });
    }

    public void MinusBaseActive ()
    {
        _NumberBaseActive = Mathf.Clamp (_NumberBaseActive - 1, 0, _NumberBaseActive);

        UpdateBaseActiveForEarning ();
    }

    public void UpdateBaseActiveForEarning ()
    {
        if (_TextBaseActive != null)
        {
            _TextBaseActive.text = string.Format ("{0}/{1}", _NumberBaseActive.ToString (), _MaxBaseActive.ToString ());
        }

        PlayerData.SaveTotalItemMoving (_NumberBaseActive);
    }

    public void ForceUnlockAllBoxes ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                var item = _ItemNode[i][j];

                if (!ReferenceEquals (item, null) && item.GetPoolId () == PoolEnums.PoolId.BaseItemBox)
                    item.TouchBusy ();
            }
        }
    }

    private void EnableDeleteHouse ()
    {
        animation_delete_house.Play ();

        is_enable_delete_house = true;
    }

    private void DisableDeleteHouse ()
    {
        animation_delete_house.Stop ();

        is_enable_delete_house = false;
    }

    #endregion

    #region Animation

    public void ShakeEndLine ()
    {
        if (_TransformEndLine == null) return;

        if (_AnimationEndLine != null)
        {
            if (_AnimationEndLine.isPlaying)
            {
                _AnimationEndLine.Stop ();
            }

            _AnimationEndLine.Play ();
        }
        else
        {
            _TransformEndLine.localScale = Vector.Vector3One;

            _TransformEndLine.DOKill ();
            _TransformEndLine.DOShakeScale (Durations.DurationShakeScale, Contains.ShakeScale);
        }
    }

    public void ShakeCoins ()
    {
        if (_AnimationCoin != null)
        {
            if (_AnimationCoin.isPlaying)
            {
                _AnimationCoin.Stop ();
            }

            _AnimationCoin.Play ();
        }
    }

    #endregion

    #region Callback

    private void OnInteractGame (bool state)
    {
        _IsInteractGame = state;
    }

    #endregion

    #region Helper

    public Vector3 GetPositionEndLine ()
    {
        return _TransformEndLine.position;
    }

    public Vector3 GetLocalPositionEndLine ()
    {
        return _TransformEndLine.localPosition;
    }

    public Vector3 GetPositionStartLine ()
    {
        return transform_earth.position;
    }

    public Vector3 ConvertScreenTouchToPosition ()
    {
        return _MainCamera.ScreenToWorldPoint (Input.mousePosition);
    }

    public Vector3 GetPositionWithIndex (int xColumn, int yRow)
    {
        // =============================== GET POSITION WITH INDEX ================================ //

        _SetPosition.x = _DefaultPosition.x + (xColumn - (_Column - 1) / 2f) * (_WidthOffset + _DistanceOffset);
        _SetPosition.y = _DefaultPosition.y - (yRow - (_Row - 1) / 2f) * (_HeightOffset + _DistanceOffset);
        _SetPosition.z = 0;

        return _SetPosition;
    }

    public int GetIndexXWithPositionX (float xPosition)
    {
        var index = (xPosition + _WidthOffset / 2 - _DefaultPosition.x) / (_WidthOffset + _DistanceOffset) + (_Column - 1) / 2f;

        return index < 0 ? -1 : (int) index;
    }

    public int GetIndexYWithPositionY (float yPosition)
    {
        var index = (_DefaultPosition.y - yPosition + _HeightOffset) / (_HeightOffset + _DistanceOffset) + (_Row - 1) / 2f;

        return index < 0 ? -1 : (int) index;
    }

    public bool IsDeleteItems (Vector3 position)
    {
        position_delete_house = transform_delete.position;

        return position.x > position_delete_house.x - width_delete_house / 2f &&
               position.x < position_delete_house.x + width_delete_house / 2f &&
               position.y > position_delete_house.y - height_delete_house / 2f &&
               position.y < position_delete_house.y + height_delete_house / 2f;
    }

    public bool IsOutOfGrid (int xColumn, int yRow)
    {
        return xColumn < 0 || yRow < 0 || xColumn >= _Column || yRow >= _Row;
    }

    public bool IsExistsNodeInGrid (int xColumn, int yRow)
    {
        return _ItemNode[xColumn][yRow] != null;
    }

    public void SetNodeInGrid (int xColumn, int yRow, NodeComponent node)
    {
        _ItemNode[xColumn][yRow] = node;

        if (node != null)
        {
            _INodeGrids[xColumn][yRow].SetInfo (node.GetId (), node.GetLevel ());

            if (node.IsUnbox ())
            {
                _INodeGrids[xColumn][yRow].EnableLevel (node.GetLevel ());
            }
            else
            {
                _INodeGrids[xColumn][yRow].DisableLevel ();
            }

            PlayerData.SaveItemStatic (node.GetLevel (), xColumn, yRow);
        }
        else
        {
            _INodeGrids[xColumn][yRow].SetInfo (-1, -1);
            _INodeGrids[xColumn][yRow].DisableLevel ();
            PlayerData.SaveItemStatic (-1, xColumn, yRow);
        }
    }

    public void EnableLevelNode (int xColumn, int yRow, int level)
    {
        _INodeGrids[xColumn][yRow].EnableLevel (level);
    }

    public void DisableLevelNode (int xColumn, int yRow)
    {
        _INodeGrids[xColumn][yRow].DisableLevel ();
    }

    public void SetStateInGrid (int xColumn, int yRow, bool isActive)
    {
        if (IsOutOfGrid (xColumn, yRow))
            return;

        _INodeGrids[xColumn][yRow].SetState (isActive);
    }

    public NodeComponent GetNodeInGrid (int xColumn, int yRow)
    {
        return IsOutOfGrid (xColumn, yRow) ? null : _ItemNode[xColumn][yRow];
    }

    public Vector2Int GetFreeIndexGrid ()
    {
        _FreeIndex.x = -1;
        _FreeIndex.y = -1;

        if (_FreeIndexXColumn.Count == 0)
            return _FreeIndex;

        var index = Random.Range (0, _FreeIndexXColumn.Count);

        _FreeIndex.x = _FreeIndexXColumn[index];
        _FreeIndex.y = _FreeIndexYColumn[index];

        _FreeIndexXColumn.RemoveAt (index);
        _FreeIndexYColumn.RemoveAt (index);

        return _FreeIndex;
    }

    public void SetUnFreeIndexGrid (int xColumn, int yRow)
    {
        for (int i = 0; i < _FreeIndexXColumn.Count; i++)
        {
            if (_FreeIndexXColumn[i] == xColumn && _FreeIndexYColumn[i] == yRow)
            {
                _FreeIndexXColumn.RemoveAt (i);
                _FreeIndexYColumn.RemoveAt (i);
                break;
            }
        }
    }

    public void SetFreeIndexGrid (int xColumn, int yRow)
    {
        _FreeIndexXColumn.Add (xColumn);
        _FreeIndexYColumn.Add (yRow);
    }

    public bool IsFreeIndexGrid ()
    {
        return _FreeIndexXColumn.Count > 0;
    }

    public bool IsSetItemForMinerGold (Vector3 position)
    {
        return position.x > position_earth.x - Width_Earth / 2f &&
               position.x < position_earth.x + Width_Earth / 2f &&
               position.y > position_earth.y - Height_Earth / 2f &&
               position.y < position_earth.y + Height_Earth / 2f &&
               _MaxBaseActive > _NumberBaseActive;
    }

    public bool IsOutOfMaxBase (int xColumn, int yRow)
    {
        return _Column * yRow + xColumn > _MaxNodeActive - 1;
    }

    public Vector3 GetPositionBusyItemNode ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                var item = _ItemNode[i][j];

                if (ReferenceEquals (item, null))
                    continue;

                if (!item.IsBusy ())
                    continue;

                return _GridNode[i][j].GetPosition ();
            }
        }

        return Vector.Vector3Null;
    }

    public Vector3 GetPositionFreeItemNode ()
    {
        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                var item = _ItemNode[i][j];

                if (ReferenceEquals (item, null))
                    continue;

                if (item.IsBusy ())
                    continue;

                return item.GetPosition ();
            }
        }

        return Vector.Vector3Null;
    }

    public List<Vector3> GetRandomFreeToUseItemNode ()
    {
        List<Vector3> Position = new List<Vector3> ();

        for (int i = 0; i < _Column; i++)
        {
            for (int j = 0; j < _Row; j++)
            {
                var item = _ItemNode[i][j];

                // =============================== Loop To Find ================================ //

                if (item == null)
                    continue;

                if (item.IsBusy ())
                    continue;

                for (int h = 0; h < _Column; h++)
                {
                    for (int k = 0; k < _Row; k++)
                    {
                        var item_b = _ItemNode[h][k];

                        if (ReferenceEquals (item_b, null))
                            continue;

                        if (item_b.IsBusy ())
                            continue;

                        if (item.GetId () == item_b.GetId ())
                            continue;

                        if (item.GetLevel () == item_b.GetLevel ())
                        {
                            Position.Add (item.GetPosition ());
                            Position.Add (item_b.GetPosition ());

                            return Position;
                        }
                    }
                }
            }
        }

        return Position;
    }

    public bool IsMaxBaseActive ()
    {
        return _MaxBaseActive <= _NumberBaseActive;
    }

    #endregion

    #region FX

    public void FxDisplayEarnCoin (double profit_earn, int profit_unit_earn, Vector3 position)
    {
        FxDisplayGold (position,
                       ApplicationManager.Instance.AppendFromCashUnit (profit_earn, profit_unit_earn));

        Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, profit_earn, profit_unit_earn);

        this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
    }

    public void FxEarnCoin (double profit_earn, int profit_unit_earn, Vector3 position)
    {
        if (GameActionManager.Instance != null)
        {
            GameActionManager.Instance.InstanceFxCoins (position,
                                                        UIGameManager.Instance.GetPositionHubCoins (),
                                                        profit_earn,
                                                        profit_unit_earn);

            GameActionManager.Instance.FxDisplayGold (position,
                                                      string.Format ("+{0}", ApplicationManager.Instance.AppendFromCashUnit (profit_earn, profit_unit_earn)));
        }
        else
        {
            Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, profit_earn, profit_unit_earn);
            PlayerData.SaveCoins ();

            this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
        }
    }

    public void FxMoveNode (Vector3 position, Transform tf1, System.Action OnCompleted)
    {
        tf1.DOComplete (true);
        var tween = tf1.DOMove (position, Durations.DurationMoving);

        if (OnCompleted != null)
        {
            tween.OnComplete (() => OnCompleted ());
        }
    }

    public void FxPutNode (Transform tf1, System.Action OnCompleted)
    {
        tf1.DOComplete (true);
        tf1.localScale = new Vector3 (1.3f, 1.3f, 1.3f);

        var tween = tf1.DOScale (1, Durations.DurationTimeScale).SetEase (Ease.OutBack);

        if (OnCompleted != null)
        {
            tween.OnComplete (() => OnCompleted ());
        }
    }

    public void FxTapNode (Transform tf1, System.Action OnCompleted)
    {
        tf1.DOComplete (true);
        tf1.localScale = new Vector3 (1.3f, 1.3f, 1.3f);

        var tween = tf1.DOScale (1, Durations.DurationTimeScale).SetEase (Ease.OutBack);

        if (OnCompleted != null)
        {
            tween.OnComplete (() => OnCompleted ());
        }
    }

    public void FxAppear (Transform tf, System.Action OnCompleted)
    {
        tf.DOComplete (true);

        tf.localScale = Vector3.zero;

        var tween = tf.DOScale (1, Durations.DurationTimeScale).SetEase (Ease.OutBack);

        if (OnCompleted != null)
        {
            tween.OnComplete (() => OnCompleted ());
        }
    }

    public void FxDeAppear (Transform tf, System.Action OnCompleted)
    {
        tf.DOComplete (true);

        var tween = tf.DOScale (0, Durations.DurationTimeScale).SetEase (Ease.InBack);

        if (OnCompleted != null)
        {
            tween.OnComplete (() => OnCompleted ());
        }
    }

    public void FxMergeTwo (Vector3 position, Transform tf1, Transform tf2, System.Action OnCompleted)
    {
        tf1.DOComplete (true);
        tf2.DOComplete (true);

        tf1.localPosition = position;
        tf2.localPosition = position;

        var tween1 = tf1.DOLocalMoveX (position.x - 1f, Durations.DurationMovingMerge);
        var tween2 = tf2.DOLocalMoveX (position.x + 1f, Durations.DurationMovingMerge);

        tween1.OnComplete (() => { tf1.DOLocalMoveX (position.x, Durations.DurationMovingMerge); });
        tween2.OnComplete (() =>
        {
            var tween = tf2.DOLocalMoveX (position.x, Durations.DurationMovingMerge);

            if (OnCompleted != null)
            {
                tween.OnComplete (() => { OnCompleted (); });
            }
        });
    }

    public void FxExploseGold ()
    {
        if (_FxExploseGold != null) _FxExploseGold.Play ();
    }

    public void FxDisplayGold (Vector3 position, string value)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxRaiseGold);

        if (fx == null)
            return;

        fx.GetComponent<FXCoin> ().Enable (position, value);
    }

    public void FXSunshine (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxSunshine, false);

        if (fx == null)
            return;

        fx.position = position;

        fx.gameObject.SetActive (true);
    }

    public void FxStarsExp (Vector3 start_position, Vector3 end_position, System.Action OnCompleted)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxStarExp, false);

        if (fx == null)
        {
            if (OnCompleted != null)
            {
                OnCompleted ();
            }

            return;
        }

        fx.DOComplete ();

        fx.position = start_position;
        fx.gameObject.SetActive (true);

        var tween = fx.DOMove (end_position, Durations.DurationMovingLine);

        tween.OnComplete (() =>
        {
            if (OnCompleted != null)
            {
                OnCompleted ();
            }

            PoolExtension.SetPool (PoolEnums.PoolId.FxStarExp, fx);

            GameActionManager.Instance.InstanceFxFireWork (end_position);
        });
    }

    public void FxShakeScale (Transform fx_transform)
    {
        fx_transform.DOComplete (fx_transform);
        fx_transform.DOPunchScale (Vector.Vector3PunchScale, Durations.DurationScale);
    }

    #endregion
}