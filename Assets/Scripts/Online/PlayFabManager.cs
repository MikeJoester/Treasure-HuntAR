using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;

using LoginResult = PlayFab.ClientModels.LoginResult;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance { get; private set; }
    public bool isLoggedIn = false;
    private PlayFabUIManager playFabUI;
    private static bool isInitialized = false;
    private string LeaderboardName = "GameScore";
    private string PlName = null;
    private string _message;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!isInitialized) {
                FB.Init(OnFacebookInitialized);
                isInitialized = true;
            }
        }
        else {
            Destroy(gameObject);
        }

        playFabUI = FindObjectOfType<PlayFabUIManager>();
    }

    private void Start() {
        // logs the given message and displays it on the screen using OnGUI method
        SetMessage("Initializing Facebook...");
        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished 
    //    DemoLogin();
    }

    private void DemoLogin() {
        PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { 
            CreateAccount = true, 
            AccessToken = "GGQVliQkZAySmxRbkxSU3d1UkFHd3JycEdmOGxSakZAEUXB1cnVxSUh6bkZAfdThSSEwzTW5uRmVCS2NvQmljeEVmUFg3MUFtcUJPa3BrMmUxMGw3RkpGMWZA3YWZASNkJ1eU9CN0JCTl9sQ1M4YUdqR3FKZA1dINHIwaVl5ZAC1fZA2VibkZAwUQZDZD",
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        }, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
    }

    public void GalleryView() {
        SceneManager.LoadScene("ObjectView");
    }

    public void LoginFB() {
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }

    public void LogOutButton() {
        isLoggedIn = false;
        FB.LogOut();
        PlayerPrefs.DeleteAll();
        Destroy(gameObject);
        Destroy(LevelManager.Instance);
        SceneManager.LoadScene("MainMenu");
    }

    private void OnFacebookInitialized() {
        SetMessage("Logging into Facebook...");
        if (FB.IsLoggedIn)
            FB.LogOut();
    }

    private void OnFacebookLoggedIn(ILoginResult result) {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (result == null || string.IsNullOrEmpty(result.Error)) {
            SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { 
                CreateAccount = true, 
                AccessToken = AccessToken.CurrentAccessToken.TokenString,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                    GetPlayerProfile = true
                }
            }, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else {
            // If Facebook authentication failed, we stop the cycle with the message
            SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
        }
    }

    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LoginResult result) {
        CheckUsername(result.InfoResultPayload.PlayerProfile.DisplayName);
        SaveLoginData(result.SessionTicket, result.PlayFabId);
        playFabUI.ChangeDisplayText("Login with Facebook success!");
        SetMessage("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error) {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }

    public void SetMessage(string message, bool error = false) {
        _message = message;
        if (error) {
            Debug.LogError(_message);
        } else {
            Debug.Log(_message);
        }  
    }

    public void RegisterButton() {
        if (playFabUI.registerPasswordInput.text.Length < 7) {
            playFabUI.ChangeDisplayText("Password too short! Must be 8 characters or above.");
            return;
        }
        var request = new RegisterPlayFabUserRequest {
            Email = playFabUI.registerEmailInput.text,
            Password = playFabUI.registerPasswordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        GameObject.Find("RegisterModal").SetActive(false);
        playFabUI.NameWindow.SetActive(true);
        playFabUI.ChangeDisplayText("Registered and logged in!");
    }

    public void LoginButton() {
        var request = new LoginWithEmailAddressRequest {
            Email = playFabUI.emailInput.text,
            Password = playFabUI.passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }
    private void OnLoginSuccess(LoginResult result) {
        playFabUI.ChangeDisplayText("Login successful!");
        CheckUsername(result.InfoResultPayload.PlayerProfile.DisplayName);
        SaveLoginData(result.SessionTicket, result.PlayFabId);
        Debug.Log("Successful login!");
    }

    public void ResetPasswordButton() {
        var request = new SendAccountRecoveryEmailRequest {
            Email = playFabUI.emailInput.text,
            TitleId = "70D4C"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, ResetPasswordError);
    }

    private void OnPasswordReset(SendAccountRecoveryEmailResult result) {
        playFabUI.ChangeDisplayText("Password reset mail sent!");
    }

    private void ResetPasswordError(PlayFabError error) {
        playFabUI.ChangeDisplayText("Email not found!");
    }

    private void CheckUsername(string userName) {
        if ( userName != null) {
            PlName = userName;
        }

        if (PlName == null) {
            playFabUI.NameWindow.SetActive(true);
            Debug.Log("Create name menu");
        } else {
            Debug.Log("Logged in!");
            playFabUI.LoginMenu.SetActive(false);
            playFabUI.MenuUI.SetActive(true);
            PlayerPrefs.SetString("PlayerName", PlName);
            playFabUI.LoginMenu.SetActive(false);
            playFabUI.MenuUI.SetActive(true);
            playFabUI.displayName.text = "WELCOME " + userName + "!";
            GetPlayerData();
        }
    }

    private void SaveLoginData(string sessionTicket, string playFabId) {
        string encryptedSessionTicket = EncryptData(sessionTicket);
        string encryptedPlayFabId = EncryptData(playFabId);

        PlayerPrefs.SetString("SessionTicket", encryptedSessionTicket);
        PlayerPrefs.SetString("PlayFabId", encryptedPlayFabId);
        PlayerPrefs.Save();

        Debug.Log("Login data saved securely.");
    }

    private string EncryptData(string data) {
        byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(data);
        string encryptedData = System.Convert.ToBase64String(dataToEncrypt);
        return encryptedData;
    }

    private string DecryptData(string encryptedData) {
        byte[] dataToDecrypt = System.Convert.FromBase64String(encryptedData);
        string decryptedData = System.Text.Encoding.UTF8.GetString(dataToDecrypt);
        return decryptedData;
    }

    private string RetrieveEncryptedData(string key) {
        if (PlayerPrefs.HasKey(key)) {
            string encryptedData = PlayerPrefs.GetString(key);
            return DecryptData(encryptedData);
        }
        return null;
    }

    public void SubmitNameButton() {
        var request = new UpdateUserTitleDisplayNameRequest {
            DisplayName = playFabUI.nameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result) {
        playFabUI.ChangeDisplayText("Create Name Success!");
        isLoggedIn = true;
        GameObject.Find("NameModal").SetActive(false);
        SavePlayerData(0, 3, 0);
        PlayerPrefs.SetString("PlayerName", PlName);
    }

    static void OnError(PlayFabError error) {
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderBoard(int score) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = LeaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result) {
        Debug.Log("Successful leaderboard sent!");
    }

    public void GetLeaderboard() {
        var request = new GetLeaderboardRequest {
            StatisticName = LeaderboardName,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    private void OnLeaderboardGet(GetLeaderboardResult result) {
        foreach (var item in result.Leaderboard) {
           playFabUI.DisplayLeaderboardData(string.Format("{0}  |  {1}  |  {2}", item.Position + 1, item.DisplayName, item.StatValue));
        }
    }

    public void GetPlayerData() {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    private void OnDataReceived(GetUserDataResult result) {
        Debug.Log("Received User Data!");
        if (result.Data != null) {
            //Set Level, Current State of players
            LevelManager.Instance.unlockedLevels = Int32.Parse(result.Data["UnlockedLevels"].Value);
            LevelManager.Instance.clearedLevels = Int32.Parse(result.Data["ClearedLevels"].Value);
            isLoggedIn = true;
            LevelManager.Instance.SpawnLevels();
        }
    }

    public void SavePlayerData(int score, int unlockedLevels, int clearedLevels) {
        var request = new UpdateUserDataRequest{
            Data = new Dictionary<string, string> {
                {"Score", score.ToString()},
                {"UnlockedLevels", unlockedLevels.ToString()},
                {"ClearedLevels", clearedLevels.ToString()},
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult result) {
        Debug.Log("User Data saved to server!");
    }
}