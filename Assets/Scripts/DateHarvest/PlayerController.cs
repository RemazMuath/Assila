using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isMovingRight = false;
    private float currentYPosition;

    [Header("Movement Settings")]
    public bool allowControl = false; 
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

        rb.isKinematic = true;
    }

    void Update()
    {
        if (!allowControl) return;

        // Maintain a constant climb speed — not accelerating
        currentYPosition += upwardMoveSpeed * Time.deltaTime;

        transform.position = new Vector3(
            transform.position.x,
            currentYPosition,
            transform.position.z
        );

        // Handle lane switching
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
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D not assigned!");
            return;
        }

        transform.position = new Vector3(
            xPos,
            currentYPosition,
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
