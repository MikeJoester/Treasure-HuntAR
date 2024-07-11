using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }
    public Level[] levels;
    [HideInInspector] public int unlockedLevels;
    [HideInInspector] public int clearedLevels;
    [HideInInspector] public Level selectedLevel;
    [SerializeField] private LevelButton levelButtonPrefab;

    private GridLayoutGroup levelContainer;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }

        if (GameObject.Find("LevelContainer") != null) {
            levelContainer = GameObject.Find("LevelContainer").GetComponent<GridLayoutGroup>();
        }
        
        for (int i = 0; i < levels.Length; i++) {
            levels[i].hasCleared = false;
        }
        for (int i = 3; i < levels.Length; i++) {
            levels[i].isLocked = true;
        }
    }

    private void Start() {
        if (PlayfabManager.Instance.isLoggedIn) { 
            PlayfabManager.Instance.GetPlayerData();    
        }
    }

    public void SpawnLevels() {
        for (int i = 0; i < unlockedLevels; i++) {    
            levels[i].isLocked = false;
        }

        for (int i = 0; i < clearedLevels; i++) {         
            levels[i].hasCleared = true;
        }

        for (int i = 0; i < levels.Length; i++) {
            levelButtonPrefab.IsLocked = levels[i].isLocked;
            levelButtonPrefab.HasCleared = levels[i].hasCleared;
            TMPro.TextMeshProUGUI levelText = levelButtonPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            levelButtonPrefab.levelData = levels[i];
            if (levelText != null) {
                levelText.text = (i + 1).ToString();
            }
            Instantiate(levelButtonPrefab, levelContainer.transform);
        }
    }
}
