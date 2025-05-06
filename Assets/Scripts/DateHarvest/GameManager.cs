using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
    public int maxPalmSegments = 30;

    [Header("Obstacle Settings")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3f;
    public float spawnHeightOffset = 10f;
    public int maxActiveObstacles = 5;
    public float[] fixedXPositions = new float[] { -3f, 0f, 3f };
    public float obstacleStopBuffer = 5f;

    [Header("Gameplay")]
    public float timeLeft = 60f;
    public float timePenalty = 5f;
    public Transform player;
    public GameObject topTreeSegment;

    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float cameraSmoothSpeed = 5f;
    public float cameraMinY = 0f;
    public float cameraShakeDuration = 0.2f;
    public float cameraShakeMagnitude = 0.2f;

    [Header("UI")]
    public TMP_Text timerText;
    public Image countImage3;
    public Image countImage2;
    public Image countImage1;
    public Image countImage_GO;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnY;
    private float nextObstacleSpawnY;
    private bool gameOver = false;
    private bool playerWon = false;
    private Vector3 originalCameraPos;
    private bool shaking = false;
    private int totalSegmentsSpawned = 0;
    private bool gameStarted = false;

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
        if (SceneManager.GetActiveScene().name == "DifficultySelection") return;

        nextSpawnY = spawnPoint.position.y;
        nextObstacleSpawnY = player.position.y + minSpawnInterval;
        InitializeSegments();
        Time.timeScale = 1f;

        if (cameraTarget == null && player != null)
            cameraTarget = player;

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            PlayerController pc = player.GetComponent<PlayerController>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Static;
        }

        StartCoroutine(ImageCountdown());
    }

    IEnumerator ImageCountdown()
    {

        if (countImage3 != null) countImage3.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (countImage3 != null) countImage3.gameObject.SetActive(false);

        if (countImage2 != null) countImage2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (countImage2 != null) countImage2.gameObject.SetActive(false);

        if (countImage1 != null) countImage1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (countImage1 != null) countImage1.gameObject.SetActive(false);

        if (countImage_GO != null) countImage_GO.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (countImage_GO != null) countImage_GO.gameObject.SetActive(false);

        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.allowControl = true;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
        }



        gameStarted = true;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "DifficultySelection") return;
        Debug.Log($"[GameManager] player: {player}, cameraTarget: {cameraTarget}, spawnPoint: {spawnPoint}");

        if (gameOver || playerWon || !gameStarted) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }

        ManageSegments();
        ManageObstacles();
        UpdateCamera();
        UpdateTimerUI();

        if (topTreeSegment != null && player.position.y >= topTreeSegment.transform.position.y)
        {
            Win();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int time = Mathf.CeilToInt(Mathf.Max(0, timeLeft));
            timerText.text = ConvertToArabicDigits(time.ToString());
        }
    }

    string ConvertToArabicDigits(string input)
    {
        string[] arabicDigits = { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
        string result = "";
        foreach (char c in input)
        {
            if (char.IsDigit(c)) result += arabicDigits[c - '0'];
            else result += c;
        }
        return result;
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
        Time.timeScale = 1f;

        string difficulty = SelectedDifficulty.ToString();
        PlayerPrefs.SetFloat("LastTime_" + difficulty, timeLeft);
        PlayerPrefs.Save();

        SceneManager.LoadScene("FailHarvest");
    }

    public void Win()
    {
        playerWon = true;
        Time.timeScale = 1f;

        string difficulty = SelectedDifficulty.ToString();
        float bestTime = PlayerPrefs.GetFloat("BestTime_" + difficulty, 0f);
        PlayerPrefs.SetFloat("LastTime_" + difficulty, timeLeft);

        if (timeLeft > bestTime)
        {
            PlayerPrefs.SetFloat("BestTime_" + difficulty, timeLeft);
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene("WinHarvest");
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
}
