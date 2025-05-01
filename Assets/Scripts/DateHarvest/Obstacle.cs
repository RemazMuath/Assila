using UnityEngine;

public class Obstacle : MonoBehaviour, IPoolable
{
    private bool active = false;

    public void OnReposition()
    {
        active = true;
    }

    private void OnEnable()
    {
        active = true;
    }

    private void Update()
    {
        // Original pool return logic
        if (Camera.main != null && transform.position.y < Camera.main.transform.position.y - 10f)
        {
            PoolManager.Instance.ReturnToPool("Obstacle", gameObject);
            active = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Triggered by: {collision.name} with tag {collision.tag}", collision.gameObject);

        if (!active || !collision.CompareTag("Player")) return;

        CameraShaker.Instance?.Shake();
        GameManager.Instance?.ApplyTimePenalty();
        collision.GetComponent<PlayerController>()?.ForceFlipFromObstacle();

        PoolManager.Instance.ReturnToPool("Obstacle", gameObject);
        active = false;
    }

}