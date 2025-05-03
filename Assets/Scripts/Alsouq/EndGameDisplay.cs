using UnityEngine;
using TMPro;

public class EndGameDisplay : MonoBehaviour
{
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        string lastLevel = PlayerPrefs.GetString("LastPlayedLevel", "Unknown");

        string bestKey = lastLevel.Contains("easy") ? "BestScore_Easy" :
                         lastLevel.Contains("medium") ? "BestScore_Medium" :
                         lastLevel.Contains("hard") ? "BestScore_Hard" :
                         "BestScore_Unknown";

        int bestScore = PlayerPrefs.GetInt(bestKey, 0);

        // Show only if assigned in Inspector
        if (lastScoreText != null)
            lastScoreText.text = lastScore.ToString();

        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString();
    }
}
