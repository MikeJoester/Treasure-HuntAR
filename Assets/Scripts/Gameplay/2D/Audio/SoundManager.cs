using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public SceneAudio[] sceneAudios;

    [System.Serializable]
    public class SceneAudio {
        public string sceneName;
        public AudioClip clip;
    }

    private AudioSource audioSource;
    private float savedVolume = 1f;
    private bool isMuted = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PlayClip(AudioClip clip) {
        if (clip != null && !isMuted) {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayClipOnce(AudioClip clip) {
        if (clip != null && !isMuted) {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        PlaySceneAudio(scene.name);
    }

    void PlaySceneAudio(string sceneName) {
        foreach (SceneAudio sceneAudio in sceneAudios) {
            if (sceneAudio.sceneName == sceneName) {
                audioSource.clip = sceneAudio.clip;
                audioSource.Play();
                return;
            }
        }
    }

    public void SetVolume(float volume) {
        savedVolume = volume;
        if (!isMuted) {
            audioSource.volume = volume;
        }
    }

    public void ToggleMute() {
        isMuted = !isMuted;
        audioSource.volume = isMuted ? 0 : savedVolume;
    }
}
