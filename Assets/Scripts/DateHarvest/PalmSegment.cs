using UnityEngine;

public class PalmSegment : MonoBehaviour
{
    public bool hasObstacle = false;
    public GameObject obstaclePrefab;

    void Start()
    {
        // Optionally spawn an obstacle on this segment
        if (hasObstacle && obstaclePrefab != null)
        {
            float xOffset = Random.Range(-3f, 3f); // adjust based on your tree width
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0.5f, 0);
            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
        }
    }
}
