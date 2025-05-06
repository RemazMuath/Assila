using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonDH : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Tammar");
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("DifficultySelection");
    }
    public void TestClick()
    {
        Debug.Log("Button Clicked!");
    }
}
