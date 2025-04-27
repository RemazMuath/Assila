using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float climbSpeed = 2f;           // Upward climb speed
    public float leftXPosition = -1.42f;    // X position when on left
    public float rightXPosition = 0.9f;     // X position when on right

    private bool isOnLeft = true;           // Start on the left side

    void Start()
    {
        // Start on left
        SnapToSide(isOnLeft);
    }

    void Update()
    {
        // Move upward automatically
        transform.position += Vector3.up * climbSpeed * Time.deltaTime;

        // Handle input for switching sides
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isOnLeft = true;
            SnapToSide(isOnLeft);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isOnLeft = false;
            SnapToSide(isOnLeft);
        }
    }

    void SnapToSide(bool left)
    {
        // Set position X
        float targetX = left ? leftXPosition : rightXPosition;
        transform.position = new Vector3(targetX, transform.position.y, 0f);

        // Flip sprite by scaling X
        Vector3 scale = transform.localScale;
        scale.x = left ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
