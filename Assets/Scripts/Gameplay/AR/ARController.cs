using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the placedPrefab is instantiated
    /// and moved to the hit position.
    /// </summary>
    
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARController : MonoBehaviour {
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        [HideInInspector] public List<GameObject> placedPrefabs;

        [SerializeField] private GameObject buttonPrefab; // Prefab for carousel buttons
        [SerializeField] private Transform buttonParent;
        
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        private Dictionary<GameObject, bool> spawnStates;
        private ARPlaneManager arPlane;
        
        private int selectedPrefabIndex = -1;
        private bool isEnabled = true;

        private ARRaycastManager m_RaycastManager;
        private List<GameObject> spawnedObjects;
        public GameObject spawnedObject;
        private int totalButtons;

        private void Awake() {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            spawnStates = new Dictionary<GameObject, bool>();
            spawnedObjects = new List<GameObject>();
            arPlane = GetComponent<ARPlaneManager>();
            totalButtons = LevelManager.Instance.levels.Length;
        
            for (int i = 0; i < totalButtons; i++) {
                int index = i;
                buttonPrefab.GetComponent<ObjectButton>().isLocked = LevelManager.Instance.levels[i].isLocked; //check locked state
                GameObject newObject = Instantiate(buttonPrefab, buttonParent.transform);
                GameObject viewObject = Instantiate(newObject.GetComponent<ObjectButton>().objectPrefabs[i], newObject.transform);
                placedPrefabs.Add(newObject);
                placedPrefabs[i].GetComponent<Button>().onClick.AddListener(() => SelectPrefab(index));
            }

            foreach (var prefab in placedPrefabs) {
                spawnStates[prefab] = false;
            }
        }

        private bool TryGetTouchPosition(out Vector2 touchPosition) {
            if (Input.touchCount > 0) {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        private bool IsTouchOverUI(Vector2 touchPosition) {
            PointerEventData eventData = new PointerEventData(EventSystem.current) {
                position = touchPosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        public void SelectPrefab(int index) {
            if (index >= 0 && index < placedPrefabs.Count && !spawnStates[placedPrefabs[index]]) {
                selectedPrefabIndex = index;
                spawnedObject = null;
                Debug.Log($"Prefab {index} selected.");
            } else {
                Debug.LogWarning("Prefab already placed or invalid index.");    
            }
        }
        private void Update() {
            if (selectedPrefabIndex != -1) {
                if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

                if (IsTouchOverUI(touchPosition))
                    return;

                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon)) {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;
                    GameObject prefabToPlace = placedPrefabs[selectedPrefabIndex];

                    if (spawnedObject == null) {
                        if (!spawnStates[prefabToPlace]) {
                            spawnedObject = Instantiate(prefabToPlace, hitPose.position, hitPose.rotation);
                            spawnedObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            spawnedObjects.Add(spawnedObject);
                            spawnStates[prefabToPlace] = true; // Mark as placed
                            selectedPrefabIndex = -1; // Reset the selected prefab index
                        }   
                    }
                }
            } 
            else {
                if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

                if (IsTouchOverUI(touchPosition))
                    return;

                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon)) {
                    var hitPose = s_Hits[0].pose;

                    if (spawnedObject == null) {
                        return;
                    } else {
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
        }

        public void ClearAllSpawnedObjects() {
            foreach (var obj in spawnedObjects) {
                if (obj != null) {
                    Destroy(obj);
                }
            }

            spawnedObjects.Clear(); // Clear the list of spawned objects

            // Reset spawn states for each prefab
            foreach (var prefab in placedPrefabs) {
                spawnStates[prefab] = false;
            }

            Debug.Log("All spawned objects have been cleared.");
        }

        public void TogglePlaneDetection() {
            if (isEnabled) {
                arPlane.enabled = false;
                isEnabled = false;
            } else {
                arPlane.enabled = true;
                isEnabled = true;
            }
        }

        public void ReturnMenu() {
            Destroy(LevelManager.Instance);
            SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
