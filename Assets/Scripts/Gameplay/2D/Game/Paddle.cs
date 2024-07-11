using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Paddle : MonoBehaviour
{
    public float moveSpeed;
    public float maxBounceAngle;
    public AudioClip bounceClip;
    private Rigidbody2D paddleBody;

    private void Awake() {
        paddleBody = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        ResetPaddle();
    }

    public void ResetPaddle() {
        paddleBody.velocity = Vector3.zero;
        transform.position = new Vector3(0f, transform.position.y, 0f);
    }

    private void FixedUpdate () {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            float moveX = touch.deltaPosition.x * moveSpeed * Time.deltaTime;

            paddleBody.MovePosition(new Vector2(transform.position.x + moveX, transform.position.y));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Ball")) {
            return;
        }
        
        SoundManager.Instance.PlayClipOnce(bounceClip);
    
        Rigidbody2D ball = collision.rigidbody;
        Collider2D paddle = collision.otherCollider;

        Vector2 ballDirection = ball.velocity.normalized;
        Vector2 contactDistance = paddle.bounds.center - ball.transform.position;

        float bounceAngle = (contactDistance.x / paddle.bounds.size.x) * maxBounceAngle;
        ballDirection = Quaternion.AngleAxis(bounceAngle, Vector3.forward) * ballDirection;

        ball.velocity = ballDirection * ball.velocity.magnitude;
    }
}
