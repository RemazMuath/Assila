using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawning Settings")]
    public Transform spawnPoint;              // <-- You'll assign this in the Inspector
    public GameObject palmSegmentPrefab;      // <-- Optional, in case you're not pooling
    public int numberOfSegments = 30;
    public float segmentSpacing = 2f;

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GeneratePalmTree();
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            Debug.Log("Out of time!");
            // Add Game Over logic here if needed
        }
    }

    public void ApplyTimePenalty()
    {
        timeLeft -= timePenalty;
        Debug.Log("Hit obstacle! Time left: " + timeLeft.ToString("F1"));
    }

    void GeneratePalmTree()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            Vector3 pos = spawnPoint.position + new Vector3(0f, i * segmentSpacing, 0f);
            GameObject segment = PoolManager.Instance.SpawnFromPool("PalmSegment", pos, Quaternion.identity);

            PalmSegment palm = segment.GetComponent<PalmSegment>();
            if (palm != null)
            {
                palm.hasObstacle = Random.value < 0.4f; // 40% chance of obstacle
            }
        }
    }
}
