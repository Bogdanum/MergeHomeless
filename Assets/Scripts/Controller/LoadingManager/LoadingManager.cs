using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Loading behaviour.
/// </summary>
public class LoadingManager : Singleton<LoadingManager>
{
    // =============================== References ============================== //

    [Header ("LAYOUT")]
    [SerializeField]
    private CanvasGroup _CanvasGroup;

    /// <summary>
    /// The user interface loading information.
    /// </summary>
    [SerializeField]
    private Text UILoadingInformation;
    
   

    private event System.Action _OnBeginLoading;
    private event System.Action _OnEndLoading;

    // =============================== Functional ============================== //

    #region Functional 

    public LoadingManager Execute (string sceneLoad, bool isFade = true, bool isUseSplashScreen = true, string message = "")
    {
        transform.gameObject.SetActive (true);

        if (UILoadingInformation != null) UILoadingInformation.text = message;

        _CanvasGroup.alpha = 0;

        StartCoroutine (LoadingTime (sceneLoad, isFade, isUseSplashScreen));

        return this;
    }

    public LoadingManager RegisterEvent (System.Action onBegin, System.Action onEnd)
    {
        _OnBeginLoading = onBegin;
        _OnEndLoading   = onEnd;

        return this;
    }

    private IEnumerator LoadingTime (string sceneLoad, bool isFade, bool isUseSplashScreen)
    {
        if (isUseSplashScreen)
        {
            if (isFade)
            {
                while (_CanvasGroup.alpha < 1)
                {
                    _CanvasGroup.alpha = Mathf.Clamp (_CanvasGroup.alpha + Time.smoothDeltaTime * 5, 0, 1);

                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }

        if (_OnBeginLoading != null)
        {
            _OnBeginLoading ();

            _OnBeginLoading = null;
        }


        if (isUseSplashScreen)
        {
            var async = SceneManager.LoadSceneAsync (sceneLoad, LoadSceneMode.Single);

            async.allowSceneActivation = false;

            var timeLoading = Time.time;

            while (async.progress < 0.9f)
            {
                yield return null;
            }

            _CanvasGroup.alpha = 1;

            if (isFade)
            {
                var distance = Mathf.Clamp (1f - (Time.time - timeLoading), 0f, 1f);

                yield return new WaitForSeconds (distance);
            }

            async.allowSceneActivation = true;

            yield return null;

            while (_CanvasGroup.alpha > 0)
            {
                _CanvasGroup.alpha = Mathf.Clamp (_CanvasGroup.alpha - Time.deltaTime * 5, 0, 1);

                yield return null;
            }
        }
        else
        {
            SceneManager.LoadScene (sceneLoad, LoadSceneMode.Single);
        }

        if (!ReferenceEquals (_OnEndLoading, null))
        {
            _OnEndLoading ();
        }
        
        transform.gameObject.SetActive (false);
    }

    #endregion
}