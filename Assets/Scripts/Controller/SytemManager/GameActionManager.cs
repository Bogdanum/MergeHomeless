using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameActionManager : Singleton<GameActionManager>
{
    #region Action

    public void SetExp (int exp)
    {
        if (PlayerData.Level == GameConfig._MaxLevel)
            return;

        PlayerData.Exp += exp;

        if (PlayerData.Exp >= Contains.ExpNeedReach)
        {
            PlayerData.Exp = PlayerData.Exp - Contains.ExpNeedReach;
            LevelUp ();
        }

        PlayerData.SaveExp ();
        UIGameManager.Instance.UpdateTextExp ();
    }

    public void LevelUp ()
    {
        PlayerData.Level = Mathf.Clamp (PlayerData.Level + 1, 0, GameConfig._MaxLevel);

        PlayerData.SaveLevel ();

        UIGameManager.Instance.UpdateTextLevel ();

        ApplicationManager.Instance.LoadLevelParameter ();
        GameManager.Instance.InstanceBase ();
        GameManager.Instance.UpdateBaseActiveForEarning ();
        GameManager.Instance.UpdateFreeList ();
        
        LevelPlayerManager.Instance.Init (PlayerData.Level);

        this.PostMissionEvent (MissionEnums.MissionId.ReachLevel);
    }

    public void SetMultiRewardCoins (int time)
    {
        if (PlayerData.TotalTimeMultiRewardCoins == 0)
        {
            PlayerData._LastTimeMultiRewardCoins = Helper.GetUtcTimeString ();
            PlayerData.SaveLastTimeMultiRewardCoins ();
        }

        PlayerData.TotalTimeMultiRewardCoins = Mathf.Clamp (PlayerData.TotalTimeMultiRewardCoins + time, 0, GameConfig.MaxTimeMultiRewardCoins);
        PlayerData.SaveTotalTimeMultiRewardCoins ();

        if (PlayerData.TotalTimeMultiRewardCoins > 0)
        {
            Contains.MultiRewardFromSpeed = GameData.Instance.MultiRewardData.UpgradeCoefficient + 1;
        }
        else
        {
            Contains.MultiRewardFromSpeed = GameConfig.ValueEarnCoinsMultiTime;
        }

        this.PostActionEvent (ActionEnums.ActionID.RefreshUIEquipments, EquipmentEnums.GetKey (EquipmentEnums.AbilityId.MultiRewardCoins));
    }

    #endregion


    #region FX

    public void FxDisplayGold (Vector3 position, string value)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxUIRaiseGold);

        if (fx == null)
            return;

        fx.GetComponent<FXCoin> ().Enable (position, value);
    }

    public void InstanceFxDiamonds (Vector3 start_position, Vector3 end_position, int diamonds)
    {
        var quantity   = 10;
        var balance    = diamonds % 10;
        var real_value = (diamonds - balance) / 10;

        if (diamonds > quantity && balance > 0)
        {
            PlayerData.Diamonds += balance;
        }
        else
        {
            if (diamonds <= quantity)
            {
                real_value = 1;
                quantity   = diamonds;
            }
        }

        for (int i = 0; i < quantity; i++)
        {
            var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxItem_Diamond);

            if (fx == null)
                continue;

            fx.transform.position = new Vector3 (start_position.x + Random.Range (2f, -2f),
                                                 start_position.y + Random.Range (2f, -2f));

            fx.transform.localScale = Vector3.zero;

            var tween = fx.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack).SetDelay (i * Durations.DurationScale / 3f);

            var index = i == quantity - 1;

            tween.OnComplete (() =>
            {
                var tween2 = fx.DOMove (end_position, Durations.DurationMovingBack).SetDelay (Durations.DurationScale).SetEase (Ease.InBack);

                tween2.OnComplete (() =>
                {
                    PoolExtension.SetPool (PoolEnums.PoolId.FxItem_Diamond, fx);

                    PlayerData.Diamonds += real_value;

                    Instance.InstanceFxFireWork (fx.position);
                    Instance.PlayAudioSound (AudioEnums.SoundId.Diamonds);
                    Instance.PostActionEvent (ActionEnums.ActionID.RefreshUIDiamonds);

                    UIGameManager.Instance.FxShakeDiamonds ();

                    if (index == true)
                    {
                        PlayerData.SaveDiamonds ();
                    }
                });
            }).SetEase (Ease.InBack);
        }
    }

    public void InstanceFxDiamonds (Vector3 startPosition, Vector3 endPosition, int quantity, System.Action OnCompleted = null)
    {
        var max_quantity = Mathf.Clamp (quantity, 0, 10);

        for (int i = 0; i < max_quantity; i++)
        {
            var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxItem_Diamond);

            if (fx == null)
                continue;

            fx.transform.position = new Vector3 (startPosition.x + Random.Range (2f, -2f),
                                                 startPosition.y + Random.Range (2f, -2f));

            fx.transform.localScale = Vector3.zero;

            var tween = fx.DOScale (1, Durations.DurationScale).SetEase (Ease.OutBack).SetDelay (i * Durations.DurationScale / 3f);

            var index = i == max_quantity - 1;

            tween.OnComplete (() =>
            {
                var tween2 = fx.DOMove (endPosition, Durations.DurationMovingBack).SetDelay (Durations.DurationScale).SetEase (Ease.InBack);

                tween2.OnComplete (() =>
                {
                    PoolExtension.SetPool (PoolEnums.PoolId.FxItem_Diamond, fx);

                    Instance.InstanceFxFireWork (fx.position);

                    Instance.PlayAudioSound (AudioEnums.SoundId.Diamonds);

                    if (index == true)
                    {
                        if (OnCompleted != null)
                        {
                            OnCompleted ();
                        }
                    }
                });
            }).SetEase (Ease.InBack);
        }
    }

    public void InstanceFxCoins (Vector3 start_position, Vector3 end_position, double coins, int unit)
    {
        var quantity = 6;

        if (coins < 6 && unit == 0)
        {
            quantity = (int) coins;
        }

        var real_coins = coins / quantity;
        var real_unit  = unit;

        Helper.FixNumber (ref real_coins, ref real_unit);

        for (int i = 0; i < quantity; i++)
        {
            var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxItem_Coins);

            if (fx == null)
                continue;

            fx.transform.position = new Vector3 (start_position.x + Random.Range (2f, -2f),
                                                 start_position.y + Random.Range (2f, -2f));

            fx.transform.localScale = Vector3.zero;

            var index = i == quantity - 1;

            var tween = fx.DOScale (1, Durations.DurationScale / 2f).SetEase (Ease.OutBack).SetDelay (i * Durations.DurationScale / 3f);

            tween.OnComplete (() =>
            {
                var tween2 = fx.DOMove (end_position, Durations.DurationMovingLine).SetDelay (Durations.DurationScale / 2f).SetEase (Ease.InBack);

                tween2.OnComplete (() =>
                {
                    PoolExtension.SetPool (PoolEnums.PoolId.FxItem_Coins, fx);

                    GameManager.Instance.ShakeCoins ();

                    Helper.AddValue (ref PlayerData.Coins, ref PlayerData.CoinUnit, real_coins, real_unit);

                    Instance.InstanceFxFireWork (fx.position);
                    Instance.PostActionEvent (ActionEnums.ActionID.RefreshUICoins);
                    Instance.PlayAudioSound (AudioEnums.SoundId.Coins);

                    if (index == true)
                    {
                        PlayerData.SaveCoins ();
                    }
                });
            }).SetEase (Ease.InBack);
        }
    }

    public void InstanceFxCoins (Vector3 startPosition, Vector3 endPosition, int quantity, System.Action OnCompleted)
    {
        var value = Mathf.Clamp (quantity, 0, 6);

        for (int i = 0; i < value; i++)
        {
            var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxItem_Coins);

            if (fx == null)
                continue;

            fx.transform.position = new Vector3 (startPosition.x + Random.Range (2f, -2f),
                                                 startPosition.y + Random.Range (2f, -2f));

            fx.transform.localScale = Vector3.zero;

            var tween = fx.DOScale (1, Durations.DurationScale / 2f).SetEase (Ease.OutBack).SetDelay (i * Durations.DurationScale / 3f);

            var index = i == value - 1;

            tween.OnComplete (() =>
            {
                var tween2 = fx.DOMove (endPosition, Durations.DurationMovingLine).SetDelay (Durations.DurationScale / 2f).SetEase (Ease.InBack);

                tween2.OnComplete (() =>
                {
                    PoolExtension.SetPool (PoolEnums.PoolId.FxItem_Coins, fx);

                    Instance.InstanceFxFireWork (fx.position);

                    if (index == true)
                    {
                        if (OnCompleted != null)
                        {
                            OnCompleted ();
                        }
                    }

                    GameManager.Instance.ShakeCoins ();

                    Instance.PlayAudioSound (AudioEnums.SoundId.Coins);
                });
            }).SetEase (Ease.InBack);
        }
    }

    public void InstanceFxFireWork (Vector3 position)
    {
        var fxFirework = PoolExtension.GetPool (PoolEnums.PoolId.FxExplode_Firework, false);

        if (fxFirework == null)
            return;

        fxFirework.transform.position = position;
        fxFirework.gameObject.SetActive (true);
    }

    public void InstanceFxTap (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxTapUpgrade, false);

        if (fx == null)
            return;

        fx.transform.position = position;
        fx.gameObject.SetActive (true);
    }

    public void InstanceFxTapFlower (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxTapFlower, false);

        if (fx == null)
            return;

        fx.transform.position = position;
        fx.gameObject.SetActive (true);
    }

    public void InstanceFxTapCoins (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxTapCoins, false);

        if (fx == null)
            return;

        fx.transform.position = position;
        fx.gameObject.SetActive (true);
    }

    public void InstanceFxTapBox (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxTapBox, false);

        if (fx == null)
            return;

        fx.transform.position = position;
        fx.gameObject.SetActive (true);
    }

    public void InstanceFxTapDiamonds (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxTapDiamonds, false);

        if (fx == null)
            return;

        fx.transform.position = position;
        fx.gameObject.SetActive (true);
    }

    public void InstanceFxLevelUpItems (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxLevelUpItem, false);

        if (fx == null)
            return;

        var script = fx.GetComponent<FxLevelUp> ();

        script.Init (position);

        fx.gameObject.SetActive (true);
    }

    public void InstanceFxLevelUp (Vector3 position)
    {
        var fx = PoolExtension.GetPool (PoolEnums.PoolId.FxLevelUp, false);

        if (fx == null)
            return;

        var script = fx.GetComponent<FxLevelUp> ();

        script.Init (position);

        fx.gameObject.SetActive (true);
    }

    #endregion
}