using UnityEngine;

public class PalmSegment : MonoBehaviour, IPoolable
{
    public bool hasObstacle = false;
    public GameObject obstaclePrefab;
    private GameObject currentObstacle;

    public void OnReposition()
    {
        hasObstacle = Random.value < 0.4f;

        if (currentObstacle != null)
            Destroy(currentObstacle);

        if (hasObstacle && obstaclePrefab != null)
        {
            float xOffset = Random.Range(-3f, 3f);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0.5f, 0);
            currentObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
        }
    }
}