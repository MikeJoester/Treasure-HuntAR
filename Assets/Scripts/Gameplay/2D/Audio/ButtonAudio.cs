using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    public AudioClip buttonClip;

    void Start() {
        GetComponent<Button>().onClick.AddListener(PlayAudio);
    }

    void PlayAudio() {
        SoundManager.Instance.PlayClipOnce(buttonClip);
    }
}