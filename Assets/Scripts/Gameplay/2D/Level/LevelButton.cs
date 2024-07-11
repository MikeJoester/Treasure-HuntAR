using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [HideInInspector] public bool IsLocked;
    [HideInInspector] public bool HasCleared;
    [HideInInspector] public Level levelData;

    [SerializeField] private TextMeshProUGUI levelNumber;
    private Button levelButton;
    private void Awake() {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(OnLevelButtonClick);
    }

    private void Update() {
        levelButton.interactable = !IsLocked;
        levelButton.GetComponent<Image>().color = HasCleared ? new Color32(85, 85, 85, 255) : new Color32(255, 255, 255, 255);
    }

    private void OnLevelButtonClick() {
        LevelManager.Instance.selectedLevel = levelData;
        SceneManager.LoadScene("Gameplay");
    }
}
