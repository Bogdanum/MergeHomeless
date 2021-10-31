using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header ("Hand")] [SerializeField] private Animation     _AnimationHand;
    [SerializeField]                   private AnimationClip _AnimationPress, _AnimationNormal;
    [SerializeField]                   private Transform     _TransformHand;

    [Header ("Optionals")] [SerializeField] [Tooltip ("Support call the tutorial after the long time player don't have any interact with the game.")]
    private bool IsAutoTutorials;

    [Range (5f, 20f)] [SerializeField] private float _TimeForRecallBack;

    [Header ("Message")] [SerializeField] private Transform   _TransformBoardMessage;
    [SerializeField]                      private CanvasGroup _PanelMaskMessage;
    [SerializeField]                      private Transform   _TransformCars;
    [SerializeField]                      private Transform   _TransformMessagePanel;
    [SerializeField]                      private Text        _TextMessageDisplay;


    private TutorialEnums.TutorialId _TutorialId;
    private bool                     IsPostCompleted;
    private bool                     IsTapAnyWhere;
    private float                    _TimeRecall;

    private bool _IsShowingMessage;

    #region System

    protected override void Awake ()
    {
        base.Awake ();

        InitSystem ();
    }

    protected override void OnDestroy ()
    {
        IsAutoTutorials = false;

        base.OnDestroy ();
    }

    #endregion

    #region Controller

    private void InitSystem ()
    {
        if (IsAutoTutorials)
        {
            Timing.RunCoroutine (_EnumeratorTutorialCallToAction ());
        }
    }

    #endregion

    #region Action

    public void ExecuteMessage (params string[] message)
    {
        if (_IsShowingMessage)
            return;

        Timing.RunCoroutine (_EnumeratorMessage (message));
    }

    public void ExecuteTutorial (TutorialEnums.TutorialId id)
    {
        if (_TutorialId == id)
            return;

        _TutorialId = id;

        switch (id)
        {
            case TutorialEnums.TutorialId.HowToPlayGame:
                Timing.RunCoroutine (_EnumeratorHowToPlayGame ());
                break;
            case TutorialEnums.TutorialId.HowToMerge:
                Timing.RunCoroutine (_EnumeratorHowToMerge ());
                break;
            case TutorialEnums.TutorialId.HowToEarningCoins:
                Timing.RunCoroutine (_EnumeratorHowToEarningCoins ());
                break;
            case TutorialEnums.TutorialId.HowToTouchBox:
                Timing.RunCoroutine (_EnumeratorHowToTouchBox ());
                break;
            case TutorialEnums.TutorialId.HowToSpeedUp:
                Timing.RunCoroutine (_EnumeratorHowToSpeedUp ());
                break;
            case TutorialEnums.TutorialId.HowToTouchItemBack:
                break;
            default:
                _TutorialId = TutorialEnums.TutorialId.None;
                break;
        }
    }

    public void SetHandPress (Vector3 position)
    {
        if (_TransformHand.gameObject.activeSelf == false) _TransformHand.gameObject.SetActive (true);

        _TransformHand.DOKill ();
        _TransformHand.position = position;

        _AnimationHand.Play (_AnimationPress.name);
    }

    public void SetHandNormal (Vector3 position)
    {
        if (_TransformHand.gameObject.activeSelf == false) _TransformHand.gameObject.SetActive (true);

        _TransformHand.DOKill ();
        _TransformHand.position = position;

        _AnimationHand.Play (_AnimationNormal.name);
    }

    public void SetHandMoving (Vector3 start_position, Vector3 end_position)
    {
        if (_TransformHand.gameObject.activeSelf == false) _TransformHand.gameObject.SetActive (true);

        _TransformHand.DOKill ();
        _TransformHand.position = start_position;
        _TransformHand.DOMove (end_position, Durations.DurationMovingHand).SetLoops (-1, LoopType.Restart);
        _AnimationHand.Play (_AnimationNormal.name);
    }

    public void DisableHand ()
    {
        if (_TransformHand.gameObject.activeSelf == true) _TransformHand.gameObject.SetActive (false);

        _TransformHand.DOKill ();
    }

    public void SetStateCompleted (bool state)
    {
        IsPostCompleted = state;
    }

    public void PostCompleted (TutorialEnums.TutorialId id)
    {
        if (_TutorialId != id)
            return;

        IsPostCompleted = true;
    }

    public void PostTapAnyWhere ()
    {
        IsTapAnyWhere = true;
        _TimeRecall   = 0;
    }

    #endregion

    #region IEnumerator

    private IEnumerator<float> _EnumeratorTutorialCallToAction ()
    {
        while (_TutorialId != TutorialEnums.TutorialId.None)
        {
            yield return Timing.WaitForOneFrame;
        }

        while (IsAutoTutorials)
        {
            _TimeRecall += 1;

            if (_TimeRecall < _TimeForRecallBack)
                yield return Timing.WaitForSeconds (1f);
            else
            {
                _TimeRecall = 0;

                if (_TutorialId == TutorialEnums.TutorialId.None)
                {
                    var random_tip = Random.Range (0, 4);

                    switch (random_tip)
                    {
                        case 0:
                            _TutorialId = TutorialEnums.TutorialId.HowToTouchBox;
                            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox ()));
                            break;
                        case 1:
                            _TutorialId = TutorialEnums.TutorialId.HowToMerge;
                            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToMerge ()));
                            break;
                        case 2:
                            _TutorialId = TutorialEnums.TutorialId.HowToEarningCoins;
                            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToEarningCoins ()));
                            break;
                        case 3:
                            _TutorialId = TutorialEnums.TutorialId.HowToSpin;
                            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToSpin ()));
                            break;
                        default:
                            _TutorialId = TutorialEnums.TutorialId.HowToTouchBox;
                            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox ()));
                            break;
                    }

                    _TutorialId = TutorialEnums.TutorialId.None;
                }

                yield return Timing.WaitForSeconds (1f);
            }
        }
    }

    private IEnumerator<float> _EnumeratorHowToPlayGame ()
    {
        if (PlayerData.Coins < 1000 && PlayerData.CoinUnit == 0)
        {
            PlayerData.Coins = 1000;
            PlayerData.SaveCoins ();
            
            this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
        }

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                    new List<string> ()
                                                                    {
                                                                        ApplicationLanguage.Text_description_welcome_racer,
                                                                        ApplicationLanguage.Text_description_racing,
                                                                        ApplicationLanguage.Text_description_racing_II,
                                                                        ApplicationLanguage.Text_description_go_go
                                                                    }
                                                                )));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox (true)));
        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox (true)));


        if (!GameMovingManager.Instance.IsMovingCars ())
        {
            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                        new List<string> ()
                                                                        {
                                                                            ApplicationLanguage.Text_description_tutorial_merge
                                                                        }
                                                                    )));

            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToMerge (true)));

            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                        new List<string> ()
                                                                        {
                                                                            ApplicationLanguage.Text_description_tutorial_earning
                                                                        }
                                                                    )));

            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToEarningCoins (true)));
        }

        if (GameMovingManager.Instance.IsMovingCars ())
        {
            yield return Timing.WaitForSeconds (4f);
            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchItemBack (true)));
        }

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox (true)));
        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox (true)));

        if (!GameMovingManager.Instance.IsMovingCars ())
        {
            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToMerge (true)));
        }

        if (!GameMovingManager.Instance.IsMovingCars ())
        {
            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToMerge (true)));
            yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToEarningCoins (true)));
        }


        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                    new List<string> ()
                                                                    {
                                                                        ApplicationLanguage.Text_description_tutorial_upgrade
                                                                    }
                                                                )));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToUpgradeItems ()));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                    new List<string> ()
                                                                    {
                                                                        ApplicationLanguage.Text_description_tutorial_speed_up
                                                                    }
                                                                )));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToSpeedUp ()));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorMessage (
                                                                    new List<string> ()
                                                                    {
                                                                        ApplicationLanguage.Text_description_tutorial_start_game
                                                                    }
                                                                )));

        yield return Timing.WaitUntilDone (Timing.RunCoroutine (_EnumeratorHowToTouchBox (true)));

        ApplicationLanguage.Instance.ChangeLanguage (LanguageEnums.GetLanguageDevice ());

        OnCompletedTutorial ();
    }

    private IEnumerator<float> _EnumeratorHowToTouchBox (bool IsLockUi = false)
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (!GameManager.Instance.IsFreeIndexGrid ())
        {
            yield break;
        }

        if (IsLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, false);
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractGame, false);
        }

        SetHandPress (UIGameManager.Instance.GetPositionTouchBox ());

        if (IsLockUi)
        {
            IsPostCompleted = false;

            while (!IsPostCompleted)
            {
                yield return Timing.WaitForOneFrame;
            }

            yield return Timing.WaitForSeconds (0.25f);

            GameManager.Instance.ForceUnlockAllBoxes ();
        }
        else
        {
            yield return Timing.WaitForSeconds (0.3f);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        if (IsLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, true);
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractGame, true);
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorHowToMerge (bool isLockUi = false)
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        GameManager.Instance.ForceUnlockAllBoxes ();

        var item_position_merge = GameManager.Instance.GetRandomFreeToUseItemNode ();

        if (item_position_merge.Count < 2)
            yield break;

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, false);
        }

        SetHandMoving (item_position_merge[0], item_position_merge[1]);

        if (isLockUi)
        {
            IsPostCompleted = false;

            while (!IsPostCompleted)
            {
                yield return Timing.WaitForOneFrame;
            }
        }
        else
        {
            yield return Timing.WaitForSeconds (0.3f);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, true);
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorHowToEarningCoins (bool isLockUi = false)
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (GameManager.Instance.IsMaxBaseActive ())
            yield break;

        GameManager.Instance.ForceUnlockAllBoxes ();

        var item_position_free = GameManager.Instance.GetPositionFreeItemNode ();

        if (item_position_free == Vector.Vector3Null)
            yield break;

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, false);
        }

        SetHandMoving (item_position_free, GameManager.Instance.GetPositionStartLine ());

        if (isLockUi)
        {
            IsPostCompleted = false;

            while (!IsPostCompleted)
            {
                yield return Timing.WaitForOneFrame;
            }
        }
        else
        {
            yield return Timing.WaitForSeconds (0.3f);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, true);
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorHowToTouchItemBack (bool isLockUi = false)
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        GameManager.Instance.ForceUnlockAllBoxes ();

        var item_position_busy = GameManager.Instance.GetPositionBusyItemNode ();

        if (item_position_busy == Vector.Vector3Null)
            yield break;

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, false);
        }

        SetHandPress (item_position_busy);

        if (isLockUi)
        {
            IsPostCompleted = false;

            while (!IsPostCompleted)
            {
                yield return Timing.WaitForOneFrame;
            }
        }
        else
        {
            yield return Timing.WaitForSeconds (0.3f);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        if (isLockUi)
        {
            this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, true);
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorMessage (IList<string> message)
    {
        _IsShowingMessage = true;

        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, false);
        this.PostActionEvent (ActionEnums.ActionID.SetStateInteractGame, false);

        if (_TransformBoardMessage.gameObject.activeSelf == false)
        {
            _TransformBoardMessage.gameObject.SetActive (true);
        }

        _PanelMaskMessage.DOComplete ();
        _PanelMaskMessage.DOFade (1, Durations.DurationFade);

        _PanelMaskMessage.alpha = 0;

        yield return Timing.WaitForSeconds (Durations.DurationFade);

        if (_TransformCars.gameObject.activeSelf == false)
        {
            _TransformCars.DOComplete ();
            _TransformCars.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);

            _TransformCars.localScale = Vector.Vector3Zero;
            _TransformCars.gameObject.SetActive (true);

            yield return Timing.WaitForSeconds (1f);
        }

        this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);

        if (_TransformMessagePanel.gameObject.activeSelf == false)
        {
            _TransformMessagePanel.localScale = Vector.Vector3Zero;
            _TransformMessagePanel.gameObject.SetActive (true);
        }

        for (int i = 0; i < message.Count; i++)
        {
            _TransformMessagePanel.DOComplete ();
            _TransformMessagePanel.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack);

            _TextMessageDisplay.text = message[i];

            this.PlayAudioSound (AudioEnums.SoundId.ItemTouchTalk);

            yield return Timing.WaitForSeconds (0.5f);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }

            _TransformMessagePanel.DOComplete ();
            _TransformMessagePanel.DOScale (0, Durations.DurationScale).SetEase (Ease.InBack);

            yield return Timing.WaitForSeconds (Durations.DurationScale);
        }

        _TransformBoardMessage.DOScale (0, Durations.DurationScale).SetEase (Ease.InBack);
        _TransformBoardMessage.DOScale (0, Durations.DurationScale).SetEase (Ease.InBack);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        if (_TransformCars.gameObject.activeSelf)
        {
            _TransformCars.gameObject.SetActive (false);
        }

        if (_TransformMessagePanel.gameObject.activeSelf)
        {
            _TransformMessagePanel.gameObject.SetActive (false);
        }

        _PanelMaskMessage.DOFade (0, Durations.DurationScale);

        yield return Timing.WaitForSeconds (Durations.DurationScale);

        if (_TransformBoardMessage.gameObject.activeSelf == true)
        {
            _TransformBoardMessage.gameObject.SetActive (false);
        }

        this.PostActionEvent (ActionEnums.ActionID.SetStateInteractUI, true);
        this.PostActionEvent (ActionEnums.ActionID.SetStateInteractGame, true);

        _TextMessageDisplay.text = string.Empty;
        _IsShowingMessage        = false;
    }

    private IEnumerator<float> _EnumeratorHowToSpin ()
    {
        yield break;
    }

    private IEnumerator<float> _EnumeratorHowToSpeedUp ()
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        var position = UIGameManager.Instance.GetPositionSpeedUpButton ();

        SetHandPress (position);

        IsTapAnyWhere = false;

        while (!IsTapAnyWhere)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (!SpeedUpManager.IsEnabled)
        {
            yield return Timing.WaitForSeconds (0.1f);
        }

        if (SpeedUpManager.IsEnabled)
        {
            if (PlayerData.Diamonds < GameConfig._DiamondBuySpeedUp)
            {
                PlayerData.Diamonds += GameConfig._DiamondBuySpeedUp;

                this.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);
            }

            position = SpeedUpManager.Instance.GetPositionSpeedUpWithDiamonds ();

            SetHandPress (position);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorHowToUpgradeItems ()
    {
        while (ApplicationManager.Instance.IsDialogEnable)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds (0.5f);

        var position = UIGameManager.Instance.GetPositionShopButton ();

        SetHandPress (position);

        IsTapAnyWhere = false;

        while (!IsTapAnyWhere)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (!ShopManager.IsEnabledItemShop)
        {
            yield return Timing.WaitForSeconds (0.1f);
        }

        if (ShopManager.IsEnabledItemShop)
        {
            if (PlayerData.Coins < 1000 && PlayerData.CoinUnit == 0)
            {
                PlayerData.Coins += 1000;

                this.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
            }

            position = ShopManager.Instance.GetPositionUpgradeItems ();

            SetHandPress (position);

            IsTapAnyWhere = false;

            while (!IsTapAnyWhere)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        DisableHand ();
    }

    private IEnumerator<float> _EnumeratorHowToUpgradeEquipments ()
    {
        yield break;
    }

    #endregion

    #region Callback

    private void OnCompletedTutorial ()
    {
        PlayFabManager.Instance.ShowInputNameWindow();

        PlayerData.SaveTutorial (_TutorialId, true);

        _TutorialId = TutorialEnums.TutorialId.None;
    }

    #endregion
}