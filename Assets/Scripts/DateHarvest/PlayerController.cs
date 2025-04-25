using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float climbSpeed = 3f;
    public float horizontalSpeed = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * horizontalSpeed, climbSpeed);
    }
}
