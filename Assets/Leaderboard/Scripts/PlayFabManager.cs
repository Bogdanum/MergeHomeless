using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using MEC;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayFabManager))]
public class PlayFabManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Пожалуйста посетите сайт developer.playfab.com и проверьте правильность введенного названия таблицы лидеров", MessageType.Warning);
        EditorGUILayout.HelpBox("Если не отправляются данные на сервер (в таблицу лидеров, данные пользователя и тд.) --> в Title Settings игры" +
                                " на developer.playfab.com в разделе API Features поставьте галочку на Allow client to post player statistics", MessageType.Info);
        base.OnInspectorGUI();
    }
}
#endif


public class PlayFabManager : Singleton<PlayFabManager>
{
    //public string BestScoreKey = "BestScore";
    [Header("Название таблицы лидеров")]
    public string _leaderboardName = "BestPlayers";

    #region UI

    #region Лидерборд
    [Header("префаб строки(Player)")] public GameObject rowPrefab;
    [Header("Количество игроков в таблице")] [Range(10, 1000)] public int playersCountAll = 100;
    [Header("Количество ближних игроков в таблице")] [Range(10, 1000)] public int playersCountAround = 100;
    [Header("Задержка перед появлением окна ввода ника после туториала")] [Range(0, 5)] public float delay = 1.5f;
    [Header("Цвет выделения игрока в таблице")] public Color color;
    [Header("UI GameObjects")]
    [SerializeField] private GameObject _leaderboard;
    [SerializeField] private GameObject _leaderbordAccessPanel;
    [SerializeField] private Transform _leaderboardContent;
    [SerializeField] private Button _showAroundPlayers;
    [SerializeField] private Button _showAllPlayers;
    [SerializeField] private GameObject _LoadingLB;

    private Color GoldColor = new Color32(255, 215, 0, 255);
    private Color SilverColor = new Color32(192, 192, 192, 255);
    private Color BronzeColor = new Color32(205, 127, 50, 255);
    #endregion

    #region Всплывающее окно для ввода ника после первой смерти
    [SerializeField] private GameObject _inputName;
    [SerializeField] private InputField _inputFieldWidow;
    [SerializeField] private GameObject _inpWindowErrorText;
    [SerializeField] private GameObject _inpWindowForb;
    [SerializeField] private Button _submtBtn;
    #endregion

    #region Поле для ввода/отображения ника в меню
    [SerializeField] private InputField _inputField;
    [SerializeField] private Text _nameMenu;
    [SerializeField] private Text _tryInpName;
    #endregion

    [SerializeField] private Button _Leaderboard;
    [SerializeField] private Text _PlayerPosButton;

    [Space(10)] [SerializeField] private InternetAccess internetAccess;
    #endregion

    #region Временные массивы и тп
    private string[] forbiddenWords;
    private TextAsset textAsset;
    string _loggedInPlayfabId;
    #endregion

    private string NICKNAME
    {
        get { return PlayerPrefs.GetString("NICKNAME", NameGenerator()); }
        set { PlayerPrefs.SetString("NICKNAME", value); }
    }

    public static int OldPosition
    {
        get { return PlayerPrefs.GetInt("OldPosition"); }
        private set { PlayerPrefs.SetInt("OldPosition", value); }
    }

    private string CurrPlayerPos
    {
        get { return PlayerPrefs.GetString("CurrPos", ""); }
        set { PlayerPrefs.SetString("CurrPos", value); }
    }

    private void Init()
    {
        StartCoroutine(internetAccess.TestConnection(result =>
        {
            PlayerPrefs.SetInt("InternetConnection", result ? 1 : 0);
            if (!result)
            {
                Debug.LogError("У вас нет интернета!");
                SetupGameWithoutInternet();
            }
            else
                Login();
        }));
        _loggedInPlayfabId = PlayerPrefs.GetString("UserPlayfabID");
        _PlayerPosButton.text = CurrPlayerPos;
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private string CheckUserID()
    {
        string deviceID;
        if (!PlayerPrefs.HasKey("DeviceID"))
        {
            deviceID = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("DeviceID", deviceID);
        }
        else
        {
            deviceID = PlayerPrefs.GetString("DeviceID");
        }
        return deviceID;
    }

    private void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = CheckUserID(),
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request,
        result =>
        {
            PlayerPrefs.SetString("UserPlayfabID", result.PlayFabId);
            if (string.IsNullOrEmpty(_loggedInPlayfabId)) _loggedInPlayfabId = result.PlayFabId;
            Debug.LogWarning("Successful login/accaunt create!");
            if (result.InfoResultPayload.PlayerProfile != null)
            {
                PlayerPrefs.SetString("NICKNAME", result.InfoResultPayload.PlayerProfile.DisplayName);
                _nameMenu.text = result.InfoResultPayload.PlayerProfile.DisplayName;
            }
            else
            {
                PlayerPrefs.SetInt("FirstGame", 1);
            }
            SetupGameWitInternet();
        }, OnError => {
            Debug.Log("Error while logging in/creating account!");
            Debug.Log(OnError.GenerateErrorReport());
            SetupGameWithoutInternet();
        });
    }

    private void SetupGameWithoutInternet()
    {
        if (PlayerPrefs.HasKey("UserName")) _nameMenu.text = NICKNAME;
        else _nameMenu.text = "Racer";
        _Leaderboard.onClick.AddListener(delegate { _leaderbordAccessPanel.SetActive(true); });
    }

    private void SetupGameWitInternet()
    {
        SendLeaderboard(PlayerData.Level);
        GetUserData(_loggedInPlayfabId);
        if (_nameMenu.text == string.Empty) _nameMenu.text = "Racer";
        if (PlayerPrefs.GetInt("FirstGame", 0) == 1)
        {
            PlayerPrefs.SetInt("FirstGame", 0);
        }
        _submtBtn.onClick.AddListener(delegate { SubmitNameButtonWindow(); });
        _Leaderboard.onClick.AddListener(delegate
        {
            GetLeaderboard();
#if !UNITY_EDITOR
                // Add 21.10.2021 ----------Firebase
                Firebase.Analytics.FirebaseAnalytics.LogEvent("open_leaderboard", "open_leaderboard", "true");
                // --------------------------------
#endif
        });
        _showAroundPlayers.onClick.AddListener(delegate
        {
            GetLeaderboardAroundPlayer();
            _showAllPlayers.gameObject.SetActive(true);
            _showAroundPlayers.gameObject.SetActive(false);
        });
        _showAllPlayers.onClick.AddListener(delegate
        {
            GetLeaderboard();
            _showAroundPlayers.gameObject.SetActive(true);
            _showAllPlayers.gameObject.SetActive(false);
        });
    }

    bool CheckHighOrLowIqUser(string input)
    {
        if (input.IndexOf(' ') > -1)
        {
            return true;
        }
        else { return false; }
    }

    public void SendLeaderboard(int score)
    {
#if !UNITY_EDITOR
            // add 21.10.2021 ------ Firebase
            if (score == 10) Firebase.Analytics.FirebaseAnalytics.LogEvent("get_10_level", "10_level", score);
            if (score == 30) Firebase.Analytics.FirebaseAnalytics.LogEvent("get_30_level", "30_level", score);
            // ------------------------------
#endif
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = _leaderboardName,
                        Value = score
                    }
                }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError => { Debug.LogWarning("Leaderboard error! LB name: " + _leaderboardName); });
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Sucessfull leaderboard send (" + _leaderboardName + ")");
        GetBestScore();
    }

    #region ЛИДЕРБОРД ДЛЯ НОРМАЛЬНЫХ ЛЮДЕЙ

    public void GetLeaderboard()
    {
        _leaderboard.SetActive(true);
        if (_leaderboardContent.childCount == 0) _LoadingLB.SetActive(true);

        var request = new GetLeaderboardRequest
        {
            StatisticName = _leaderboardName,
            StartPosition = 0,
            MaxResultsCount = playersCountAll
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError => { });
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        _LoadingLB.SetActive(false);
        foreach (Transform item in _leaderboardContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in result.Leaderboard)
        {
            string name = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : "Racer";
            GameObject newGo = Instantiate(rowPrefab, _leaderboardContent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = name;
            texts[2].text = item.StatValue.ToString();

            Image trofImage = newGo.GetComponentInChildren<Image>();

            if (item.PlayFabId == _loggedInPlayfabId)
            {
                texts[0].color = color;
                texts[1].color = color;
                //texts[2].color = color;
            }
            if (item.Position == 0) { trofImage.color = GoldColor; }
            else
            if (item.Position == 1) { trofImage.color = SilverColor; }
            else
            if (item.Position == 2) { trofImage.color = BronzeColor; }

        }
    }

    public void GetLeaderboardAroundPlayer()
    {

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = _leaderboardName,
            MaxResultsCount = playersCountAround
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError => { });
    }

    void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
    {

        foreach (Transform item in _leaderboardContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            string name = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : "Racer";
            GameObject newGo = Instantiate(rowPrefab, _leaderboardContent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = name;
            texts[2].text = item.StatValue.ToString();

            Image trofImage = newGo.GetComponentInChildren<Image>();

            if (item.PlayFabId == _loggedInPlayfabId)
            {
                texts[0].color = color;
                texts[1].color = color;
                //texts[2].color = color;
            }
            if (item.Position == 0) { trofImage.color = GoldColor; }
            else
            if (item.Position == 1) { trofImage.color = SilverColor; }
            else
            if (item.Position == 2) { trofImage.color = BronzeColor; }
        }
    }

    #endregion ЛИДЕРБОРД ДЛЯ НОРМАЛЬНЫХ ЛЮДЕЙ

    public void SubmitNameButton()
    {
        if (_inputField.text == "" || CheckHighOrLowIqUser(_inputField.text))
        {
            _inputField.text = "" + NameGenerator();
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = CheckForbiddenWords(_inputField.text),
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError => { });
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name");
        _inputField.text = result.DisplayName;
        _nameMenu.text = result.DisplayName;
        NICKNAME = result.DisplayName;
        _tryInpName.text = result.DisplayName;
    }

    public void SubmitNameButtonWindow()
    {

        if (_inputFieldWidow.text == "" || CheckHighOrLowIqUser(_inputFieldWidow.text))
        {
            _inputFieldWidow.text = "" + NameGenerator();
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = CheckForbiddenWords(_inputFieldWidow.text),
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdateWindow, OnError =>
        {
            _inpWindowErrorText.SetActive(true);
            _inputField.text = NameGenerator();
        });

    }

    void OnDisplayNameUpdateWindow(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated window display name");
        Debug.LogWarning(result.DisplayName);
        if (result.DisplayName == _inputFieldWidow.text)
            _inputName.SetActive(false);
        _inputField.text = result.DisplayName;
        _nameMenu.text = result.DisplayName;
        NICKNAME = result.DisplayName;
        _tryInpName.text = result.DisplayName;
    }

    void GetBestScore()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = _leaderboardName,
            StartPosition = 0,
            MaxResultsCount = playersCountAll
        };
        PlayFabClientAPI.GetLeaderboard(request, GetFuckingNigerBestScore, OnError => { });
    }

    void GetFuckingNigerBestScore(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            if (item.PlayFabId == _loggedInPlayfabId)
            {
                _PlayerPosButton.text = (item.Position + 1).ToString();
                CurrPlayerPos = (item.Position + 1).ToString();
            }
        }
    }

    public string NameGenerator()
    {
        char[] vowels = "aeuoyi".ToCharArray();
        char[] consonants = "qwrtpsdfghjklzxcvbnm".ToCharArray();

        char[] newNickLength = new char[Random.Range(3, 10)];
        StringBuilder newNick = new StringBuilder();

        while (newNick.Length < newNickLength.Length)
        {
            bool firstVowel = Random.Range(0, 2) == 0 ? true : false;

            if (firstVowel)
            {
                newNick.Append(vowels[Random.Range(0, vowels.Length)]);
                newNick.Append(consonants[Random.Range(0, consonants.Length)]);
            }
            else
            {
                newNick.Append(consonants[Random.Range(0, consonants.Length)]);
                newNick.Append(vowels[Random.Range(0, vowels.Length)]);
            }
        }
        if (newNickLength.Length % 2 != 0) newNick.Remove(newNick.Length - 1, 1);
        newNick[0] = char.ToUpper(newNick[0]);
        _nameMenu.text = newNick.ToString();
        return CheckForbiddenWords(newNick.ToString());
    }

    public string CheckForbiddenWords(string username)
    {
        textAsset = Resources.Load("ForbiddenWords/ForbiddenWords") as TextAsset;
        forbiddenWords = textAsset.text.Split('\n');
        foreach (string line in forbiddenWords)
        {
            string pattern = @"\S*" + line + @"\S*";
            if (Regex.IsMatch(username, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled))
            {
                Debug.LogWarning("Ник " + username + " запрещен!");
                _inpWindowForb.SetActive(true);
                return NameGenerator();
            }
        }
        textAsset = Resources.Load("ForbiddenWords/ForbiddenWordsArab") as TextAsset;
        forbiddenWords = textAsset.text.Split('\n');
        foreach (string line in forbiddenWords)
        {
            if (username.Contains(line))
            {
                Debug.LogWarning("Ник " + username + " запрещен!");
                _inpWindowForb.SetActive(true);
                return NameGenerator();
            }
        }
        return username;
    }

    private void OnApplicationPause(bool pause)
    {
        SaveUserDiamonds(PlayerData.Diamonds);
    }

    private void OnApplicationQuit()
    {
        SaveUserDiamonds(PlayerData.Diamonds);
    }

    public void SaveUserDiamonds(int diamonds)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Diamonds", diamonds.ToString() }
            }
        },
        result => {
            PlayerPrefs.SetInt("CurrUserDiamonds", diamonds);
        },
        error => {
        });
    }

    void GetUserData(string myPlayFabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserData()
        {
            PlayFabId = myPlayFabId,
            Keys = null
        }, result => {
            if (result.Data != null)
            {
                if (result.Data.ContainsKey("Diamonds"))
                {
                    int diamondsOnServer = int.Parse(result.Data["Diamonds"].Value);
                    if (!PlayerPrefs.HasKey("CurrUserDiamonds"))
                    {
                        PlayerData.Diamonds = diamondsOnServer;
                        PlayerData.SaveDiamonds();
                    }
                }
            }
            else
            {

            }
        }, (error) => {
        });
    }

    public void ShowInputNameWindow() => StartCoroutine(CorShowInputNameWindow());

    private IEnumerator CorShowInputNameWindow()
    {
        yield return new WaitForSeconds(delay);
        _inputName.SetActive(true);
    }
}
