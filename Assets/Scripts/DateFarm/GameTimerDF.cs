using UnityEngine;
using TMPro;

public class GameTimerDF : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timer = 0f;
    private bool isRunning = false;
    private float bestTime = 0f;

    public float GetCurrentTime()
    {
        return timer;
    }

    public float GetBestTime()
    {
        return bestTime;
    }

    public void UpdateBestTime()
    {
        if (timer > bestTime)
        {
            bestTime = timer;
            PlayerPrefs.SetFloat("BestTime", bestTime); 
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        timerText.text = "٠٠:٠٠";
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f); 
    }

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = $"{ToArabicNumber(minutes)}:{ToArabicNumber(seconds)}";
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    private string ToArabicNumber(int number)
    {
        string english = number.ToString("00");
        string arabic = "";
        foreach (char c in english)
            arabic += (char)('\u0660' + (c - '0'));
        return arabic;
    }
}
