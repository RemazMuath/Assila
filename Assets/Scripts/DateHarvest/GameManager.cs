using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawning Settings")]
    public Transform spawnPoint;
    public GameObject palmSegmentPrefab;
    public float segmentSpacing = 2f;
    public int visibleSegmentsCount = 6;

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public GameObject gameOverUI;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private float nextSpawnY;
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
    }

    void InitializeSegments()
    {
        for (int i = 0; i < visibleSegmentsCount; i++)
        {
            SpawnSegment();
        }
    }

    void SpawnSegment()
    {
        GameObject segment = PoolManager.Instance.SpawnFromPool(
            "PalmSegment",
            new Vector3(0f, nextSpawnY, 0f),
            Quaternion.identity
        );

        if (segment != null)
        {
            activeSegments.Enqueue(segment);
            nextSpawnY += segmentSpacing;
        }
    }

    void ManageSegments()
    {
        if (activeSegments.Count == 0) return;

        GameObject bottomSegment = activeSegments.Peek();
        float bottomY = bottomSegment.transform.position.y;

        // Check if bottom segment is far below the player
        if (bottomY + segmentSpacing < player.position.y - (segmentSpacing * 2f))
        {
            GameObject segment = activeSegments.Dequeue();

            // Move the bottom segment to the new top position
            Vector3 topPosition = new Vector3(0f, nextSpawnY, 0f);
            segment.transform.position = topPosition;
            segment.GetComponent<IPoolable>()?.OnReposition();
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
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }
}
