using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isMovingRight = false;

    [Header("Movement Settings")]
    public float upwardMoveSpeed = 2f;
    public float leftXPosition = -1.42f; // Exact left position (original value)
    public float rightXPosition = 0.9f;  // Exact right position (original value)

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Auto upward movement
        transform.Translate(Vector2.up * upwardMoveSpeed * Time.deltaTime);

        // Keyboard input
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
    }

    public void MoveLeft()
    {
        if (isMovingRight)
        {
            isMovingRight = false;
            spriteRenderer.flipX = false;
            SnapToPosition(leftXPosition); // Force exact left position
        }
    }

    public void MoveRight()
    {
        if (!isMovingRight)
        {
            isMovingRight = true;
            spriteRenderer.flipX = true;
            SnapToPosition(rightXPosition); // Force exact right position
        }
    }

    private void SnapToPosition(float xPos)
    {
        // Instant position update (bypass physics)
        transform.position = new Vector3(xPos, transform.position.y, 0);
    }

    public void ForceFlipFromObstacle()
    {
        // Debug to verify it's being called
        Debug.Log("Obstacle triggered flip!");

        if (isMovingRight)
        {
            MoveLeft(); // Uses the same position snapping
        }
        else
        {
            MoveRight(); // Uses the same position snapping
        }
    }
}