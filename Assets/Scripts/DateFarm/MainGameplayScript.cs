using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using ArabicSupport;

public class MainGameplayScript : MonoBehaviour
{
    public GameObject background;
    public GameObject player;

    [Header("Countdown Images")]
    public Image countImage3;
    public Image countImage2;
    public Image countImage1;
    public TextMeshProUGUI countTextGo;

    public GameObject[] obstaclePrefabs;
    public Transform obstacleSpawnPoint;
    public float jumpForce = 10f;

    private float obstacleSpawnTimer = 0f;
    private float obstacleSpawnInterval = 2f;
    private float baseSpeed = 1f;
    [HideInInspector] public float scrollSpeed;
    private float accelerationRate = 0.005f;
    [HideInInspector] public bool gameStarted = false;

    void Start()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Easy");
        SetDifficulty(difficulty);

        // ❌ Hide Assila at start
        if (player != null)
            player.SetActive(false);

        StartCoroutine(ImageCountdown());
    }

    void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                baseSpeed = 40f;
                accelerationRate = 2.0f;
                obstacleSpawnInterval = 2.5f;
                break;
            case "Medium":
                baseSpeed = 70f;
                accelerationRate = 2.5f;
                obstacleSpawnInterval = 1.8f;
                break;
            case "Hard":
                baseSpeed = 100f;
                accelerationRate = 3.0f;
                obstacleSpawnInterval = 1f;
                break;
        }

        scrollSpeed = baseSpeed;
    }

    IEnumerator ImageCountdown()
    {
        countImage3.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countImage3.gameObject.SetActive(false);

        countImage2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countImage2.gameObject.SetActive(false);

        countImage1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countImage1.gameObject.SetActive(false);

        countTextGo.gameObject.SetActive(true);
        countTextGo.text = ArabicFixer.Fix("إنطلااااااق");

        // ✅ Show Assila now
        if (player != null)
            player.SetActive(true);

        yield return new WaitForSeconds(1f);
        countTextGo.gameObject.SetActive(false);

        // ✅ Game officially starts now
        gameStarted = true;
    }

    void Update()
    {
        if (!gameStarted) return;

        scrollSpeed += accelerationRate * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        obstacleSpawnTimer += Time.deltaTime;
        if (obstacleSpawnTimer >= obstacleSpawnInterval)
        {
            SpawnObstacle();
            obstacleSpawnTimer = 0f;
        }
    }

    void TryJump()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
            return;

        int index = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[index], obstacleSpawnPoint.position, Quaternion.identity);
    }
}
