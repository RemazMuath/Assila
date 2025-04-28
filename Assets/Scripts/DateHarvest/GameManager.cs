using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Palm Segment Settings")]
    public Transform spawnPoint;
    public GameObject palmSegmentPrefab;
    public float segmentSpacing = 8f;
    public int visibleSegmentsCount = 6;

    [Header("Obstacle Settings")]
    public float spawnObstacleInterval = 5f;
    public float xOffsetMin = -3f;
    public float xOffsetMax = 3f;
    public float spawnHeightOffset = 10f;

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public GameObject gameOverUI;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private float nextSpawnY;
    private float nextObstacleSpawnY;
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        nextSpawnY = spawnPoint.position.y;
        nextObstacleSpawnY = player.position.y + spawnObstacleInterval;

        InitializeSegments();
        Time.timeScale = 1f;
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }

        ManageSegments();
        ManageObstacles();
    }

    void InitializeSegments()
    {
        for (int i = 0; i < visibleSegmentsCount; i++)
        {
            SpawnSegment();
        }
    }

    void ManageSegments()
    {
        if (activeSegments.Count == 0) return;

        GameObject bottomSegment = activeSegments.Peek();
        float bottomY = bottomSegment.transform.position.y;

        if (bottomY + segmentSpacing < player.position.y - (segmentSpacing * 2f))
        {
            GameObject segment = activeSegments.Dequeue();

            Vector3 topPosition = new Vector3(0f, nextSpawnY, 0f);
            segment.transform.position = topPosition;
            segment.GetComponent<IPoolable>()?.OnReposition();
            activeSegments.Enqueue(segment);

            nextSpawnY += segmentSpacing;
        }
    }

    void ManageObstacles()
    {
        if (player.position.y >= nextObstacleSpawnY)
        {
            float randomX = Random.Range(xOffsetMin, xOffsetMax);
            Vector3 spawnPos = new Vector3(randomX, player.position.y + spawnHeightOffset + 2f, 0f);

            GameObject obstacle = PoolManager.Instance.SpawnFromPool("Obstacle", spawnPos, Quaternion.identity);
            if (obstacle == null)
            {
                Debug.LogWarning("Obstacle could not be spawned. Check PoolManager.");
            }

            nextObstacleSpawnY += spawnObstacleInterval;
        }
    }

    void SpawnSegment()
    {
        GameObject segment = PoolManager.Instance.SpawnFromPool("PalmSegment", new Vector3(0f, nextSpawnY, 0f), Quaternion.identity);
        if (segment != null)
        {
            activeSegments.Enqueue(segment);
            nextSpawnY += segmentSpacing;
        }
    }

    public void ApplyTimePenalty()
    {
        if (gameOver) return;

        timeLeft -= timePenalty;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }
}
