using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameScene_Easy : MonoBehaviour // 🆕 CHANGE class name
{
    [Header("Date Display")]
    public List<Image> slots;
    public List<Sprite> dateSprites;

    [Header("Speech Bubble")]
    public List<Sprite> labelSprites;

    [Header("Man Spawning")]
    public GameObject walkingManPrefab;
    public Transform manParent;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    private List<int> selectedIndices = new List<int>();
    private int currentAskedIndex;
    private int nextAskedIndex;

    private int totalScore = 0;
    private int correctAnswers = 0;
    private int totalCustomers = 0;
    private float timeSinceCustomerAsked = 0f;

    private float countdownTime = 60f;
    private bool gameOver = false;
    private bool canClick = false;

    void Start()
    {
        LoadSelectedDates();
        ShowDates();
        SpawnNewCustomer();
    }

    void Update()
    {
        if (!gameOver)
        {
            countdownTime -= Time.deltaTime;

            if (countdownTime <= 0f)
            {
                countdownTime = 0f;
                EndGame();
            }

            UpdateUI();
        }
    }

    void LoadSelectedDates()
    {
        string saved = PlayerPrefs.GetString("selectedDates", "");
        if (string.IsNullOrEmpty(saved))
        {
            Debug.LogError("No saved dates found!");
            return;
        }

        string[] parts = saved.Split(',');
        foreach (string s in parts)
        {
            if (int.TryParse(s, out int index))
                selectedIndices.Add(index);
        }
    }

    void ShowDates()
    {
        for (int i = 0; i < selectedIndices.Count && i < slots.Count; i++)
        {
            slots[i].sprite = dateSprites[selectedIndices[i]];
            slots[i].preserveAspect = true;
        }
    }

    public void PrepareNextQuestion()
    {
        int randomPick = Random.Range(0, selectedIndices.Count);
        nextAskedIndex = randomPick;
    }

    public Sprite GetAskedNameSprite()
    {
        return labelSprites[selectedIndices[nextAskedIndex]];
    }

    public void ConfirmAskedName()
    {
        currentAskedIndex = selectedIndices[nextAskedIndex];
        timeSinceCustomerAsked = 0f;
        canClick = true;
    }

    public void OnDateClicked(int slotIndex)
    {
        if (gameOver || !canClick)
            return;

        int clickedDateIndex = selectedIndices[slotIndex];

        if (clickedDateIndex == currentAskedIndex)
        {
            Debug.Log("Correct Answer!");
            correctAnswers++;
            totalScore += 10;
            FindObjectOfType<ExcellentFeedback>().ShowExcellent();    // ✅ Call the excellent popup animation
        }
        else
        {
            Debug.Log("Wrong Answer!");
            totalScore -= 10;
            if (totalScore < 0)
                totalScore = 0;
        }

        canClick = false;

        WalkingMan man = FindObjectOfType<WalkingMan>();
        if (man != null)
            man.StartWalkingOut();
    }

    public void SpawnNewCustomer()
    {
        if (!gameOver && walkingManPrefab != null)
            StartCoroutine(SpawnManAfterDelay(0.1f));
    }

    private IEnumerator SpawnManAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!gameOver)
        {
            GameObject newMan = Instantiate(walkingManPrefab, manParent);
            RectTransform manRect = newMan.GetComponent<RectTransform>();

            if (manRect != null)
                manRect.anchoredPosition = new Vector2(2500f, 0f);

            PrepareNextQuestion();
        }
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(countdownTime / 60f);
            int seconds = Mathf.FloorToInt(countdownTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (scoreText != null)
            scoreText.text = totalScore.ToString();
    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        gameOver = true;

        // Save current run
        PlayerPrefs.SetInt("LastScore", totalScore);
        string sceneName = SceneManager.GetActiveScene().name.ToLower(); // 👈 normalize to lowercase
        PlayerPrefs.SetString("LastPlayedLevel", sceneName);

        // Determine best score key (case-insensitive check)
        string bestKey = sceneName.Contains("easy") ? "BestScore_Easy" :
                         sceneName.Contains("medium") ? "BestScore_Medium" :
                         sceneName.Contains("hard") ? "BestScore_Hard" :
                         "BestScore_Unknown";

        int previousBest = PlayerPrefs.GetInt(bestKey, 0);
        if (totalScore > previousBest)
        {
            PlayerPrefs.SetInt(bestKey, totalScore);
            Debug.Log("✅ New best score saved: " + totalScore);
        }
        else
        {
            Debug.Log("ℹ️ Score was lower, best remains: " + previousBest);
        }

        PlayerPrefs.Save();

        // Load win/lose screen
        if (totalScore >= 70)
            SceneManager.LoadScene("WinScene");
        else
            SceneManager.LoadScene("LoseScene");
    }




    public bool IsGameOver()
    {
        return gameOver;
    }

}