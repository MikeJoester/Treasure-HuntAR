using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour
{
    public List<GameObject> objectPrefabs;
    public bool isLocked;
    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void Update() {
        button.interactable = !isLocked;
    }
}
