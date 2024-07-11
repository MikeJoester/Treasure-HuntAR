using System;
using UnityEngine;
using UnityEngine.UI;

public class Brick : MonoBehaviour
{
    public int brickId;
    [SerializeField] private Sprite[] states = new Sprite[6];
    [SerializeField] private AudioClip brickClip;

    private Image brickSprite;
    private BoxCollider2D brickCollider;
    private RectTransform brickTransform;
    private int[] health = new int[] {-1, 1, 2, 3, 4};
    private int brickPoint = 100;
    private int brickHealth;

    private void Awake() {
        brickSprite = GetComponent<Image>();
        brickCollider = GetComponent<BoxCollider2D>();
        brickTransform = GetComponent<RectTransform>();
        InitBrick(brickId);
    }

    private void Update() {
        brickCollider.size = brickTransform.rect.size;
    }

    public void InitBrick(int brickIndex) {
        gameObject.SetActive(true);
        if (brickIndex == -1) {
            Color newColor = brickSprite.color;
            newColor.a = 0f;
            brickSprite.color = newColor;
            brickCollider.enabled = false;
        } else {
            brickSprite.sprite = states[brickIndex];
            brickHealth = health[brickIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (brickHealth > -1) {
            if (collision.gameObject.tag == "Ball") {
                SoundManager.Instance.PlayClipOnce(brickClip);
                brickHealth--;
                GameManager.Instance.AddPoint(brickPoint);

                if (brickHealth == 0) {
                    Color newColor = brickSprite.color;
                    newColor.a = 0f;
                    brickSprite.color = newColor;
                    brickCollider.enabled = false;
                    GameManager.Instance.totalBricks--;
                }
                else {
                    brickSprite.sprite = states[brickHealth];
                }
            }
        }
    }
}
