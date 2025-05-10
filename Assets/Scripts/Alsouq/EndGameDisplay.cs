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

        if (lastScoreText != null)
            lastScoreText.text = ToArabicNumerals(lastScore.ToString());

        if (bestScoreText != null)
            bestScoreText.text = ToArabicNumerals(bestScore.ToString());
    }

    // 🔤 Converts Western digits to Arabic numerals
    private string ToArabicNumerals(string number)
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
