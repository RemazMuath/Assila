using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiSceneLoader : MonoBehaviour
{

    public void LoadDifficulty()
    {
        SceneManager.LoadScene("DifficultySelection");
    }

    public void LoadDifficultyDF()
    {
        SceneManager.LoadScene("DifficultySelectionDF");
    }

    public void LoadDifficultyAlsouq()
    {
        SceneManager.LoadScene("DifficultySelectionAlsouq");
    }
}
