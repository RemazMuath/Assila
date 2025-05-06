using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyMenu : MonoBehaviour
{
    public void SelectDifficulty(int level)
    {
        GameManager.SelectedDifficulty = (DifficultyLevel)level;
        SceneManager.LoadScene("DateHarvest");
    }
}
