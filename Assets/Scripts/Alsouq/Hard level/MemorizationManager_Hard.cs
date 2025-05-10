using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemorizationManager_Hard : MonoBehaviour
{
    [Header("Date Image Slots")]
    public List<Image> dateSlots;        // Assign slot 1 to 8
    public List<Image> nameSlots;        // Assign nameSlot 1 to 8

    [Header("All Available Sprites")]
    public List<Sprite> datePhotos;      // 23 date images
    public List<Sprite> labelPhotos;     // 23 Arabic label images

    [Header("Timer UI")]
    public TMP_Text timerText;    // Drag your TimerText object here in the Inspector

    private List<int> selectedIndices = new List<int>();
    private float countdown = 30f;
    private bool timerRunning = true;

    void Start()
    {
        FillSlotsWithRandomDates();
    }

    void Update()
    {
        if (!timerRunning) return;

        countdown -= Time.deltaTime;
        countdown = Mathf.Max(countdown, 0); // Clamp at zero

        if (timerText != null)
        {
            timerText.text = ToArabicNumerals(Mathf.CeilToInt(countdown).ToString());
        }

        if (countdown <= 0)
        {
            timerRunning = false;
            GoToNextScene();
        }
    }

    void FillSlotsWithRandomDates()
    {
        selectedIndices.Clear();

        while (selectedIndices.Count < 8)
        {
            int rand = Random.Range(0, datePhotos.Count);
            if (!selectedIndices.Contains(rand))
                selectedIndices.Add(rand);
        }

        for (int i = 0; i < 8; i++)
        {
            int index = selectedIndices[i];

            // Set date image
            dateSlots[i].sprite = datePhotos[index];

            // Set name image
            Image labelImg = nameSlots[i];
            labelImg.sprite = labelPhotos[index];
            labelImg.preserveAspect = true;
            labelImg.type = Image.Type.Simple;

            // Match positioning to its date slot
            RectTransform dateRT = dateSlots[i].GetComponent<RectTransform>();
            RectTransform labelRT = nameSlots[i].GetComponent<RectTransform>();

            // Match anchors/pivots
            labelRT.anchorMin = dateRT.anchorMin;
            labelRT.anchorMax = dateRT.anchorMax;
            labelRT.pivot = new Vector2(0.5f, 1f); // top center of label

            // Set label just below the date image
            labelRT.anchoredPosition = dateRT.anchoredPosition + new Vector2(0, -230); // adjust Y offset if needed
            labelRT.sizeDelta = new Vector2(400, 120);
            labelRT.localScale = Vector3.one;
        }

        // Save selected indices (for later use)
        PlayerPrefs.SetString("selectedDates", string.Join(",", selectedIndices));
    }

    void GoToNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene_hard"); // Replace with your actual scene name
    }

    // 🔤 Converts Western digits to Arabic numerals
    string ToArabicNumerals(string number)
    {
        char[] arabicDigits = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
        char[] result = new char[number.Length];

        for (int i = 0; i < number.Length; i++)
        {
            char c = number[i];
            if (char.IsDigit(c))
                result[i] = arabicDigits[c - '0'];
            else
                result[i] = c;
        }

        return new string(result);
    }
}
