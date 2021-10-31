using UnityEngine;
using UnityEngine.UI;

public class UISpeedUpHub : MonoBehaviour
{
    [Header ("Hud Menu")] [SerializeField] private Transform _TransformTimeProcess;

    [Header ("Hud Menu")] [SerializeField] private Transform _TransformSpeedUpIcon;

    [Header ("Hud Menu")] [SerializeField] private Text _TextProcessTimeOnHud;

    private System.Action<bool>   HandleStateSpeedUpHub;
    private System.Action<string> HandleStateSpeedUpTime;

    #region System

    private void OnEnable ()
    {
        RegisterSpeedHub ();

        OnStateSpeedUpHub (SpeedUpManager.InstanceAwake ().IsEnableSpeedUp ());
    }

    private void OnDisable ()
    {
        UnRegisterSpeedHub ();
    }

    #endregion

    #region Controller

    private void RegisterSpeedHub ()
    {
        HandleStateSpeedUpHub  = OnStateSpeedUpHub;
        HandleStateSpeedUpTime = OnUpdateTimeSpeedUpHub;

        SpeedUpManager.InstanceAwake ().RegisterSpeedUpHub (HandleStateSpeedUpHub);
        SpeedUpManager.InstanceAwake ().RegisterSpeedUpTime (HandleStateSpeedUpTime);
    }

    private void UnRegisterSpeedHub ()
    {
        if (SpeedUpManager.Instance == null) return;
        SpeedUpManager.Instance.UnRegisterSpeedUpHub (HandleStateSpeedUpHub);
        SpeedUpManager.Instance.UnRegisterSpeedUpTime (HandleStateSpeedUpTime);
    }

    private void OnStateSpeedUpHub (bool obj)
    {
        _TransformSpeedUpIcon.gameObject.SetActive (!obj);
        if (_TransformTimeProcess != null) _TransformTimeProcess.gameObject.SetActive (obj);
    }

    private void OnUpdateTimeSpeedUpHub (string time)
    {
        if (_TextProcessTimeOnHud != null) _TextProcessTimeOnHud.text = time;
    }

    #endregion
}