using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonDF : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Tammar"); 
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("DifficultySelectionDF"); 
    }
    public void TestClick()
    {
        Debug.Log("Button Clicked!");
    }
}
