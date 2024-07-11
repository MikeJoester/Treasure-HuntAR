using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D ballBody;
    public float speed = 10f;
    
    private Vector3 initialPosition;

    private void Awake() {
        ballBody = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        initialPosition = transform.position;
        ResetBall();
    }

    public void ResetBall() {
        ballBody.velocity = Vector2.zero;
        transform.position = initialPosition;

        CancelInvoke();
        Invoke(nameof(SetRandomTrajectory), 1f);
    }

    private void SetRandomTrajectory() {
        Vector2 force = new Vector2();
        force.x = Random.Range(-1f, 1f);
        force.y = -1f;

        ballBody.AddForce(force.normalized * speed, ForceMode2D.Impulse);
    }

    private void FixedUpdate() {
        ballBody.velocity = ballBody.velocity.normalized * speed;
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bottom")) {
            GameManager.Instance.HpLost();
        }
    }
}
