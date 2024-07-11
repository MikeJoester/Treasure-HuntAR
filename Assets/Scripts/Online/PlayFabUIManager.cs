using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabUIManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI alertText;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TextMeshProUGUI displayName;

    [Header("Windows")]
    public GameObject NameWindow;
    public GameObject LoginMenu;
    public GameObject MenuUI;
    public GameObject LeaderboardWindow;

    [Header("NameWindow")]
    public TMP_InputField nameInput;

    [Header("Leaderboard")]
    public GameObject listingPrefab;
    public GameObject listingContainer;

    [Header("Buttons")]
    public Button loginButton;
    public Button loginFacebookButton;
    public Button registerButton;
    public Button nameSubmitButton;
    public Button logOutButton;
    public Button objectGalleryButton;
    public Button getLeaderboardButton;

    private void Start() {

        if (PlayfabManager.Instance.isLoggedIn) {
            LoginMenu.SetActive(false);
            MenuUI.SetActive(true);
            displayName.text = "WELCOME " + PlayerPrefs.GetString("PlayerName") + "!";
        }

        loginButton.onClick.AddListener(PlayfabManager.Instance.LoginButton);
        loginFacebookButton.onClick.AddListener(PlayfabManager.Instance.LoginFB);
        registerButton.onClick.AddListener(PlayfabManager.Instance.RegisterButton);
        nameSubmitButton.onClick.AddListener(PlayfabManager.Instance.SubmitNameButton);
        logOutButton.onClick.AddListener(PlayfabManager.Instance.LogOutButton);
        objectGalleryButton.onClick.AddListener(PlayfabManager.Instance.GalleryView);
        getLeaderboardButton.onClick.AddListener(DisplayLeaderboard);
    }

    public void DisplayLeaderboard() {
        LeaderboardWindow.SetActive(true);
        PlayfabManager.Instance.GetLeaderboard();
    }

    public void DisplayLeaderboardData(string data) {
        listingContainer = GameObject.FindGameObjectWithTag("LeaderboardView");
        listingPrefab.GetComponent<TextMeshProUGUI>().text = data;
        if (listingContainer != null)
            Instantiate(listingPrefab, listingContainer.transform);
        else Debug.Log("Missing container");
    }

    public void ClearLeaderboard() {
        foreach(Transform child in listingContainer.transform) {
            Destroy(child.gameObject);
        }
    }

    public void ChangeDisplayText(string text) {
        alertText.text = text;
    }
}
