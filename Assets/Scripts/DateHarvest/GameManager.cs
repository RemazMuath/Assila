// Merged version of GameManager, CameraFollow, CameraShaker, Difficulty Logic, and Win Conditions
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyLevel { Easy, Medium, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static DifficultyLevel SelectedDifficulty = DifficultyLevel.Medium;

    [Header("Palm Segment Settings")]
    public Transform spawnPoint;
    public GameObject palmSegmentPrefab;
    public float segmentSpacing = 8f;
    public int visibleSegmentsCount = 6;
    public int maxPalmSegments = 30; // NEW: limit to palm segment generation

    [Header("Obstacle Settings")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3f;
    public float spawnHeightOffset = 10f;
    public int maxActiveObstacles = 5;
    public float[] fixedXPositions = new float[] { -3f, 0f, 3f };
    public float obstacleStopBuffer = 5f; // NEW: how far below the top obstacles should stop

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public GameObject gameOverUI;
    public GameObject winUI; // NEW: win panel
    public GameObject topTreeSegment; // reference to static top segment
    public int score = 0;

    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float cameraSmoothSpeed = 5f;
    public float cameraMinY = 0f;
    public float cameraShakeDuration = 0.2f;
    public float cameraShakeMagnitude = 0.2f;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnY;
    private float nextObstacleSpawnY;
    private bool gameOver = false;
    private bool playerWon = false;
    private Vector3 originalCameraPos;
    private bool shaking = false;
    private int totalSegmentsSpawned = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        originalCameraPos = Camera.main.transform.localPosition;
    }

    void Start()
    {
        nextSpawnY = spawnPoint.position.y;
        nextObstacleSpawnY = player.position.y + minSpawnInterval;
        InitializeSegments();
        Time.timeScale = 1f;
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);

        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                switch (SelectedDifficulty)
                {
                    case DifficultyLevel.Easy:
                        controller.upwardMoveSpeed = 2f;
                        break;
                    case DifficultyLevel.Medium:
                        controller.upwardMoveSpeed = 3.5f;
                        break;
                    case DifficultyLevel.Hard:
                        controller.upwardMoveSpeed = 5f;
                        break;
                }
            }
        }

        if (cameraTarget == null && player != null)
        {
            cameraTarget = player;
        }
    }

    void Update()
    {
        if (gameOver || playerWon) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }

        ManageSegments();
        ManageObstacles();
        UpdateCamera();

        if (topTreeSegment != null && player.position.y >= topTreeSegment.transform.position.y)
        {
            Win();
        }
    }

    void ManageObstacles()
    {
        activeObstacles.RemoveAll(obstacle => obstacle == null || !obstacle.activeSelf);

        float newY = player.position.y + spawnHeightOffset;
        float minVerticalDistance = 2.5f;

        foreach (GameObject obs in activeObstacles)
        {
            if (obs == null) continue;
            float existingY = obs.transform.position.y;
            if (Mathf.Abs(existingY - newY) < minVerticalDistance) return;
        }

        // Prevent obstacle spawn if we're too close to the top
        if (topTreeSegment != null && newY >= topTreeSegment.transform.position.y - obstacleStopBuffer)
        {
            return;
        }

        if (player.position.y >= nextObstacleSpawnY && activeObstacles.Count < maxActiveObstacles)
        {
            float fixedX = fixedXPositions[Random.Range(0, fixedXPositions.Length)];
            Vector3 spawnPos = new Vector3(fixedX, newY, 0f);
            GameObject obstacle = PoolManager.Instance.SpawnFromPool("Obstacle", spawnPos, Quaternion.identity);
            if (obstacle != null)
            {
                activeObstacles.Add(obstacle);
            }
            nextObstacleSpawnY += Random.Range(minSpawnInterval, maxSpawnInterval);
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
        if (activeSegments.Count == 0 || totalSegmentsSpawned >= maxPalmSegments) return;

        GameObject bottomSegment = activeSegments.Peek();
        float bottomY = bottomSegment.transform.position.y;

        if (bottomY + segmentSpacing < player.position.y - (segmentSpacing * 2f))
        {
            GameObject segment = activeSegments.Dequeue();
            segment.transform.position = new Vector3(0f, nextSpawnY, 0f);
            segment.GetComponent<IPoolable>()?.OnReposition();
            activeSegments.Enqueue(segment);
            nextSpawnY += segmentSpacing;
            totalSegmentsSpawned++;
        }
    }

    void SpawnSegment()
    {
        if (totalSegmentsSpawned >= maxPalmSegments) return;

        GameObject segment = PoolManager.Instance.SpawnFromPool("PalmSegment",
            new Vector3(0f, nextSpawnY, 0f), Quaternion.identity);
        if (segment != null)
        {
            activeSegments.Enqueue(segment);
            nextSpawnY += segmentSpacing;
            totalSegmentsSpawned++;
        }
    }

    public void ApplyTimePenalty()
    {
        if (gameOver || playerWon) return;

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

    void Win()
    {
        playerWon = true;
        Time.timeScale = 0f;
        if (winUI != null) winUI.SetActive(true);
        Debug.Log("Player has reached the top! WIN!");
    }

    void UpdateCamera()
    {
        if (cameraTarget == null) return;

        float targetY = Mathf.Max(cameraMinY, cameraTarget.position.y);
        Vector3 targetPos = new Vector3(0f, targetY, -10f);
        Vector3 smoothPos = Vector3.Lerp(Camera.main.transform.position, targetPos, cameraSmoothSpeed * Time.deltaTime);
        Camera.main.transform.position = smoothPos;
    }

    public void ShakeCamera()
    {
        if (!shaking) StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        shaking = true;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            float y = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            Camera.main.transform.localPosition = originalCameraPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalCameraPos;
        shaking = false;
    }

    public void AddScore(int value)
    {
        score += value;
    }
}
