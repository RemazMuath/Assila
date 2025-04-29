using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemorizationManager_Easy : MonoBehaviour
{
    [Header("Date Image Slots")]
    public List<Image> dateSlots;        // ✅ Only assign 4 slots
    public List<Image> nameSlots;        // ✅ Only assign 4 name slots

    [Header("All Available Sprites")]
    public List<Sprite> datePhotos;      // 23 date images
    public List<Sprite> labelPhotos;     // 23 Arabic label images

    [Header("Timer UI")]
    public TMP_Text timerText;

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
        countdown = Mathf.Max(countdown, 0);

        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(countdown).ToString();
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

        while (selectedIndices.Count < 4) // ✅ Only 4 slots
        {
            int rand = Random.Range(0, datePhotos.Count);
            if (!selectedIndices.Contains(rand))
                selectedIndices.Add(rand);
        }

        for (int i = 0; i < 4; i++)
        {
            int index = selectedIndices[i];

            // Set date image
            dateSlots[i].sprite = datePhotos[index];

            // Set name image
            Image labelImg = nameSlots[i];
            labelImg.sprite = labelPhotos[index];
            labelImg.preserveAspect = true;
            labelImg.type = Image.Type.Simple;

            // Match positioning
            RectTransform dateRT = dateSlots[i].GetComponent<RectTransform>();
            RectTransform labelRT = nameSlots[i].GetComponent<RectTransform>();

            labelRT.anchorMin = dateRT.anchorMin;
            labelRT.anchorMax = dateRT.anchorMax;
            labelRT.pivot = new Vector2(0.5f, 1f);

            labelRT.anchoredPosition = dateRT.anchoredPosition + new Vector2(0, -230);
            labelRT.sizeDelta = new Vector2(400, 120);
            labelRT.localScale = Vector3.one;
        }

        PlayerPrefs.SetString("selectedDates", string.Join(",", selectedIndices));
    }

    void GoToNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene_easy"); // ✅ Must match your easy game scene
    }
}
