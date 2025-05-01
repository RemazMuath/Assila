using UnityEngine;
using TMPro;
public class ResultScreenManager : MonoBehaviour
{
    public TextMeshProUGUI bestTimeNumber;
    public TextMeshProUGUI lastTimeNumber; // Optional for Fail Scene

    void Start()
    {
        // Fetch the best time from PlayerPrefs
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        bestTimeNumber.text = FormatTimeArabic(bestTime);

        // Check if it's FailScene, and display the LastTime
        if (lastTimeNumber != null) // Only in Fail Scene
        {
            float lastTime = PlayerPrefs.GetFloat("LastTime", 0f);
            lastTimeNumber.text = FormatTimeArabic(lastTime);
        }
    }

    string FormatTimeArabic(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{ToArabic(minutes)}:{ToArabic(seconds)}";
    }

    string ToArabic(int number)
    {
        string arabic = "";
        foreach (char c in number.ToString("00"))
            arabic += (char)('\u0660' + (c - '0'));
        return arabic;
    }
}
