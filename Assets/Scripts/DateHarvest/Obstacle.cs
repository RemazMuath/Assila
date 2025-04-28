using UnityEngine;

public class Obstacle : MonoBehaviour, IPoolable
{
    private bool active = false;

    public void OnReposition()
    {
        active = true; // Mark obstacle active when repositioned
    }

    private void OnEnable()
    {
        active = true;
    }

    private void Update()
    {
        if (Camera.main != null && transform.position.y < Camera.main.transform.position.y - 10f)
        {
            PoolManager.Instance.ReturnToPool("Obstacle", gameObject);
            active = false; // Disable collision logic when off screen
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active) return; // Ignore if obstacle was already deactivated

        if (collision.CompareTag("Player"))
        {
            CameraShaker.Instance?.Shake();
            GameManager.Instance?.ApplyTimePenalty();
            PoolManager.Instance.ReturnToPool("Obstacle", gameObject);
            active = false;
        }
    }
}
