using Firebase;
using Firebase.Analytics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FirebaseInit))]
public class FirebaseInitHelp : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Если при сборке выдает ошибку: FAILURE: Build failed with an exception. A problem occurred configuring project ':launcher'." +
            " > android.defaultConfig.versionCode is set to 0, but it should be a positive integer. ---> удалите папку Temp в папке проекта и измените версию Player Settings/Bundle Version Code", MessageType.Warning);

        base.OnInspectorGUI();
    }
}
#endif

public class FirebaseInit : Singleton<FirebaseInit>
{
    protected override void Awake()
    {
        base.Awake();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });
    }
}
