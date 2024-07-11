using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int totalBricks;

    [Header("Game UI")]
    [SerializeField] private Image gameBackground;
    [SerializeField] private GameObject brickRows;
    [SerializeField] private VerticalLayoutGroup brickCols;
    [SerializeField] private GameObject brickPiece;
    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private Ball ball;
    [SerializeField] private Paddle paddle;
    [SerializeField] Image[] hitPoints;

    [Header("Pause Menu")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject pauseMenuUI;
    
    [Header("Game Over UI")]
    [SerializeField] private Button restartGame;
    [SerializeField] private Button returnMenu;
    [SerializeField] private GameObject gameOverUI;

    [Header("Game Win UI")]
    [SerializeField] private Button objectViewButton;
    [SerializeField] private Button returnToMenu;
    [SerializeField] private GameObject gameWinUI;

    [Header ("Game SFX")]
    [SerializeField] private AudioClip lifeLost;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip gameWinClip;

    private bool isPaused = false;
    private int totalHealth;
    private int totalPoints = 0;
    private Coroutine respawnCoroutine;
    private Level currentLevel;
    private PlayfabManager playFabManager;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        currentLevel = LevelManager.Instance.selectedLevel;
        playFabManager = FindObjectOfType<PlayfabManager>();
        gameBackground.sprite = currentLevel.levelBG;
        // Debug.Log(currentLevel.levelNum);
        SpawnBricks();
    }

    private void Start() {
        totalHealth = hitPoints.Length;
        
        AssignButtons();
    }

    private void AssignButtons() {
        //Pause Menu
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(TogglePause);
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(ReturnMenu);

        //GameOver Menu
        restartGame.onClick.AddListener(RestartGame);
        returnMenu.onClick.AddListener(ReturnMenu);

        //GameWin Menu
        objectViewButton.onClick.AddListener(ObjectViewScene);
        returnToMenu.onClick.AddListener(ReturnMenu);
    }

    private void TogglePause() {
        if (!isPaused) {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
        else {
            pauseMenuUI.SetActive(false); 
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    private void ToggleGameOver(GameObject gameOverModal) {
        if (!isPaused) {
            gameOverModal.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
        else {
            gameOverModal.SetActive(false); 
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    private void SpawnBricks() {
        for (int rowIndex = 0; rowIndex < currentLevel.BrickRows.Count; rowIndex++) {
            GameObject newBrick = Instantiate(brickRows, brickCols.transform); //Spawn Rows
            
            BrickCols _brickCol = currentLevel.BrickRows[rowIndex];

            //Spawn each of the brick with specific Ids in the rows
            for (int colIndex = 0; colIndex < _brickCol.brickColSize.Count; colIndex++) {
                if (_brickCol.brickColSize[colIndex] != 0) {
                    totalBricks++;
                }
                brickPiece.GetComponent<Brick>().brickId = _brickCol.brickColSize[colIndex];
                Instantiate(brickPiece, newBrick.GetComponent<HorizontalLayoutGroup>().transform); 
            }   
        }
    }

    private void Update() {
        totalScore.text = totalPoints.ToString();
        if (totalHealth == 1) {
            hitPoints[0].color = new Color(255, 0, 0, 255);
        } else hitPoints[0].color = new Color(0, 255, 0, 255);

        if (totalBricks == 0) {
            currentLevel.hasCleared = true;
            SoundManager.Instance.PlayClipOnce(gameWinClip);
            gameWinUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void UpdatePlayerData() {
        int levelCleared = 0;
        int levelUnlocked = 0;
        foreach (var level in LevelManager.Instance.levels) {
            if (level.hasCleared) {
                levelCleared ++;
            }
            if (!level.isLocked) {
                levelUnlocked ++;
            }
        }
        playFabManager.SavePlayerData(totalPoints, levelUnlocked, levelCleared);
        PlayfabManager.Instance.SendLeaderBoard(totalPoints);
    }

    private void ObjectViewScene() {
        UpdatePlayerData();
        SceneManager.LoadScene("ObjectView");
        Destroy(gameObject);
    }

    private void Respawn() {
        if (respawnCoroutine != null) {
            StopCoroutine(respawnCoroutine);
        }
        respawnCoroutine = StartCoroutine(RespawnCoroutine());
    }

    private void RestartGame() {
        StartCoroutine(RestartCoroutine());
    }
    private void ReturnMenu() {
        Time.timeScale = 1f;
        UpdatePlayerData();
        SceneManager.LoadScene("MainMenu");
        Destroy(LevelManager.Instance);
        Destroy(gameObject);
    }

    private void ChangeOpacity(Image sprite, float opacity) {
        Color newColor = sprite.color;
        newColor.a = opacity;
        sprite.color = newColor;
    }

    public void HpLost() {
        totalHealth--;
        ChangeOpacity(hitPoints[totalHealth], 0f);

        if (totalHealth == 0) {
            SoundManager.Instance.PlayClipOnce(gameOverClip);
            ToggleGameOver(gameOverUI);
        } else {
            SoundManager.Instance.PlayClipOnce(lifeLost);
            Respawn(); 
        }       
    }

    public void AddPoint(int point) {
        totalPoints += point;
    }

    private IEnumerator RespawnCoroutine() {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);

        if (ball != null) {
            ball.ResetBall();
        }

        if (paddle != null) {
            paddle.ResetPaddle();
        }

        Time.timeScale = 1f;
    }

    private IEnumerator RestartCoroutine() {
        Time.timeScale = 0f;

        for(int i = 0; i < hitPoints.Length; i++) {
            ChangeOpacity(hitPoints[i], 255f);
        }
        totalHealth = hitPoints.Length;
        totalPoints = 0;
        isPaused = false;
        gameOverUI.SetActive(false); 
        pauseMenuUI.SetActive(false);
        Respawn();
        foreach(Transform child in brickCols.transform) {
            Destroy(child.gameObject);
        }
        SpawnBricks();
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
    }
}
