using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Drag your Player GameObject here in the Inspector
    public float smoothSpeed = 5f;    // Controls how smooth the camera follows
    public float minY = 0f;           // Optional: prevents the camera from going below the palm base

    void LateUpdate()
    {
        if (target == null) return;

        // Clamp Y to prevent going too low (e.g., below palm tree base)
        float targetY = Mathf.Max(minY, target.position.y);

        // Lock X and Z, only follow Y
        Vector3 targetPos = new Vector3(0f, targetY, -10f);
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothPos;
    }
}