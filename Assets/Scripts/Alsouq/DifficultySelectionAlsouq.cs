using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectionAlsouq : MonoBehaviour
{
    public void LoadEasy()
    {
        SceneManager.LoadScene("Memorization_easy");
    }

    public void LoadMedium()
    {
        SceneManager.LoadScene("Memorization_medium");
    }

    public void LoadHard()
    {
        SceneManager.LoadScene("Memorization_hard");
    }
}
