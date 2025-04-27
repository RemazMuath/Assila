using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene_Hard : MonoBehaviour
{
    [Header("Date Display")]
    public List<Image> slots;           // Assign 8 slots (Slot1 to Slot8)
    public List<Sprite> dateSprites;    // Assign all 23 date photos here

    [Header("Speech Bubble")]
    public Image askedNameImage;        // Image inside bubble to show the asked Arabic name
    public List<Sprite> labelSprites;   // 23 Arabic label photos (must match dateSprites order)

    private List<int> selectedIndices = new List<int>();
    private int currentAskedIndex; // To know which date he asked for

    // New Variables
    private int totalScore = 0;
    private int correctAnswers = 0;
    private int totalCustomers = 0;
    private float timeSinceCustomerAsked = 0f;

    void Start()
    {
        LoadSelectedDates();
        ShowDates();
        StartCustomerRequest(); // Start first customer
    }

    void Update()
    {
        timeSinceCustomerAsked += Time.deltaTime;
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
            {
                selectedIndices.Add(index);
            }
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

    public void StartCustomerRequest()
    {
        Debug.Log("Customer arrived! Ready to ask for a date...");

        int randomPick = Random.Range(0, selectedIndices.Count);
        currentAskedIndex = selectedIndices[randomPick];

        askedNameImage.sprite = labelSprites[currentAskedIndex];
        askedNameImage.preserveAspect = true;

        timeSinceCustomerAsked = 0f; // Reset the reaction time
    }

    public void OnDateClicked(int slotIndex)
    {
        int clickedDateIndex = selectedIndices[slotIndex];

        if (clickedDateIndex == currentAskedIndex)
        {
            // Correct click
            Debug.Log("Correct Answer!");

            correctAnswers++;

            if (timeSinceCustomerAsked <= 3f)
                totalScore += 10;
            else if (timeSinceCustomerAsked <= 5f)
                totalScore += 5;
            else if (timeSinceCustomerAsked <= 10f)
                totalScore += 1;
        }
        else
        {
            // Wrong click
            Debug.Log("Wrong Answer!");
            totalScore -= 5;
        }

        // After any click, move to next customer
        NextCustomer();
    }

    public void NextCustomer()
    {
        totalCustomers++;

        // You can add "man walks away" animation here if you want later

        StartCustomerRequest();
    }

    public void SpawnNewCustomer()
    {
        Debug.Log("Spawning new customer...");

        // You can later instantiate a new man prefab here
        // Example (if you have a prefab set up):
        // Instantiate(manPrefab, startingPosition, Quaternion.identity);

        // For now, you can just print for testing
    }

}
