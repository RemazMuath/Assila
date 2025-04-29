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
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3f;
    public float spawnHeightOffset = 10f;
    public int maxActiveObstacles = 5;
    public float[] fixedXPositions = new float[] { -3f, 0f, 3f };  // NEW: fixed X positions

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public GameObject gameOverUI;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnY;
    private float nextObstacleSpawnY;
    private bool gameOver = false;

    void Awake()
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
        nextObstacleSpawnY = player.position.y + minSpawnInterval;
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

    void ManageObstacles()
    {
        activeObstacles.RemoveAll(obstacle => obstacle == null || !obstacle.activeSelf);

        if (player.position.y >= nextObstacleSpawnY && activeObstacles.Count < maxActiveObstacles)
        {
            SpawnObstacle();
            nextObstacleSpawnY += Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnObstacle()
    {
        float fixedX = fixedXPositions[Random.Range(0, fixedXPositions.Length)];  // NEW: fixed X position logic
        Vector3 spawnPos = new Vector3(fixedX, player.position.y + spawnHeightOffset, 0f);

        GameObject obstacle = PoolManager.Instance.SpawnFromPool("Obstacle", spawnPos, Quaternion.identity);
        if (obstacle != null)
        {
            activeObstacles.Add(obstacle);
        }
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
            segment.transform.position = new Vector3(0f, nextSpawnY, 0f);
            segment.GetComponent<IPoolable>()?.OnReposition();
            activeSegments.Enqueue(segment);
            nextSpawnY += segmentSpacing;
        }
    }

    void SpawnSegment()
    {
        GameObject segment = PoolManager.Instance.SpawnFromPool("PalmSegment",
            new Vector3(0f, nextSpawnY, 0f), Quaternion.identity);
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
