using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isMovingRight = false;
    private float currentYPosition; // Track position independently

    [Header("Movement Settings")]
    public float upwardMoveSpeed = 2f;
    public float leftXPosition = -1.42f;
    public float rightXPosition = 0.9f;
    public float leftColliderX = -7.2f;
    public float rightColliderX = 7.2f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentYPosition = transform.position.y;

        // Completely prevent physics interference
        rb.isKinematic = true;
    }

    void Update()
    {
        // Calculate movement independently
        currentYPosition += upwardMoveSpeed * Time.deltaTime;

        // Apply position directly
        transform.position = new Vector3(
            transform.position.x,
            currentYPosition,
            transform.position.z
        );

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
            SnapToPosition(leftXPosition, leftColliderX);
        }
    }

    public void MoveRight()
    {
        if (!isMovingRight)
        {
            isMovingRight = true;
            spriteRenderer.flipX = true;
            SnapToPosition(rightXPosition, rightColliderX);
        }
    }

    private void SnapToPosition(float xPos, float colliderX)
    {
        // Update positions without affecting Y movement
        transform.position = new Vector3(
            xPos,
            currentYPosition, // Maintain consistent Y position
            transform.position.z
        );
        boxCollider.offset = new Vector2(colliderX, boxCollider.offset.y);
    }

    public void ForceFlipFromObstacle()
    {
        Debug.Log("Obstacle triggered flip!");
        if (isMovingRight) MoveLeft();
        else MoveRight();
    }
}