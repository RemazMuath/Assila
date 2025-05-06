using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using ArabicSupport;
using UnityEngine.SceneManagement;

public class MainGameplayScript : MonoBehaviour
{
    public GameObject background;
    public GameObject player;

    [Header("Countdown Images")]
    public Image countImage3;
    public Image countImage2;
    public Image countImage1;
    public Image countImage_GO;

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

        if (player != null)
        {
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
                animator.Play("Idle");

            player.SetActive(true);
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Static;
        }

        StartCoroutine(ImageCountdown());
    }

    void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                baseSpeed = 15f;
                accelerationRate = 1.0f;
                obstacleSpawnInterval = 2.0f;
                break;
            case "Medium":
                baseSpeed = 18f;
                accelerationRate = 1.0f;
                obstacleSpawnInterval = 1.8f;
                break;
            case "Hard":
                baseSpeed = 21f;
                accelerationRate = 1.0f;
                obstacleSpawnInterval = 1.2f;
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

        countImage_GO.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countImage_GO.gameObject.SetActive(false);

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Dynamic; // Unfreeze

            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Run"); // Switch to running animation
        }
        // After "GO" image disappears
        gameStarted = true;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        FindObjectOfType<GameTimerDF>()?.StartTimer(); // Start Timer

    }

    void Update()
    {
        if (!gameStarted) return;

        scrollSpeed = Mathf.Min(scrollSpeed + accelerationRate * Time.deltaTime, 60f);

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
        if (CanJump())
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical speed before jumping
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    bool CanJump()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        return rb != null && Mathf.Abs(rb.velocity.y) < 0.01f;
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
            return;

        int index = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[index], obstacleSpawnPoint.position, Quaternion.identity);
    }

    void CheckWinCondition(float currentTime)
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0f); // Default 0f

        if (currentTime > bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", currentTime);
            PlayerPrefs.Save();
            SceneManager.LoadScene("WinSceneDF");
        }
        else
        {
            SceneManager.LoadScene("FailSceneDF");
        }
    }
}
