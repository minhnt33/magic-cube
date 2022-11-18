using UnityEngine;
using System.Collections;
using MagicCubeManager;

public class GPManager : MonoBehaviour
{
    private static GPManager _instance;

    public int _showInterstisialAdPeriod = 4;
    public int _showBannerAdPeriod = 1;

    private const string _inAd = "ca-app-pub-6022706640212369/9895682930";
    private const string _bannerAd = "ca-app-pub-6022706640212369/8418949738";

    private PrefManager _prefManager;
    private AndroidAdMobController _admobController;

    private GoogleMobileAdBanner _banner;

    public static GPManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void OnEnable()
    {
        GameStateManager.OnGameStateChange += OnGameStateChange;
        SceneManager.OnSceneChange += OnSceneChange;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChange -= OnGameStateChange;
        SceneManager.OnSceneChange -= OnSceneChange;
    }

    private void OnGameStateChange(GAME_STATE newState)
    {
        if (newState == GAME_STATE.END)
        {
            if (_prefManager.GameplayCount % _showInterstisialAdPeriod == 0 && !_prefManager.IsFirstPlayingGame)
            {
                ShowInterstisialAd();
            }
        }
    }

    private void OnSceneChange(SceneState newScene)
    {
        if (newScene == SceneState.LEVEL_SCENE && _prefManager.GameplayInCurrentSession > _showBannerAdPeriod)
        {
            ShowBannerAd();
        }
        else if (newScene == SceneState.GAME_SCENE)
        {
            HideBanner();
        }
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //SignIn();
    }

    void Start()
    {
        _prefManager = PrefManager.Instance;
        _admobController = AndroidAdMobController.Instance;

        _admobController.Init(_inAd);
        _admobController.SetInterstisialsUnitID(_inAd);
        _admobController.SetBannersUnitID(_bannerAd);
        _admobController.LoadInterstitialAd();
    }

    public void ShowInterstisialAd()
    {
        _admobController.ShowInterstitialAd();
        _admobController.LoadInterstitialAd();
    }

    public void ShowBannerAd()
    {
        _banner = _admobController.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.SMART_BANNER);
        _banner.Show();
    }

    public void HideBanner()
    {
        if (_banner != null)
            _banner.Hide();
    }

    private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
    {
        GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;

        if (result.IsSuccess)
        {

        }
        else
        {

        }
    }

    private void OnLeaderBoardsLoaded(GooglePlayResult result)
    {
        GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;

        if (result.IsSucceeded)
        {

        }
        else
        {
            AndroidMessage.Create("Leader-Boards Loaded error: ", result.Message);
        }
    }

    public void SignIn()
    {
        GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
        GooglePlayConnection.Instance.Connect();
    }

    public void Disconnect()
    {
        GooglePlusAPI.Instance.ClearDefaultAccount();
    }

    private void LoadingLeaderboard()
    {
        GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
        GooglePlayManager.Instance.LoadLeaderBoards();
    }

    public void SubmitScore(int score, string leaderboardKey)
    {
        ShowLeaderboard(leaderboardKey);
        GooglePlayManager.Instance.SubmitScoreById(leaderboardKey, score);
    }

    public void ShowLeaderboard(string leaderboardKey)
    {
        if (CheckConnection())
        {
            LoadingLeaderboard();
            GooglePlayManager.Instance.ShowLeaderBoardById(leaderboardKey);
        }
        else
        {
            SignIn();
            LoadingLeaderboard();
            GooglePlayManager.Instance.ShowLeaderBoardById(leaderboardKey);
        }
    }

    private bool CheckConnection()
    {
        if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
            return true;

        return false;
    }
}
