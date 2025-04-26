using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawning Settings")]
    public Transform spawnPoint;
    public GameObject palmSegmentPrefab;
    public int poolSegmentCount = 10;
    public float segmentSpacing = 2f;

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public int visibleSegmentsCount = 4;

    [Header("Game Over")]
    public GameObject gameOverUI; // Assign a UI panel in inspector

    private List<GameObject> activeSegments = new List<GameObject>();
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize the pool first
        PoolManager.Instance.InitializePool("PalmSegment", palmSegmentPrefab, poolSegmentCount);
        InitializeSegments();

        // Ensure game is running at start
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

        CheckReposition();
    }

    public void ApplyTimePenalty()
    {
        if (gameOver) return;

        timeLeft -= timePenalty;
        Debug.Log("Hit obstacle! Time left: " + timeLeft.ToString("F1"));

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }
    }

    void InitializeSegments()
    {
        for (int i = 0; i < visibleSegmentsCount; i++)
        {
            Vector3 pos = spawnPoint.position + new Vector3(0f, i * segmentSpacing, 0f);
            SpawnSegment(pos);
        }
    }

    void SpawnSegment(Vector3 position)
    {
        GameObject segment = PoolManager.Instance.SpawnFromPool("PalmSegment", position, Quaternion.identity);
        if (segment != null)
        {
            activeSegments.Add(segment);
        }
        else
        {
            Debug.LogWarning("Failed to spawn PalmSegment from Pool!");
        }
    }

    void CheckReposition()
    {
        if (gameOver || activeSegments.Count == 0) return;

        GameObject bottomSegment = activeSegments[0];
        float bottomY = bottomSegment.transform.position.y;

        if (bottomY + segmentSpacing < player.position.y - segmentSpacing * 2f)
        {
            // Return the bottom segment to the pool
            PoolManager.Instance.ReturnToPool("PalmSegment", bottomSegment);
            activeSegments.RemoveAt(0);

            // Spawn a new segment at the top
            Vector3 topPosition = activeSegments[activeSegments.Count - 1].transform.position +
                                  new Vector3(0f, segmentSpacing, 0f);
            SpawnSegment(topPosition);
        }
    }

    void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f; // Freeze the game

        // Show game over UI if assigned
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Debug.Log("Game Over!");
    }
}