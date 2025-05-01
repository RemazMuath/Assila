using UnityEngine;
using TMPro;

public class ResultScreenManager : MonoBehaviour
{
    public TextMeshProUGUI bestTimeNumber;
    public TextMeshProUGUI lastTimeNumber; // Optional for Fail Scene

    void Start()
    {
        // TEMP TEST — you can remove this part later
        bestTimeNumber.text = "TEST BEST TIME";
        if (lastTimeNumber != null)
            lastTimeNumber.text = "TEST LAST TIME";

        // Load difficulty-specific records
        string difficulty = PlayerPrefs.GetString("Difficulty", "Easy");

        float bestTime = PlayerPrefs.GetFloat("BestTime_" + difficulty, -1f);
        float lastTime = PlayerPrefs.GetFloat("LastTime_" + difficulty, -1f);

        Debug.Log($"[ResultScreenManager] BestTime Raw: {bestTime}, LastTime Raw: {lastTime}");

        if (bestTime >= 0)
            bestTimeNumber.text = FormatTimeArabic(bestTime);
        else
            bestTimeNumber.text = "??";

        if (lastTimeNumber != null)
        {
            if (lastTime >= 0)
                lastTimeNumber.text = FormatTimeArabic(lastTime);
            else
                lastTimeNumber.text = "??";
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
